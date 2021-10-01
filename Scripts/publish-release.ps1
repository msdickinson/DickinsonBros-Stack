# Inputs
$sourceFolder = "D:\Source\Repos\DickinsonBros\Source"


# Vars
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $sourceFolder
$packgeOutputPath = C:\Packages\$versionSuffix 

# Pack source projects
foreach ($file in $files) {
    dotnet pack $sourceFolder\$file\$file.csproj -p:EmbedAllSources=true -p:DebugSymbols=true -p:DebugType=embedded  -c release --version-suffix $versionSuffix --output $packgeOutputPath
}

# Push packa