# Inputs
$dickinsonBrosPath = "D:\Source\Repos\DickinsonBros\DickinsonBros.sln"
$dickinsonBrosSourcePath = "D:\Source\Repos\DickinsonBros\Source"
$packgeOutputPath = "C:\Packages"

# Consents
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $sourcePath

# Build solution
dotnet build $dickinsonBrosPath --configuration Release

# Pack source projects
foreach ($file in $files) {
   dotnet pack $dickinsonBrosSourcePath\$file\$file.csproj -c Release --version-suffix $VersionSuffix --output $packgeOutputPath
}