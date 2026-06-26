{
  description = "hegel-dotnet — .NET client for the Hegel property-based testing protocol";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };
      in
      {
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [
            dotnet-sdk_10 # .NET 10 LTS SDK (matches global.json + net10.0 TFM)
            python3 # build/fetch-natives.py (downloads libhegel from hegel-rust releases)
            just # task runner (matches the upstream hegel-* repos)
            git
          ];

          shellHook = ''
            export DOTNET_CLI_TELEMETRY_OPTOUT=1
            export DOTNET_NOLOGO=1
            echo "hegel-dotnet dev shell"
            echo "  dotnet  $(dotnet --version)"
            echo "  just    $(just --version 2>/dev/null | cut -d' ' -f2)"
          '';
        };
      }
    );
}
