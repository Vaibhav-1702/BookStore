using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }


        public int BookId { get; set; }

        public int UserId { get; set; }


        [Range(1, 5)]
        public int Rating { get; set; }


        public string Comment { get; set; }


        public DateTime ReviewDate { get; set; } = DateTime.Now;


        public Book Book { get; set; }

        public User User { get; set; }
    }
}
