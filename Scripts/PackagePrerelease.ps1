$DateTime = [datetime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$VersionSuffix = "-alpha" + $DateTime
dotnet pack "DickinsonBros.Core.Redactor" -c Release --version-suffix $VersionSuffix --output C:\Packages
