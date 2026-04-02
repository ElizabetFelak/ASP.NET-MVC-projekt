param(
    [string]$HookName = 'UnknownHook',
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$PayloadParts
)

$logPath = 'c:\Users\lolno\source\repos\ASP.NET-MVC-projekt\lab-1\agent_log.txt'

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
