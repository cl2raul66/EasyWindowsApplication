#Requires -Version 5.1
<#
.SYNOPSIS
    Gestor interactivo de plantillas de Easy Windows Application.

.DESCRIPTION
    Permite reinstalar o desinstalar las plantillas 'easywinapp' y 'simpleeasywinapp'
    sin tener que recordar comandos. Al ejecutarlo se muestra un menu interactivo:

      1. Reinstalar desde el repositorio (src\ProjectTemplates)
      2. Reinstalar desde un paquete .nupkg local
      3. Desinstalar

    Salir: presiona [Esc]

    Uso:
        .\devtools\install-templates.ps1
#>

[CmdletBinding()]
param()

# ================= Configuracion =================
$RepoRoot      = Split-Path -Parent $PSScriptRoot
$TemplatesFolder = [IO.Path]::GetFullPath((Join-Path $RepoRoot 'src\ProjectTemplates')).TrimEnd('\')

function Test-DotNet {
    return $null -ne (Get-Command dotnet -ErrorAction SilentlyContinue)
}

function Get-InstallationStatus {
    param()

    $status = @{
        Installed  = $false
        Source     = 'none'
        RepoSource = $null
        PkgSources = @()
        Templates  = @()
        TemplateIds = @()
    }

    $output = (& dotnet new uninstall 2>&1 | Out-String)
    $currentSource = $null

    foreach ($line in ($output -split "\r?\n")) {
        if ($line -match '^\s{3}\S') {
            $currentSource = $line.Trim()
        }
        elseif ($line -match '^\s{6,}\S' -and $line -match 'easywinapp|simpleeasywinapp') {
            $status.Templates += $line.Trim()
            if ($line -match '\((easywinapp|simpleeasywinapp)\)') {
                $id = $Matches[1]
                if ($id -notin $status.TemplateIds) { $status.TemplateIds += $id }
            }
            if ($null -ne $currentSource) {
                $srcFull = ''
                try { $srcFull = [IO.Path]::GetFullPath($currentSource).TrimEnd('\') } catch { }
                if ($srcFull -ieq $TemplatesFolder) {
                    $status.RepoSource = $currentSource
                }
                elseif ($currentSource -notin $status.PkgSources) {
                    $status.PkgSources += $currentSource
                }
            }
        }
    }

    if ($status.Templates.Count -gt 0) {
        $status.Installed = $true
        $status.Source = if ($status.RepoSource) { 'repo' } else { 'pkg' }
    }
    return $status
}

function Show-Header {
    Clear-Host
    Write-Host ''
    Write-Host '====================================================' -ForegroundColor Cyan
    Write-Host '  Easy Windows Application - Gestor de Plantillas' -ForegroundColor Cyan
    Write-Host '====================================================' -ForegroundColor Cyan
    Write-Host ''
}

function Show-Status {
    param($Status)

    if ($Status.Installed) {
        $srcLabel = if ($Status.Source -eq 'repo') { 'desde repositorio' } else { 'desde paquete NuGet' }
        Write-Host ("  Estado : [OK] Plantillas instaladas ({0})" -f $srcLabel) -ForegroundColor Green
        if ($Status.TemplateIds.Count -gt 0) {
            Write-Host ("           templates: {0}" -f ($Status.TemplateIds -join ', ')) -ForegroundColor Green
        }
    }
    else {
        Write-Host '  Estado : [--] No hay plantillas instaladas' -ForegroundColor Yellow
    }
    Write-Host ''
}

function Show-MainMenu {
    Write-Host '  Que deseas hacer?'
    Write-Host ''
    Write-Host '   Preciona [1] para Reinstalar desde repositorio'
    Write-Host '   Preciona [2] para Reinstalar desde paquete NuGet'
    Write-Host '   Preciona [3] para Desinstalar'
    Write-Host ''
    Write-Host '  Presiona [Esc] para salir'
    Write-Host ''

    $key = $Host.UI.RawUI.ReadKey("NoEcho, IncludeKeyDown")
    switch ($key.VirtualKeyCode) {
        27 { return 'quit' }
        49 { return '1' }
        50 { return '2' }
        51 { return '3' }
        default { return 'invalid' }
    }
}

function Show-Confirm {
    param([string]$Action)

    Write-Host ''
    Write-Host ("  Continuar con '{0}'?" -f $Action) -ForegroundColor Yellow
    $resp = Read-Host '  [S/n]'
    return ($resp.Trim().ToLower() -ne 'n')
}

function Wait-BackToMenu {
    Write-Host '  Presiona [Enter] para volver al menu o [Esc] para salir.'
    $key = $Host.UI.RawUI.ReadKey("NoEcho, IncludeKeyDown")
    if ($key.VirtualKeyCode -eq 27) { exit }
    Write-Host ''
}

function Invoke-Uninstall {
    $status = Get-InstallationStatus
    if (-not $status.Installed) {
        Write-Host '  No hay plantillas instaladas.' -ForegroundColor Yellow
        return
    }

    $targets = @()
    if ($status.RepoSource) { $targets += $status.RepoSource }
    $targets += $status.PkgSources

    foreach ($t in $targets) {
        Write-Host ("  Desinstalando: {0}" -f $t) -ForegroundColor DarkGray
        & dotnet new uninstall $t | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Host ("  [ERROR] Fallo al desinstalar: {0}" -f $t) -ForegroundColor Red
        }
    }
}

function Install-FromRepo {
    $status = Get-InstallationStatus
    if ($status.Installed) {
        if (-not (Show-Confirm 'Reinstalar desde repositorio')) { return }
        Invoke-Uninstall
    }

    Write-Host ''
    Write-Host ("  Instalando desde: {0}" -f $TemplatesFolder) -ForegroundColor DarkGray
    & dotnet new install $TemplatesFolder
    if ($LASTEXITCODE -eq 0) {
        Write-Host ''
        Write-Host '  [OK] Plantillas instaladas desde el repositorio.' -ForegroundColor Green
    }
    else {
        Write-Host ''
        Write-Host '  [ERROR] Fallo al instalar desde el repositorio.' -ForegroundColor Red
    }
    Wait-BackToMenu
}

function Install-FromPackage {
    $status = Get-InstallationStatus
    if ($status.Installed) {
        if (-not (Show-Confirm 'Reinstalar desde paquete NuGet')) { return }
        Invoke-Uninstall
    }

    $nupkgBase = Join-Path $TemplatesFolder 'bin'
    $files = @(Get-ChildItem -Path $nupkgBase -Filter '*.nupkg' -Recurse -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -notmatch '\.(symbols|snupkg)\.nupkg$' })

    $pkg = $null

    if ($files.Count -eq 0) {
        Write-Host ''
        Write-Host '  No se encontraron paquetes .nupkg en src\ProjectTemplates\bin' -ForegroundColor Yellow
        Write-Host ''
        Write-Host '   [1] Escribir ruta manualmente'
        Write-Host '   [2] Volver al menu'
        Write-Host ''
        $c = Read-Host '  Selecciona [1-2]'
        if ($c.Trim() -eq '1') {
            $pkg = (Read-Host '  Ruta del paquete .nupkg').Trim().Trim('"')
            if (-not (Test-Path -LiteralPath $pkg)) {
                Write-Host '  [ERROR] La ruta no existe.' -ForegroundColor Red
                Wait-BackToMenu
                return
            }
        }
        else {
            return
        }
    }
    elseif ($files.Count -eq 1) {
        $pkg = $files[0].FullName
    }
    else {
        Write-Host ''
        Write-Host '  Paquetes encontrados:'
        for ($i = 0; $i -lt $files.Count; $i++) {
            Write-Host ("    [{0}] {1}" -f ($i + 1), $files[$i].Name)
        }
        Write-Host ''
        $c = Read-Host ("  Selecciona paquete [1-{0}]" -f $files.Count)
        $idx = 0
        if ([int]::TryParse($c.Trim(), [ref]$idx) -and $idx -ge 1 -and $idx -le $files.Count) {
            $pkg = $files[$idx - 1].FullName
        }
        else {
            Write-Host '  [ERROR] Seleccion no valida.' -ForegroundColor Red
            Wait-BackToMenu
            return
        }
    }

    if ($null -ne $pkg -and $pkg -ne '') {
        Write-Host ''
        Write-Host ("  Instalando desde: {0}" -f $pkg) -ForegroundColor DarkGray
        & dotnet new install $pkg
        if ($LASTEXITCODE -eq 0) {
            Write-Host ''
            Write-Host '  [OK] Plantillas instaladas desde el paquete.' -ForegroundColor Green
        }
        else {
            Write-Host ''
            Write-Host '  [ERROR] Fallo al instalar desde el paquete.' -ForegroundColor Red
        }
    }
    Wait-BackToMenu
}

function Uninstall-Templates {
    $status = Get-InstallationStatus
    if (-not $status.Installed) {
        Write-Host '  No hay plantillas instaladas.' -ForegroundColor Yellow
        Wait-BackToMenu
        return
    }
    if (-not (Show-Confirm 'Desinstalar')) { return }
    Invoke-Uninstall
    Write-Host ''
    Write-Host '  [OK] Plantillas desinstaladas.' -ForegroundColor Green
    Wait-BackToMenu
}

# ================= Ejecucion =================
if (-not (Test-DotNet)) {
    Write-Host ''
    Write-Host '  [ERROR] dotnet no encontrado en el PATH.' -ForegroundColor Red
    Write-Host '          Instala el .NET SDK y vuelve a intentarlo.'
    Write-Host ''
    exit 1
}

$running = $true
while ($running) {
    Show-Header
    $status = Get-InstallationStatus
    Show-Status $status
    $choice = Show-MainMenu

    switch ($choice) {
        '1' { Install-FromRepo }
        '2' { Install-FromPackage }
        '3' { Uninstall-Templates }
        'quit' { $running = $false }
        default {
            Write-Host ''
            Write-Host '  [ERROR] Opcion no valida.' -ForegroundColor Red
            Start-Sleep -Milliseconds 900
        }
    }
}
