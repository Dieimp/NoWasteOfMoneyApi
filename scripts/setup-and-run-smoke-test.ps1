#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$TestProject = Join-Path $RepoRoot "tests\NoWasteOfMoney.Tests\NoWasteOfMoney.Tests.csproj"

function Find-BackendContainer {
    $priorityFilters = @(
        "name=nowasteofmoney_api",
        "name=nowasteofmoney-api",
        "name=api"
    )

    foreach ($filter in $priorityFilters) {
        $match = docker ps --filter $filter --format "{{.ID}}|{{.Names}}|{{.Ports}}" 2>$null |
            Select-Object -First 1
        if ($match) { return $match }
    }

    return docker ps --format "{{.ID}}|{{.Names}}|{{.Ports}}" 2>$null |
        Where-Object { $_ -match '(?i)api|backend|nowasteofmoney' } |
        Select-Object -First 1
}

function Get-HostPortFromMapping {
    param([string]$Ports)
    if ($Ports -match '0\.0\.0\.0:(\d+)->') { return $matches[1] }
    if ($Ports -match ':(\d+)->') { return $matches[1] }
    return $null
}

Write-Host "[1/4] Detecting backend container..."
$containerLine = Find-BackendContainer

if (-not $containerLine) {
    Write-Warning "No backend container found. Using API_BASE_URL=http://localhost:8080"
    $env:API_BASE_URL = "http://localhost:8080"
}
else {
    $parts = $containerLine -split '\|', 3
    $containerId = $parts[0]
    $containerName = $parts[1]
    $containerPorts = $parts[2]
    $hostPort = Get-HostPortFromMapping -Ports $containerPorts

    if (-not $hostPort) { $hostPort = "8080" }

    $env:API_BASE_URL = "http://localhost:$hostPort"
    Write-Host "Container: $containerName ($containerId)"
    Write-Host "API_BASE_URL: $env:API_BASE_URL"
}

Write-Host "[2/4] Ensuring test project exists..."
if (-not (Test-Path $TestProject)) {
    throw "Test project not found at $TestProject"
}

Write-Host "[3/4] Restoring test dependencies (idempotent)..."
dotnet restore $TestProject

Write-Host "[4/4] Running smoke tests..."
dotnet test $TestProject --no-restore --verbosity normal
