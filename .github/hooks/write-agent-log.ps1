param(
    [string]$HookName = 'UnknownHook',
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$PayloadParts
)
$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..\..')).Path
$outputDir = "lab-5"
$logDir = Join-Path $repoRoot $outputDir
if (-not (Test-Path $logDir)) {
    New-Item -ItemType Directory -Path $logDir -Force | Out-Null
}
$logPath = Join-Path $logDir 'agent_log.txt'

$payload = if ($PayloadParts -and $PayloadParts.Count -gt 0) {
    $PayloadParts -join ' '
} else {
    [Console]::In.ReadToEnd()
}

if (-not [string]::IsNullOrWhiteSpace($payload)) {
    $timestamp = Get-Date -Format o
    $entry = "[$timestamp] [$HookName] $($payload.Trim())"
    Add-Content -LiteralPath $logPath -Value $entry
}
