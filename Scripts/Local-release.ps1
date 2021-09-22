# Inputs
$dickinsonBrosSourcePath = "D:\Source\Repos\DickinsonBros\Source"
$packgeOutputPath = "C:\Packages"

# Consents
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $dickinsonBrosSourcePath

# Pack source projects
foreach ($file in $files) {
   dotnet pack $dickinsonBrosSourcePath\$file\$file.csproj -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg  -c Release --version-suffix $VersionSuffix --output $packgeOutputPath
}