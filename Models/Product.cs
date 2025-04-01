namespace ASP.NET_OLX.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public Seller Seller { get; set; }
    }

    public class Seller
    {
        public string Name { get; set; }
        public string Contact { get; set; }
    }
}