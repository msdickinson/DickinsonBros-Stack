# Consents
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "alpha-" + $dateTime

# Inputs
$packgeOutputPath = "C:\Packages\Temp\"
$projectFolder = "C:\Users\marks\source\repos\ConsoleApp1\"
$packageName = "SampleLib"
$packageFileName = $packageName + ".1.0.0-" + $versionSuffix + ".nupkg"
$projectFullPath = $projectFolder + $packageName + "\" + $packageName + ".csproj"
$pacakgeFullPath = $packgeOutputPath + $packageFileName
$packageServer = "http://localhost:5000/v3/index.json"



Write-Output ""
Write-Output ("Project Full Path: " + $projectFullPath)
Write-Output ("Pacakge Output Path : " + $packgeOutputPath)
Write-Output ("Pacakge FullPath : " + $pacakgeFullPath)
Write-Output ("Pacakge Server : " + $packageServer)

Write-Output ("Version Suffix : " + $versionSuffix)
# Pack source projects"
dotnet pack C:\Users\marks\source\repos\ConsoleApp1\SampleLib\SampleLib.csproj -p:DebugType=embedded -c release --version-suffix $versionSuffix --output $packgeOutputPath
dotnet nuget push -s $packageServer $pacakgeFullPath
