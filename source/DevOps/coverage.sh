#!/bin/sh

echo `pwd`

workingDir=".."

cd $workingDir

dotnet clean
dotnet msbuild GD3.sln

rm -f coverage.xml
rm -f coverage.json
rm -rf coverage-html

cd DevOps

# Instrument assemblies inside 'test' folder to detect hits for source files inside 'src' folder
dotnet minicover instrument --workdir $workingDir --assemblies *.Tests/**/bin/**/*.dll  --sources **/*.cs  --exclude-sources **/Migrations/*.cs

# Reset hits count in case minicover was run for this project
dotnet minicover reset

cd $workingDir

for project in *.Tests/*.csproj; do 
	if [ $# -eq 1 ]; then
		dotnet test --no-build --logger trx --results-directory $1 $project  
	else
		dotnet test --no-build $project  
	fi
done

cd DevOps

# Uninstrument assemblies, it's important if you're going to publish or deploy build outputs
dotnet minicover uninstrument --workdir $workingDir

# Create OpenCover report
dotnet minicover opencoverreport --workdir $workingDir --threshold 80

cd $workingDir
