dotnet build
dotnet pack
dotnet tool install --add-source ./nupkg justdoit --global | grep --invert-match warning --line-buffered
