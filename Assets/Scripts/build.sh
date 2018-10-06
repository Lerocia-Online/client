#! /bin/sh

project="Multiplayer Example"

echo "Attempting to build $project for Linux"
/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -executeMethod BuildHelper.build

echo 'Logs from build'
cat $(pwd)/unity.log


echo 'Attempting to zip builds'
zip -r $(pwd)/Builds/server.zip $(pwd)/Builds/