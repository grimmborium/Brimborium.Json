# Brimborium.Json
Featured and fast JSON De-Serialization
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


