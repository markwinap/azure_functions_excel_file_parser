using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.Data;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace POCAzureFunctions
{
    public static class FileHandlerHttpTrigger
    {
        [FunctionName("FileHandlerHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();
            ISheet sheet;


    
            try
            {
                //Read settings from appsettings.json
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = config["pocazurefunction_SERVICEBUS"];
                var queue = config["ServiceBus_QUEUE"];
                // Configure Azure Service Bus
                await using var client = new ServiceBusClient(connectionString);
                // Connect to queue
                ServiceBusSender sender = client.CreateSender(queue);

                var formdata = await req.ReadFormAsync();
                var file = req.Form.Files["file"];
                log.LogInformation("File - {0} - {0} - {0}", file.FileName, file.Length, file.ContentType);

                // Read the file content   
                var stream = file.OpenReadStream();

                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                log.LogInformation("Cells {0}", cellCount);

                //Add Table Headers
                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                    {
                        dtTable.Columns.Add(cell.ToString());
                    }
                }
                // Add Table Rows
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    // If row is null skip it
                    if (row == null) continue;
                    // If row cells are empty skip it
                    if (row.Cells.TrueForAll(c => c.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString());
                            }
                        }
                    }
                    if (rowList.Count > 0)
                        dtTable.Rows.Add(rowList.ToArray());
                    rowList.Clear();
                }
                var body = JsonConvert.SerializeObject(dtTable);
            
                // Create a message
                ServiceBusMessage message = new ServiceBusMessage(body);
                // Send Azure Service Bus Message
                await sender.SendMessageAsync(message);

                return new OkObjectResult(body);
            }
            catch (Exception ex)
            {
                log.LogInformation("Exception - {0}", ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }

        }

    }
}
