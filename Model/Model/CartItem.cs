using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.Model
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }


        public int CartId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        [JsonIgnore]
        public Cart Cart { get; set; }

        public Book Book { get; set; }
    }
}
