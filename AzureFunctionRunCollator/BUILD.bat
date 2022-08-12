dotnet publish -c Release -o Publish

cd Publish

REM NOTE that this does not work at the moment, need to manually zip using windows
REM tar.exe -a -c -f RunMiddenCliCopyOutputToBlob.zip *

REM NOTE: Run this manually unless you got the tar.exe command to work
REM NOTE: Replace "Midden" and "MiddenRunCollator" with your own Resource Group and Function App
REM az functionapp deployment source config-zip -g Midden -n MiddenRunCollator --src RunMiddenCliCopyOutputToBlob.zip