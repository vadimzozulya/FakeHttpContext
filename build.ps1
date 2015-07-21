param
(
  [Parameter(Mandatory=$false)][string] $nugetPackagesOutput = $PSScriptRoot + "\output",
  [Parameter(Mandatory=$false)][string] $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe",
  [Parameter(Mandatory=$false)][string] $nugetExe = ".\src\.nuget\NuGet.exe",
  [switch] $pushPackage
)

function Ask-YesOrNo {
  param([string]$title="Confirm",[string]$message="Are you sure?")
  $choiceYes = New-Object System.Management.Automation.Host.ChoiceDescription "&Yes", "Answer Yes."
  $choiceNo = New-Object System.Management.Automation.Host.ChoiceDescription "&No", "Answer No."
  $options = [System.Management.Automation.Host.ChoiceDescription[]]($choiceYes, $choiceNo)
  $result = $host.ui.PromptForChoice($title, $message, $options, 1)
  return ($result -eq 0)
}

function Build-Solution(){
  $solution = ".\src\FakeHttpContext.sln"
  $target =  "/t:Build"
  $buildConfiguration = "/p:Configuration=Release"

  $buildCmd = "$msbuild  $solution $target $buildConfiguration"
  iex $buildCmd
}

function Build-Nuget-Packages(){
  if (Test-Path $nugetPackagesOutput) {
    gci -Path $nugetPackagesOutput -Include "*.*" -File -Recurse | foreach{ $_.Delete()}
  } else {
    New-Item -ItemType Directory -Path $nugetPackagesOutput
  }

  $assemblyInfoFilePath = '.\src\FakeHttpContext\Properties\AssemblyInfo.cs'
  $assemblyInformationalVersionRegex = "(\[assembly: AssemblyInformationalVersion\()(`")(.*)(`"\))"
  $existingVersion = (select-string -Path $assemblyInfoFilePath -Pattern $assemblyInformationalVersionRegex).Matches[0].Groups[3]

  Get-ChildItem -Path .\nuspec | foreach {
    $nuspecFile = $_.FullName
    $params = "-OutputDirectory $nugetPackagesOutput -version $existingVersion"
    $nugetCmd = "$nugetExe pack $nuspecFile $params"
    iex $nugetCmd
  }
}

function Push-Nuget-Package(){
  $packages = Get-ChildItem -Path $nugetPackagesOutput

  $names = [string]::Join("`n", $packages.Name)
  if (Ask-YesOrNo(" Will be pushed to the server: `n$names")) {
    $packages | foreach {
      $package = $_.FullName
      iex "$nugetExe push $package"
    }
  }
}

Build-Solution
Build-Nuget-Packages
if ($pushPackage) {
  Push-Nuget-Package
}