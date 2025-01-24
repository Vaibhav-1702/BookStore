using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;


        [MaxLength(50)]
        public string Status { get; set; } = "Pending";


        public decimal TotalAmount { get; set; }

        public int AddressId { get; set; }

        // Navigation property for related OrderItems
        public ICollection<OrderItem> OrderItems { get; set; }


        public User? User { get; set; }

        public Address Address { get; set; }
    }
}
