# Inputs
$packgeOutputPath = "C:\Packages"

# Consents
$dateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$versionSuffix = "v1-alpha" + $dateTime


# Pack source projects
   dotnet pack C:\Users\marks\source\repos\ConsoleApp1\SampleLib\SampleLib.csproj --include-symbols --include-source -p:SymbolPackageFormat=snupkg  -c debug--output $packgeOutputPath
  