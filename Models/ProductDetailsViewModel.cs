namespace ASP.NET_OLX.Models
{
    public class ProductDetailsViewModel
        {
            public Product Product { get; set; }
            public SellerModel Seller { get; set; }
            public List<Product> RelatedAds { get; set; }

            public string RazorpayKey { get; set; }

    }

    public class SellerModel
        {
            public string Name { get; set; }
            public string Contact { get; set; }
            public double Rating { get; set; }
        }
    }

