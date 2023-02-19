using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Net;
using System.Reflection.Metadata;

namespace VirutalStoreFunctions
{
    public class PhotoImageSearch
    {
        private readonly ILogger<PhotoImageSearch> _logger;

        public PhotoImageSearch(ILogger<PhotoImageSearch> log)
        {
            _logger = log;
        }

        [FunctionName("image")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult GetImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("photos/{Query.id}.jpg", FileAccess.Read, Connection = Literals.AzureWebJobsStorage)] byte[] myBlob)
        {
            string name = req.Query["id"];
            return new FileContentResult(myBlob, "image/jpeg")
            {
                FileDownloadName = $"{name}.jpg"
            };
        }
    }
}

