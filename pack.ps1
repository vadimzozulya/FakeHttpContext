param
(
  [Parameter(Mandatory=$false)][string] $nugetPackagesOutput = $PSScriptRoot + "\output"
)

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
  $nugetCmd = "nuget pack $nuspecFile $params"
  iex $nugetCmd
}
