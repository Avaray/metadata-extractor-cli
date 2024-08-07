#!/bin/bash

dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true
