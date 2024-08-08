#!/bin/bash

# This file is used to manually build executables for all platforms.
# For GitHub Actions you can specify the platforms in jobs.build.strategy.matrix

dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r linux-x86 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r osx-x64 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r osx-x86 -p:PublishSingleFile=true -p:SelfContained=true
