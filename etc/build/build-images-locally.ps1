param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

$dbMigratorFolder = Join-Path $slnFolder "src/DMSpro.P42.DbMigrator"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t dmspro/p42-db-migrator:$version .




$webFolder = Join-Path $slnFolder "src/DMSpro.P42.Web"

Write-Host "********* BUILDING Web Application *********" -ForegroundColor Green
Set-Location $webFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t dmspro/p42-web:$version .

$hostFolder = Join-Path $slnFolder "src/DMSpro.P42.HttpApi.Host"
$identityServerAppFolder = Join-Path $slnFolder "src/DMSpro.P42.IdentityServer"

Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
Set-Location $hostFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t dmspro/p42-api:$version .

Write-Host "********* BUILDING IdentityServer Application *********" -ForegroundColor Green
Set-Location $identityServerAppFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t dmspro/p42-identityserver:$version .


### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder