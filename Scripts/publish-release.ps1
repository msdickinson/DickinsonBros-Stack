# Inputs
$dickinsonBrosSourcePath = "D:\Source\Repos\DickinsonBros\Source"
$packgeOutputPath = "C:\Packages"

# Consents
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $sourcePath

# Pack source projects
foreach ($file in $files) {
   dotnet pack $dickinsonBrosSourcePath\$file\$file.csproj --include-symbols -c debug --version-suffix $VersionSuffix --output $packgeOutputPath
}