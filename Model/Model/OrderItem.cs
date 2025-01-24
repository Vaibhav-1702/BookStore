using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }


        [Column(TypeName = "decimal(10, 2)")]
        public decimal PurchasePrice { get; set; }

        public Order Order { get; set; }

        public Book Book { get; set; }
    }
}
