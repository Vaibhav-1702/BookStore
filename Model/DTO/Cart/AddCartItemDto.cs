using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO.Cart
{
    public class AddCartItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
