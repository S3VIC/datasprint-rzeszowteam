Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$projectPath = Join-Path $PSScriptRoot "norm.cli/norm.cli.csproj"

if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
	Write-Error "Project file not found: $projectPath"
	exit 1
}

Write-Host "Building norm.cli in Release configuration..."
dotnet build $projectPath --configuration Release --nologo

if ($LASTEXITCODE -ne 0) {
	exit $LASTEXITCODE
}

Write-Host "Build completed successfully."
