Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Push-Location (Join-Path $PSScriptRoot "..")
try {
    node tests\smoke-test.js
    node server\smoke-test.js
    node server\protocol-smoke-test.js
    node --check server\authoritative-server.js
    Write-Host "All Cricket Arena checks passed."
}
finally {
    Pop-Location
}
