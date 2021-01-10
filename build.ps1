param(
    [switch] $main,
    [switch] $disguise,
    [switch] $test
)
function GetSolutionFolder2(){
    [string] $p = $SCRIPT:MyInvocation.MyCommand.Path
    if (-not ([string]::IsNullOrEmpty($p))){
        $p = Split-Path -Path $p -Parent
        return $p
    } else {
        $p = Get-Location
        $p
    }
}
function GetSolutionFolder(){
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value
    Split-Path $Invocation.MyCommand.Path
}

[string] $solutionFolder = GetSolutionFolder
Write-Host "SolutionFolder: $($solutionFolder)" -ForegroundColor Yellow
Set-Location $solutionFolder

function  buildDefault {
    param (
        [string] $Configuration
    )
    Write-Host "- default -" -ForegroundColor Yellow

    if ([string]::IsNullOrEmpty($Configuration)) { $Configuration = "Debug" }

    & dotnet build "$($solutionFolder)\Brimborium.Json.sln" --configuration $Configuration
    if (-not $?) { return }

    & dotnet build "$($solutionFolder)\sample\SampleApp1.sln" --configuration $Configuration
    if (-not $?) { return }

    Write-Host "- ok -" -ForegroundColor Yellow
}

function  buildDisguise {
    param (
        [string] $Configuration
    )
    Write-Host "- disguise -" -ForegroundColor Yellow
    
    if ([string]::IsNullOrEmpty($Configuration)) { $Configuration = "Debug" }
    
    & dotnet build "$($solutionFolder)\src\Brimborium.Disguise\Brimborium.Disguise.csproj" --configuration $Configuration
    if (-not $?) { return }
    
    & dotnet build "$($solutionFolder)\src\Brimborium.Disguise.RunTime\Brimborium.Disguise.RunTime.csproj" --configuration $Configuration
    if (-not $?) { return }
    
    & dotnet build "$($solutionFolder)\src\Brimborium.Disguise.CompileTime\Brimborium.Disguise.CompileTime.csproj" --configuration $Configuration
    if (-not $?) { return }

    Write-Host "- ok -" -ForegroundColor Yellow
}


function  buildTest {
    param (
        [string] $Configuration
    )
    Write-Host "- test -" -ForegroundColor Yellow
    
    if ([string]::IsNullOrEmpty($Configuration)) { $Configuration = "Debug" }
    
    & dotnet test "$($solutionFolder)\test\Brimborium.Disguise.Test\Brimborium.Disguise.Test.csproj" --configuration $Configuration
    if (-not $?) { return }

    Write-Host "- ok -" -ForegroundColor Yellow
}

[bool] $default = -not( $main.ToBool() -or $disguise.ToBool() -or $test.ToBool() )

if ($default -or $main.ToBool()){    
    buildDefault
}
if ($disguise.ToBool()){
    buildDisguise
}
if ($test.ToBool()){
    buildTest
}


Write-Host "- fini -" -ForegroundColor White