
namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public decimal Price { get; set; }
        public String PictureUrl { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public ProductBrand ProductBrand { get; set; }
        public int ProductBrandId { get; set; }
    }
}