using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace azure_functions_csharp_blob_out_binding
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Blob("outputdemo/{sys.utcnow}.txt", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream outputFile,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            UnicodeEncoding uniencoding = new UnicodeEncoding();
            string messageToWriteToFile = "Message from " + name;
            byte[] output = uniencoding.GetBytes(messageToWriteToFile);

            await outputFile.WriteAsync(output, 0, output.Length);

            return new OkObjectResult(responseMessage);
        }
    }
}
