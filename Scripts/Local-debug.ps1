# Inputs
$sourceFolder = "D:\Source\Repos\DickinsonBros\Source"
$packgeOutputPath = "C:\Packages"

# Vars
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $sourceFolder

# Pack source projects
foreach ($file in $files) {
    dotnet pack $sourceFolder\$file\$file.csproj -p:EmbedAllSources=true -p:DebugSymbols=true -p:DebugType=embedded  -c debug --version-suffix $versionSuffix --output $packgeOutputPath
}