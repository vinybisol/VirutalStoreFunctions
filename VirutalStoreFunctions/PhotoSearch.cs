using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;
using VirutalStoreFunctions.Models;

namespace VirutalStoreFunctions
{
    public class PhotoSearch
    {
        private readonly ILogger<PhotoSearch> _logger;

        public PhotoSearch(ILogger<PhotoSearch> log)
        {
            _logger = log;
        }

        [FunctionName("PhotoSearch")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB("photos", "metadata", Id = "{Query.id}", PartitionKey = "{Query.id}", Connection = Literals.ConnectionStrings)] PhotoUploadModel photoModel)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");


            var searchTerm = req.Query["id"].ToString();
            var searchTermPartition = req.Query["partitionKey"].ToString();

            if (photoModel == null)
            {
                _logger.LogInformation($"ToDo item not found");
            }
            else
            {
                _logger.LogInformation($"Found ToDo item, Description={photoModel}");
            }




            return new OkObjectResult(photoModel);
        }
    }
}

