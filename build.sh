#!/usr/bin/env bash
set -euo pipefail

# BASH_SOURCE[0] expands to the path used to invoke this script file.
# Using dirname+cd+pwd gives us an absolute directory path to the script location.
script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
project_path="${script_dir}/norm.cli/norm.cli.csproj"

if [[ ! -f "${project_path}" ]]; then
	echo "Project file not found: ${project_path}" >&2
	exit 1
fi

echo "Building norm.cli in Release configuration..."
dotnet build "${project_path}" --configuration Release --nologo

echo "Build completed successfully."

