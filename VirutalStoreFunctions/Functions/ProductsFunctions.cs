using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VirutalStoreFunctions.Models;
using VirutalStoreFunctions.Models.Dtos;

namespace VirutalStoreFunctions.Functions
{
    public class ProductsFunctions
    {
        private readonly ILogger<ProductsFunctions> _logger;

        public ProductsFunctions(ILogger<ProductsFunctions> log)
        {
            _logger = log;
        }

        [FunctionName("ProductGetAll")]
        [OpenApiOperation(operationId: "Add", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ProductGetAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product/getall")] HttpRequest req,
            [CosmosDB(
            ProductsModel.DatabaseName,
            ProductsModel.CollectionName,
            Connection = Literals.ConnectionStrings,
            CreateIfNotExists = true)] CosmosClient client)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Container container = client.GetContainer(ProductsModel.DatabaseName, ProductsModel.CollectionName);

            IOrderedQueryable<ProductsModel> docQueryable = container.GetItemLinqQueryable<ProductsModel>(true);
            var myResults = docQueryable.ToList().OrderBy(o => o.Name);

            return new OkObjectResult(myResults);
        }


        [FunctionName("ProductGetById")]
        [OpenApiOperation(operationId: "Add", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ProductGetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product/getbyid/{id}")] HttpRequest req,
            [CosmosDB(
            ProductsModel.DatabaseName,
            ProductsModel.CollectionName,
            Id = "{id}", PartitionKey = "{id}",
            Connection = Literals.ConnectionStrings,
            CreateIfNotExists = true)] ProductsModel product)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            if (product.Active is false)
                return new OkObjectResult(null);

            return new OkObjectResult(product);
        }

        [FunctionName("ProductAdd")]
        [OpenApiOperation(operationId: "Add", tags: new[] { "name" })]
        [OpenApiRequestBody("", typeof(ProductsRequestPostDto))]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ProductAddAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "product/add")] HttpRequest req,
        [Blob("photos", FileAccess.Write, Connection = Literals.AzureWebJobsStorage)] BlobContainerClient myBlobContainerClient,
        [CosmosDB(
            ProductsModel.DatabaseName,
            ProductsModel.CollectionName,
            PartitionKey = ProductsModel.PartitionKey,
            Connection = Literals.ConnectionStrings,
            CreateIfNotExists = true)] IAsyncCollector<ProductsModel> items
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            ProductsRequestPostDto request = JsonConvert.DeserializeObject<ProductsRequestPostDto>(body);
            ProductsModel productsModel = new(request);

            if (request.PhotoString.Any())
            {
                foreach (string photoString64 in request.PhotoString)
                {
                    var newId = Guid.NewGuid();
                    var blobName = $"{newId}.jpg";

                    await myBlobContainerClient.CreateIfNotExistsAsync();

                    var cloudBlockBlob = myBlobContainerClient.GetBlobClient(blobName);
                    var photoBytes = Convert.FromBase64String(photoString64);
                    using Stream stream = new MemoryStream(photoBytes);
                    await cloudBlockBlob.UploadAsync(stream);
                    productsModel.AddPhotoId(newId.ToString());
                }
            }

            await items.AddAsync(productsModel);

            return new OkObjectResult(new { productsModel });
        }

        //TODO Fazer o update
        public async Task<IActionResult> ProductUpdateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "product/update/{id}")] HttpRequest req,
            [Blob("photos", FileAccess.Write, Connection = Literals.AzureWebJobsStorage)] BlobContainerClient myBlobContainerClient,
            [CosmosDB(
            ProductsModel.DatabaseName,
            ProductsModel.CollectionName,
            PartitionKey = ProductsModel.PartitionKey,
            Connection = Literals.ConnectionStrings,
            CreateIfNotExists = true)] IAsyncCollector<ProductsModel> items
)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            ProductsRequestPostDto request = JsonConvert.DeserializeObject<ProductsRequestPostDto>(body);
            ProductsModel productsModel = new(request);

            if (request.PhotoString.Any())
            {
                foreach (string photoString64 in request.PhotoString)
                {
                    var newId = Guid.NewGuid();
                    var blobName = $"{newId}.jpg";

                    await myBlobContainerClient.CreateIfNotExistsAsync();

                    var cloudBlockBlob = myBlobContainerClient.GetBlobClient(blobName);
                    var photoBytes = Convert.FromBase64String(photoString64);
                    using Stream stream = new MemoryStream(photoBytes);
                    await cloudBlockBlob.UploadAsync(stream);
                    productsModel.AddPhotoId(newId.ToString());
                }
            }

            await items.AddAsync(productsModel);

            return new OkObjectResult(new { productsModel });
        }

        [FunctionName("ProductDeleteById")]
        [OpenApiOperation(operationId: "Add", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> ProductDeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "product/delete/{id}")] HttpRequest req,
            [CosmosDB(
            ProductsModel.DatabaseName,
            ProductsModel.CollectionName,
            Connection = Literals.ConnectionStrings,
            CreateIfNotExists = true)] CosmosClient client,
            string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Container container = client.GetContainer(ProductsModel.DatabaseName, ProductsModel.CollectionName);

            ResponseMessage response = await container.DeleteItemStreamAsync(id, new PartitionKey(id));

            if (!response.IsSuccessStatusCode)
                return new BadRequestObjectResult(new { message = "erro do deletar", id });

            return new OkResult();
        }
    }
}

