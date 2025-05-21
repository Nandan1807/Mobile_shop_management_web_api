using Microsoft.AspNetCore.Http;

namespace mobile_shop_web_api.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public string ProductName { get; set; }
        public string? ProductImage { get; set; } // Store the image path
        public decimal ProductPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? ProductDescription { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int CategoryId { get; set; }
        public int ProductBrandId { get; set; }
        
        // New property for image upload
        public IFormFile? ImageFile { get; set; }
    }
}