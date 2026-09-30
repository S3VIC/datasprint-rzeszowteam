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

if [[ $# -gt 1 ]]; then
	echo "Usage: $0 [runtime-identifier]" >&2
	exit 1
fi

runtime_id="${1:-linux-x64}"

echo "Building norm.cli in Release configuration (self-contained, runtime: ${runtime_id})..."
dotnet build "${project_path}" --configuration Release --self-contained true --runtime "${runtime_id}" --nologo

echo "Self-contained build completed successfully."
