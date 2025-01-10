using System.ComponentModel.DataAnnotations;

namespace eCommerce.SharedLibrary.DTOs.Responses
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public ProductDTO() { }

        public ProductDTO(int id, string name, int quantity, decimal price)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Price = price;
        }
    }

}
