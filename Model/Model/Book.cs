using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }


        [Required]
        public string Title { get; set; }


        [MaxLength(255)]
        public string Author { get; set; }

        public string Description { get; set; }


        [Required, Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }


        public int Stock { get; set; }


        public DateTime? PublicationDate { get; set; }
    }
}
