#!/usr/bin/env python3
"""Fetch the libhegel native libraries from the hegel-rust GitHub releases and
lay them out for NuGet packaging / local runs under runtimes/<rid>/native/.

The .NET port of hegel-java's scripts/fetch_natives.py.

Release asset names (lib-prefixed on every platform):
    libhegel-<os>-<arch>.(so|dylib|dll)        e.g. libhegel-linux-amd64.so
with a sibling <name>.sha256 (GNU sha256sum format).

Output is always `libhegel.(so|dylib|dll)`: the binding imports the native as
`libhegel` (not `hegel`) so that on case-insensitive Windows it never collides with
the managed `Hegel.dll` assembly.

By default this fetches only the *host* RID (fast, for local dev / the test
suite). Pass `--rid all` to populate the full matrix for packaging.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import platform
import sys
import urllib.error
import urllib.request
from pathlib import Path

DEFAULT_REPO = "hegeldev/hegel-rust"

# (os, arch) in the release asset name  ->  (.NET RID, native extension).
ASSET_TO_RID: dict[tuple[str, str], tuple[str, str]] = {
    ("linux", "amd64"): ("linux-x64", "so"),
    ("linux", "arm64"): ("linux-arm64", "so"),
    ("darwin", "amd64"): ("osx-x64", "dylib"),
    ("darwin", "arm64"): ("osx-arm64", "dylib"),
    ("windows", "amd64"): ("win-x64", "dll"),
    ("windows", "arm64"): ("win-arm64", "dll"),
}
RID_TO_ASSET = {rid: key for key, (rid, _ext) in ASSET_TO_RID.items()}


def host_rid() -> str:
    """The .NET RID for the machine running this script."""
    sysname = {"darwin": "darwin", "linux": "linux", "win32": "windows"}.get(
        sys.platform, sys.platform
    )
    machine = platform.machine().lower()
    arch = "arm64" if machine in ("arm64", "aarch64") else "amd64"
    rid, _ext = ASSET_TO_RID.get((sysname, arch), (None, None))
    if rid is None:
        sys.exit(f"[fetch-natives] unsupported host platform: {sysname}/{machine}")
    return rid


def _request(url: str, accept: str = "application/octet-stream") -> urllib.request.Request:
    headers = {"User-Agent": "hegel-dotnet-fetch-natives", "Accept": accept}
    token = os.environ.get("GITHUB_TOKEN") or os.environ.get("GH_TOKEN")
    if token:
        headers["Authorization"] = f"Bearer {token}"
    return urllib.request.Request(url, headers=headers)


def resolve_release(repo: str, version: str) -> dict:
    if version in ("latest", "", None):
        url = f"https://api.github.com/repos/{repo}/releases/latest"
    else:
        url = f"https://api.github.com/repos/{repo}/releases/tags/{version}"
    try:
        with urllib.request.urlopen(_request(url, "application/vnd.github+json")) as resp:
            return json.load(resp)
    except urllib.error.HTTPError as e:
        sys.exit(f"[fetch-natives] failed to resolve release {version!r} from {repo}: {e}")


def download(url: str) -> bytes:
    with urllib.request.urlopen(_request(url)) as resp:
        return resp.read()


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--version", default="latest", help="hegel-rust release tag (e.g. v0.23.1) or 'latest'")
    parser.add_argument("--repo", default=DEFAULT_REPO)
    parser.add_argument("--out", default="runtimes", help="output root (runtimes/<rid>/native/)")
    parser.add_argument(
        "--rid",
        default="current",
        help="'current' (host only, default), 'all', or a specific RID like 'osx-arm64'",
    )
    args = parser.parse_args(argv)

    if args.rid == "current":
        wanted = {host_rid()}
    elif args.rid == "all":
        wanted = set(RID_TO_ASSET)
    else:
        if args.rid not in RID_TO_ASSET:
            sys.exit(f"[fetch-natives] unknown RID {args.rid!r}; known: {', '.join(sorted(RID_TO_ASSET))}")
        wanted = {args.rid}

    release = resolve_release(args.repo, args.version)
    tag = release.get("tag_name", args.version)
    assets = {a["name"]: a["browser_download_url"] for a in release.get("assets", [])}
    out_root = Path(args.out)

    produced = 0
    for rid in sorted(wanted):
        os_name, arch = RID_TO_ASSET[rid]
        _, ext = ASSET_TO_RID[(os_name, arch)]
        asset = f"libhegel-{os_name}-{arch}.{ext}"
        if asset not in assets:
            sys.exit(f"[fetch-natives] release {tag} has no asset {asset!r}")

        dest = out_root / rid / "native" / f"libhegel.{ext}"
        print(f"[fetch-natives] {tag}: {asset} -> {dest}", file=sys.stderr)
        blob = download(assets[asset])

        sidecar = f"{asset}.sha256"
        if sidecar in assets:
            expected = download(assets[sidecar]).decode().split()[0].strip().lower()
            actual = hashlib.sha256(blob).hexdigest()
            if actual != expected:
                sys.exit(f"[fetch-natives] checksum mismatch for {asset}: expected {expected}, got {actual}")
        else:
            print(f"[fetch-natives] warning: no .sha256 sidecar for {asset}; skipping verification", file=sys.stderr)

        dest.parent.mkdir(parents=True, exist_ok=True)
        dest.write_bytes(blob)
        produced += 1

    if produced == 0:
        sys.exit("[fetch-natives] no natives were produced")

    print(f"[fetch-natives] done: {produced} native(s) for {tag}", file=sys.stderr)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
