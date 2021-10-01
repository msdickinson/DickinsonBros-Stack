# Inputs
$sourceFolder = "D:\Source\Repos\DickinsonBros\Source"
$packgeOutputPath = "C:\Packages\temp\"
$packageServer = "http://localhost:5000/v3/index.json"

# Vars
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "-alpha" + $dateTime
$files = Get-ChildItem -Directory -Path $sourceFolder

# Pack source projects
foreach ($file in $files) {

       $projectFullPath = $sourceFolder + "\$file\$file.csproj"
       $packageFileName = -join ($file,".1.0.0-",$versionSuffix,".nupkg")
       $packageFullPath = $packgeOutputPath + $packageFileName
 
       dotnet pack $projectFullPath --include-symbols --include-source -p:SymbolPackageFormat=snupkg -p:EmbedUntrackedSources=true -p:EmbedAllSources=true -p:DebugSymbols=true -p:DebugType=portable -c debug --version-suffix $versionSuffix --output $packgeOutputPath
       dotnet nuget push -s $packageServer $packageFullPath
    }
}