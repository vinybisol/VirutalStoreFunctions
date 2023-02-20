using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VirutalStoreFunctions.Models.Dtos;

namespace VirutalStoreFunctions.Models
{
    public class ProductsModel
    {
        public const string DatabaseName = "virtualstore";
        public const string CollectionName = "products";
        public const string PartitionKey = "/id";

        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("shortName")]
        public string ShortName { get; private set; }

        [JsonProperty("price")]
        public double Price { get; private set; }

        [JsonProperty("priceMarket")]
        public double PriceMarket { get; private set; }

        [JsonProperty("note")]
        public string Note { get; private set; }

        [JsonProperty("photosIds")]
        public IList<string> PhotosIds { get; private set; } = new List<string>();

        public ProductsModel()
        {

        }

        public ProductsModel(Guid id, string name, string shortName, double price, double priceMarket, string note, string[] photoString)
        {
            Id = id;
            Name = name;
            ShortName = shortName;
            Price = price;
            PriceMarket = priceMarket;
            Note = note;
            PhotosIds = photoString;
        }
        public ProductsModel(ProductsRequestPostDto productsRequestPost)
        {
            Id = Guid.NewGuid();
            Name = productsRequestPost.Name;
            ShortName = productsRequestPost.ShortName;
            Price = productsRequestPost.Price;
            PriceMarket = productsRequestPost.PriceMarket;
            Note = productsRequestPost.Note;
        }

        public void AddPhotoId(string photoId)
            => PhotosIds.Add(photoId);

    }
}
