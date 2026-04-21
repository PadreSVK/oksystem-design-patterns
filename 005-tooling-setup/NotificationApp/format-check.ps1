#!/usr/bin/env pwsh
# =============================================================================
# format-check.ps1 — CI format gate (Hands-On 005, step 6)
#
# Usage:
#   ./format-check.ps1                      # check current project
#   ./format-check.ps1 -Project <path>      # check a specific .csproj or folder
#
# Exit codes:
#   0  — no formatting violations; safe to merge
#   1  — violations found; run 'dotnet format' locally to fix
#
# Add to GitHub Actions:
#   - name: Format check
#     run: pwsh hands-on/005-tooling-setup/NotificationApp/format-check.ps1
# =============================================================================

param(
    [string]$Project = $PSScriptRoot
)

Write-Host "Running dotnet format --verify-no-changes on: $Project"

dotnet format $Project --verify-no-changes --severity warn

if ($LASTEXITCODE -ne 0) {
    Write-Error "Formatting violations found. Run 'dotnet format' locally to fix them, then re-commit."
    exit 1
}

Write-Host "No formatting violations found."
exit 0
