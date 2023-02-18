using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using VirutalStoreFunctions.Models;

namespace VirutalStoreFunctions
{
    public class PhotoStorage
    {
        private readonly ILogger<PhotoStorage> _logger;

        public PhotoStorage(ILogger<PhotoStorage> log)
        {
            _logger = log;
        }

        [FunctionName("photo")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiRequestBody("", typeof(PhotoStorage))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("photos", FileAccess.Write, Connection = Literals.StorageConnectionString)] BlobContainerClient myBlobContainerClient,
            [CosmosDB("photos", "metadata", PartitionKey = "/id", Connection = Literals.CosmosDBConnection, CreateIfNotExists = true)] IAsyncCollector<PhotoUploadModel> items
            )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<PhotoUploadModel>(body);

            var newId = Guid.NewGuid();
            var blobName = $"{newId}.jpg";

            await myBlobContainerClient.CreateIfNotExistsAsync();

            var cloudBlockBlob = myBlobContainerClient.GetBlobClient(blobName);
            var photoBytes = Convert.FromBase64String(request.Photo);
            using Stream stream = new MemoryStream(photoBytes);
            await cloudBlockBlob.UploadAsync(stream);

            var itens = new PhotoUploadModel()
            {
                Id = newId,
                PartitionKey = newId,
                Name = request.Name,
                Description = request.Description,
                Tags = request.Tags,
            };

            await items.AddAsync(itens);

            return new OkObjectResult(newId);
        }
    }
}

