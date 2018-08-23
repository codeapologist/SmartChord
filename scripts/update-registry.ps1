$registryPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\B\command"

$Name = '(default)'

$value = ""

$registryKey = Get-ItemProperty -Path $registryPath

$value = $registryKey.$Name
Write-Host $value


# IF(!(Test-Path $registryPath))
# {
# New-Item -Path $registryPath -Force | Out-Null
# }

# New-ItemProperty -Path $registryPath -Name $name -Value $value `
# -PropertyType DWORD -Force | Out-Null}