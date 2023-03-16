using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IotSupplyStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [StringLength(100)]
        public string P_Code { get; set; }
        [StringLength(50)]
        public string P_Status { get; set; }
        public int P_Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        public Suppliers Suppliers { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
      
        public ICollection<ProductOrder> ProductOrders { get; set; }
        public DetailProduct DetailProduct { get; set; }
        public ICollection<DetailProduct> DetailProductId { get; set; }
    }
}
