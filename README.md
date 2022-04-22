#

## Requirements

-  Azure Functions Core Tools - https://go.microsoft.com/fwlink/?linkid=2174
-  .Net 6.0 - https://dotnet.microsoft.com/en-us/download/dotnet/6.0
-  Azure Functions Vistual Studio Extension - https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions


## Run Function Locally
```
func start
```

## Debug
```sh
F5
```

## Local Endpoint
http://localhost:7071/api/FileHandlerHttpTrigger

## Send File CURL
```sh
curl --location --request POST 'http://localhost:7071/api/FileHandlerHttpTrigger' \
--form 'file=@"/FileTest.xlsx"'
```

## Packages
dotnet add package NPOI --version 2.5.5
dotnet add package DotNetCore.NPOI --version 1.2.3