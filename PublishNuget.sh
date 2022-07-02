#!/bin/bash

if [ $# -ne 1 ]
then
  echo "Please provide a package version."
  exit 0
fi

echo "Packing all available nugets"
rm -r Packages
dotnet pack -p:PackageVersion="$1" -c Release . -o Packages/