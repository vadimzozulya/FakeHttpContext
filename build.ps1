param
(
  [Parameter(Mandatory=$false)][string] $nugetPackagesOutput = $PSScriptRoot + "\output",
  [Parameter(Mandatory=$false)][string] $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe",
  [Parameter(Mandatory=$false)][string] $nugetExe = ".\src\.nuget\NuGet.exe"
)

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

Build-Solution
Build-Nuget-Packages