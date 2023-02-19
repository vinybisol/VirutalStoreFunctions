namespace VirutalStoreFunctions.Models.Dtos
{
    public class ProductsRequestPostDto
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public double Price { get; set; }
        public double PriceMarket { get; set; }
        public string Note { get; set; }
        public string[] PhotoString { get; set; }
    }
}
