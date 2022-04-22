# Azure Functions Excel File Parser .Net 6.0

## Requirements

-  Azure Functions Core Tools - https://go.microsoft.com/fwlink/?linkid=2174
-  .Net 6.0 - https://dotnet.microsoft.com/en-us/download/dotnet/6.0
-  Azure Functions Vistual Studio Extension - https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions


## Packages
dotnet add package NPOI --version 2.5.5
dotnet add package DotNetCore.NPOI --version 1.2.3
dotnet add package RabbitMQ.Client
dotnet add package MySql.Data
dotnet add package Azure.Messaging.ServiceBus
## Run Function Locally
```
func host start --port 4034
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




![Alt text](https://github.com/markwinap/azure_functions_excel_file_parser/raw/master/request_sample.png "Request Sample")

## Docker
### Mysql
```sh
docker run --name some-mysql -p 3306:3306 -v ./db:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=test -d mysql:latest
```
### Rabbit MQ
```sh
docker run -d -p 5672:5672 --hostname my-rabbit --name some-rabbit rabbitmq:latest
```