# Brimborium.Json
Featured and fast JSON De-Serialization

a experiment


dotnet new sln




dotnet new classlib -n Brimborium.Json -o src\Brimborium.Json -f netcoreapp3.1
dotnet sln add src\Brimborium.Json

dotnet new xunit -n Brimborium.Json.Test -o test\Brimborium.Json.Test -f netcoreapp3.1
dotnet sln add test\Brimborium.Json.Test

dotnet new console -n microsoft.botsay -f net5.0

dotnet new console -n Brimborium.Json.Tool -o src\Brimborium.Json.Tool -f netcoreapp3.1
dotnet sln add src\Brimborium.Json.Tool


dotnet new tool-manifest
dotnet tool install --add-source ./microsoft.botsay/nupkg microsoft.botsay
dotnet tool run botsay hello from the bot


dotnet new tool-manifest

dotnet pack
dotnet tool install --add-source .\src\Brimborium.Json.Tool\nupkg\Brimborium.Json.Tool.1.0.0.nupkg Brimborium.Json -f net5.0
dotnet tool install --framework netcoreapp3.1 --add-source .\src\Brimborium.Json.Tool\nupkg\Brimborium.Json.Tool.1.0.0.nupkg Brimborium.Json 
dotnet tool install --add-source G:\github\grimmborium\Brimborium.Json\output\Brimborium.Json\bin\Debug\Brimborium.Json.1.0.0.nupkg Brimborium.Json

dotnet run --project  ".\src\Brimborium.Json.Tool\Brimborium.Json.Tool.csproj"

 
