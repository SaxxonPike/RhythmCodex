#!/bin/bash

dotnet publish -c Release -f net48 -o Release/net48 Source/RhythmCodex.Cli/RhythmCodex.Cli.csproj
dotnet publish -c Release -f netcoreapp3.0 -o Release/netcoreapp3.0 Source/RhythmCodex.Cli/RhythmCodex.Cli.csproj
