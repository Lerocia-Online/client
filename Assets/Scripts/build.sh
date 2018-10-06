#! /bin/sh

project="Multiplayer Example"

echo "Attempting to build $project for Linux"
/Applications/Unity/Unity.app/Contents/MacOS/Unity 
  -quit 
  -batchmode 
  -nographics 
  -executeMethod BuildHelper.build

echo 'Logs from build'
cat $(pwd)/unity.log


echo 'Attempting to zip builds'
zip -r $(pwd)/Build/linux.zip $(pwd)/Build/linux/
zip -r $(pwd)/Build/mac.zip $(pwd)/Build/osx/
zip -r $(pwd)/Build/windows.zip $(pwd)/Build/windows/