
"..\tools\nuget\NuGet.exe" "pack" "..\src\Shay.Core\Shay.Core.csproj" -Properties Configuration=Release -IncludeReferencedProjects
"..\tools\nuget\NuGet.exe" "pack" "..\src\Shay.Dapper\Shay.Dapper.csproj" -Properties Configuration=Release -IncludeReferencedProjects
"..\tools\nuget\NuGet.exe" "pack" "..\src\Shay.Redis\Shay.Redis.csproj" -Properties Configuration=Release -IncludeReferencedProjects
"..\tools\nuget\NuGet.exe" "pack" "..\src\Shay.Framework\Shay.Framework.csproj" -Properties Configuration=Release -IncludeReferencedProjects