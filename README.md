## Full Text Search

```bash
dotnet build src/FullTextSearch/FullTextSearch.csproj

time dotnet run --project src/FullTextSearch/FullTextSearch.csproj -- --insert
time dotnet run --project src/FullTextSearch/FullTextSearch.csproj -- --like
time dotnet run --project src/FullTextSearch/FullTextSearch.csproj -- --contains
time dotnet run --project src/FullTextSearch/FullTextSearch.csproj -- --freetext
```