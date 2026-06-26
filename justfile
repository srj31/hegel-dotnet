# hegel-dotnet task runner. Run `just` to list tasks.
# Inside `nix develop` (or with direnv) these use the pinned .NET 10 SDK.

default:
    @just --list

# Restore + build the whole solution.
build:
    dotnet build

# Run the test suite with coverage.
test:
    dotnet test --collect:"XPlat Code Coverage"

# Format F# sources with Fantomas (and verify formatting in CI with `just format-check`).
format:
    dotnet tool restore
    dotnet fantomas .

format-check:
    dotnet tool restore
    dotnet fantomas --check .

# Download the libhegel native binaries from the hegel-rust releases and lay
# them out under runtimes/<rid>/native/ for packaging. See build/fetch-natives.py.
fetch-natives version="latest":
    python3 build/fetch-natives.py --version {{version}}

# Pack the NuGet packages (after fetch-natives).
pack:
    dotnet pack -c Release -o artifacts
