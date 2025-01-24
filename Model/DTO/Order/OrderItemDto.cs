using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO.Order
{
    public class OrderItemDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } // Added
        public string BookAuthor { get; set; } // Added
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
