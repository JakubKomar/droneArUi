# Autor: Jakub Komárek
# Popis: Skript pro spuštění všech potřebných aplikací pro správný chod dronServeru


# Získání aktuálního umístění skriptu
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Definition

#Cesty
$dockerPath = "C:\Program Files\Docker\Docker\Docker Desktop.exe" 
$videoServerPath = "$scriptDirectory\videoServer\MonaServer.exe"
$monitorPath = "$scriptDirectory\dronServerMonitor\DroCo.exe"
$dockerTelemetryDirPath = "$scriptDirectory\dronServer\compose_files\" 

# Spuštění video serveru
Start-Process -FilePath $videoServerPath

# Spuštění monitorovací aplikace
Start-Process -FilePath $monitorPath

# Spuštění Dockeru
Start-Process -FilePath $dockerPath
Start-Sleep -Seconds 20

# Spuštění docker aplikace - telemetrický server
Set-Location -Path $dockerTelemetryDirPath 
# Spuštění příkazu docker compose up
Start-Process -FilePath "docker" -ArgumentList "compose", "up" -NoNewWindow

