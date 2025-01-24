using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }


        public int BookId { get; set; }


        public byte[] ImageData { get; set; }


        public string ImageUrl { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public Book Book { get; set; }
    }
}
