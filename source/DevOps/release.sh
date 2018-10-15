#!/bin/sh

#move up to start the process
cd ..

netcoreVersion="2.1"
version=$(cat version.txt | cut -d. -f1,2,3)
build=$(cat version.txt | cut -d. -f4)
build=$(($build+1))
dist="../dist"
appdir="Analyzer"

echo -n "$version.$build" > version.txt

export Version=$(cat version.txt)
echo -e "Setting version to \e[32m$Version\e[0m"
echo -e "\e[33mTargeting dotnetcore version \e[35m$netcoreVersion\e[0m"
dotnet restore
dotnet clean
dotnet build

echo -e "\e[35mRunning test for solution...\e[0m"
for project in *.Tests/*.csproj; do 
	echo -e "\e[33Executing Tests for $project\e[0m"
	dotnet test --no-build $project  
	
	if [ $? -ne 0 ] 
	then
		echo -e "\e[31mTest Failure, Aborting!!!\e[0m"
		#git reset --hard
		exit
	fi
done

echo -e "\e[33mCreating NuGet Package\e[0m"
if [ ! -d "$dist" ]; then
  mkdir $dist
fi

cd $appdir
dotnet build -c release
mv "Analyzer/bin/Release/GD3-Analyzer.$version.nupkg" $dist

# commit build number bump to source
cd ..
git add version.txt
git commit -m "devOps(release):version bump to [$version.$build] from release script"
git push
