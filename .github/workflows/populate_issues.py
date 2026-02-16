import os
import sys
import time
import requests
import yaml
from typing import Any, Dict, List, Optional, Tuple

GITHUB_API = "https://api.github.com"

def die(msg: str) -> None:
    print(msg, file=sys.stderr)
    sys.exit(1)

def gh_headers(token: str) -> Dict[str, str]:
    return {
        "Authorization": f"Bearer {token}",
        "Accept": "application/vnd.github+json",
        "X-GitHub-Api-Version": "2022-11-28",
        "User-Agent": "issue-populator",
    }

def request_json(method: str, url: str, token: str, **kwargs) -> Any:
    r = requests.request(method, url, headers=gh_headers(token), **kwargs)
    # Helpful rate-limit / abuse detection backoff
    if r.status_code in (429, 403) and "rate limit" in r.text.lower():
        time.sleep(3000)
        r = requests.request(method, url, headers=gh_headers(token), **kwargs)

    if r.status_code >= 400:
        die(f"GitHub API error {r.status_code} for {method} {url}\n{r.text}")
    if r.text.strip() == "":
        return None
    return r.json()

def paginate(url: str, token: str, params: Optional[Dict[str, Any]] = None) -> List[Any]:
    items: List[Any] = []
    page = 1
    while True:
        p = dict(params or {})
        p["per_page"] = 100
        p["page"] = page
        chunk = request_json("GET", url, token, params=p)
        if not chunk:
            break
        items.extend(chunk)
        if len(chunk) < 100:
            break
        page += 1
    return items

def repo_owner_name() -> Tuple[str, str]:
    repo = os.environ.get("GITHUB_REPOSITORY", "")
    if "/" not in repo:
        die("GITHUB_REPOSITORY not set or invalid")
    owner, name = repo.split("/", 1)
    return owner, name

def ensure_labels(owner: str, repo: str, token: str, labels: List[Dict[str, Any]]) -> None:
    existing = paginate(f"{GITHUB_API}/repos/{owner}/{repo}/labels", token)
    by_name = {l["name"]: l for l in existing}

    for lab in labels:
        name = lab["name"]
        color = lab.get("color", "ededed").lstrip("#")
        desc = lab.get("description", "")
        if name not in by_name:
            print(f"Creating label: {name}")
            request_json(
                "POST",
                f"{GITHUB_API}/repos/{owner}/{repo}/labels",
                token,
                json={"name": name, "color": color, "description": desc},
            )
        else:
            # Idempotent update: keep label in desired state
            cur = by_name[name]
            if cur.get("color", "").lower() != color.lower() or (cur.get("description") or "") != desc:
                print(f"Updating label: {name}")
                request_json(
                    "PATCH",
                    f"{GITHUB_API}/repos/{owner}/{repo}/labels/{requests.utils.quote(name, safe='')}",
                    token,
                    json={"new_name": name, "color": color, "description": desc},
                )

def ensure_milestones(owner: str, repo: str, token: str, milestones: List[Dict[str, Any]]) -> Dict[str, int]:
    # include closed milestones too, so reruns still find them
    existing = paginate(f"{GITHUB_API}/repos/{owner}/{repo}/milestones", token, params={"state": "all"})
    by_title = {m["title"]: m for m in existing}

    title_to_number: Dict[str, int] = {}
    for ms in milestones:
        title = ms["title"]
        desc = ms.get("description", "")
        state = ms.get("state", "open")  # optional

        if title not in by_title:
            print(f"Creating milestone: {title}")
            created = request_json(
                "POST",
                f"{GITHUB_API}/repos/{owner}/{repo}/milestones",
                token,
                json={"title": title, "description": desc, "state": state},
            )
            title_to_number[title] = int(created["number"])
        else:
            cur = by_title[title]
            number = int(cur["number"])
            title_to_number[title] = number

            # Idempotent update: ensure description/state match desired
            needs = False
            payload: Dict[str, Any] = {}
            if (cur.get("description") or "") != desc:
                payload["description"] = desc
                needs = True
            if cur.get("state") != state:
                payload["state"] = state
                needs = True
            if needs:
                print(f"Updating milestone: {title}")
                request_json(
                    "PATCH",
                    f"{GITHUB_API}/repos/{owner}/{repo}/milestones/{number}",
                    token,
                    json=payload,
                )

    return title_to_number

def find_issue_by_backlog_id(owner: str, repo: str, token: str, backlog_id: str) -> Optional[int]:
    # Search API is simplest and avoids scanning all issues.
    # We embed: <!-- backlog-id: XYZ -->
    q = f'repo:{owner}/{repo} "backlog-id: {backlog_id}" in:body type:issue'
    data = request_json("GET", f"{GITHUB_API}/search/issues", token, params={"q": q, "per_page": 5})
    items = data.get("items", [])
    if not items:
        return None
    # Return the first match
    return int(items[0]["number"])

def normalize_issue_body(backlog_id: str, body: str) -> str:
    marker = f"<!-- backlog-id: {backlog_id} -->"
    # ensure marker is present at top for stable searches
    body = body or ""
    if marker in body:
        return body
    return marker + "\n\n" + body

def upsert_issue(
    owner: str,
    repo: str,
    token: str,
    issue: Dict[str, Any],
    milestone_number_by_title: Dict[str, int],
) -> None:
    backlog_id = issue.get("id")
    if not backlog_id:
        die("Every issue must have an 'id' field for idempotency.")

    title = issue["title"]
    body = normalize_issue_body(backlog_id, issue.get("body", ""))
    labels = issue.get("labels", []) or []
    milestone_title = issue.get("milestone")
    milestone_number = None
    if milestone_title:
        milestone_number = milestone_number_by_title.get(milestone_title)
        if milestone_number is None:
            die(f"Issue {backlog_id} references unknown milestone: {milestone_title}")

    existing_number = find_issue_by_backlog_id(owner, repo, token, backlog_id)

    payload: Dict[str, Any] = {
        "title": title,
        "body": body,
        "labels": labels,
    }
    if milestone_number is not None:
        payload["milestone"] = milestone_number

    if existing_number is None:
        print(f"Creating issue [{backlog_id}]: {title}")
        request_json(
            "POST",
            f"{GITHUB_API}/repos/{owner}/{repo}/issues",
            token,
            json=payload,
        )
    else:
        print(f"Updating issue [{backlog_id}] (#{existing_number}): {title}")
        request_json(
            "PATCH",
            f"{GITHUB_API}/repos/{owner}/{repo}/issues/{existing_number}",
            token,
            json=payload,
        )

def main() -> None:
    token = os.environ.get("GITHUB_TOKEN")
    backlog_file = os.environ.get("BACKLOG_FILE", "issues.yml")
    if not token:
        die("GITHUB_TOKEN not set (did you run in GitHub Actions with permissions?)")

    if not os.path.exists(backlog_file):
        die(f"Backlog file not found: {backlog_file}")

    with open(backlog_file, "r", encoding="utf-8") as f:
        spec = yaml.safe_load(f)

    owner, repo = repo_owner_name()

    labels = spec.get("labels", []) or []
    milestones = spec.get("milestones", []) or []
    issues = spec.get("issues", []) or []

    print(f"Repo: {owner}/{repo}")
    print(f"Backlog file: {backlog_file}")
    print(f"Labels: {len(labels)}, milestones: {len(milestones)}, issues: {len(issues)}")

    if labels:
        ensure_labels(owner, repo, token, labels)
    ms_map = ensure_milestones(owner, repo, token, milestones) if milestones else {}

    for issue in issues:
        upsert_issue(owner, repo, token, issue, ms_map)

    print("Done. (Idempotent upsert completed)")

if __name__ == "__main__":
    main()

