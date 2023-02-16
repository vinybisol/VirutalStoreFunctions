using System;
using Newtonsoft.Json;

namespace VirutalStoreFunctions.Models
{
    public class PhotoUploadModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("partitionKey")]
        public Guid PartitionKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }
    }
}
