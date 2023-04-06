namespace VirutalStoreFunctions.Models.Dtos
{
    public class ProductsRequestPostDto
    {
        public string Name { get; }
        public string ShortName { get; }
        public double Price { get; }
        public double PriceMarket { get; }
        public string Note { get; }
        public bool Active { get; }
        public string[] PhotoString { get; }

        public ProductsRequestPostDto(string name, string shortName, double price, double priceMarket, string note, string[] photoString, bool active)
        {
            Name = name;
            ShortName = shortName;
            Price = price;
            PriceMarket = priceMarket;
            Note = note;
            PhotoString = photoString;
            Active = active;
        }
    }
}
