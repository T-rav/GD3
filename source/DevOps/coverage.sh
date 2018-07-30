#!/bin/sh

workingDir=".."

cd $workingDir

dotnet restore
dotnet build

rm -f coverage.xml
rm -f coverage.json
rm -rf coverage-html

cd DevOps

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir $workingDir --assemblies *.Tests/**/bin/**/*.dll  --sources **/*.cs

# Reset hits count in case minicover was run for this project
dotnet minicover reset

cd $workingDir

for project in *.Tests/*.csproj; do dotnet test --no-build $project; done

cd DevOps

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir $workingDir

# Create OpenCover report
dotnet minicover opencoverreport --workdir $workingDir --threshold 80

cd $workingDir
