using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class WishlistItem
    {
        [Key]
        public int WishlistItemId { get; set; }

        public int WishlistId { get; set; }

        public int BookId { get; set; }

       // public int Quantity { get; set; }

        public Wishlist Wishlist { get; set; }

        public Book Book { get; set; }
    }
}
