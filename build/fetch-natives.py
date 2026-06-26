#!/usr/bin/env python3
"""Fetch the libhegel native libraries from the hegel-rust GitHub releases and
lay them out for NuGet packaging under runtimes/<rid>/native/libhegel.<ext>.

This is the .NET port of hegel-java's scripts/fetch_natives.py. It is a STUB:
the layout/asset logic is sketched but the download + sha256 verification are
not implemented yet. See docs/ARCHITECTURE.md.

Asset name pattern in hegel-rust releases:
    libhegel-<os>-<arch>.(so|dylib|dll)        e.g. libhegel-linux-amd64.so
with a sibling <name>.sha256 (GNU sha256sum format).

Unlike hegel-java (which skips Windows), hegel-dotnet targets Windows too — the
engine publishes win-amd64/win-arm64 DLLs.
"""

from __future__ import annotations

import argparse
import sys

DEFAULT_REPO = "hegeldev/hegel-rust"

# (os, arch) in the release asset name  ->  .NET RID + native extension.
ASSET_TO_RID: dict[tuple[str, str], tuple[str, str]] = {
    ("linux", "amd64"): ("linux-x64", "so"),
    ("linux", "arm64"): ("linux-arm64", "so"),
    ("darwin", "amd64"): ("osx-x64", "dylib"),
    ("darwin", "arm64"): ("osx-arm64", "dylib"),
    ("windows", "amd64"): ("win-x64", "dll"),
    ("windows", "arm64"): ("win-arm64", "dll"),
}


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--version", default="latest", help="hegel-rust release tag (e.g. v0.23.1) or 'latest'")
    parser.add_argument("--repo", default=DEFAULT_REPO)
    parser.add_argument("--out", default="runtimes", help="output root (runtimes/<rid>/native/)")
    args = parser.parse_args(argv)

    # TODO:
    #  1. Resolve the release for --version (GitHub API: /releases/tags/v<version> or /releases/latest).
    #  2. For each asset matching libhegel-<os>-<arch>.(so|dylib|dll):
    #       - download it and its .sha256 sidecar,
    #       - verify the checksum,
    #       - copy to <out>/<rid>/native/libhegel.<ext>.
    #  3. Fail hard if no natives were produced.
    print(
        f"[fetch-natives] STUB — would fetch libhegel {args.version} from "
        f"{args.repo} into {args.out}/<rid>/native/ for: "
        + ", ".join(rid for rid, _ in ASSET_TO_RID.values()),
        file=sys.stderr,
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
