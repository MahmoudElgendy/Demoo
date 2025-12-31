# Define a log file path
$global:logFile = "deployment.log"

# Logging function
function Log-Message {
    param (
        [string]$Message,
        [ConsoleColor]$color = "red"
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "$timestamp - $Message"
    # Output to console
    Write-Host $logEntry -ForegroundColor $color
    # Append to log file
    # $logEntry | Out-File -Append -FilePath $logFile
}

function Log-Error {
    param (
        [string]$Message
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "$timestamp - $Message"
    Write-Error $logEntry
}