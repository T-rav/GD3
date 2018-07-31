#!/bin/sh

workingDir=".."

cd $workingDir

dotnet restore
dotnet build
dotnet publish -c Release -r win10-x64