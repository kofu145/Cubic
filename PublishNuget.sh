#!/bin/bash

if [ $# -ne 2 ]
then
  echo "Please provide a package version and API key."
  exit 0
fi

echo "Packing all available nugets"
rm -r Packages
dotnet pack -p:PackageVersion="$1" -c Release . -o Packages/

read -p "Would you like to publish packages? [Y/n] "
if [[ $REPLY =~ ^[Yy]$ ]]
then
  dotnet nuget push Packages/Cubic.$1.nupkg --api-key $2 --source https://api.nuget.org/v3/index.json
  dotnet nuget push Packages/Cubic.Freetype.$1.nupkg --api-key $2 --source https://api.nuget.org/v3/index.json
  dotnet nuget push Packages/Cubic.Graphics.$1.nupkg --api-key $2 --source https://api.nuget.org/v3/index.json
  dotnet nuget push Packages/Cubic.Graphics.Platforms.OpenGL33.$1.nupkg --api-key $2 --source https://api.nuget.org/v3/index.json
fi 