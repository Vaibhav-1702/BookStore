using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public int UserId { get; set; }


        [Required, MaxLength(255)]
        public string AddressLine { get; set; }


        [Required, MaxLength(100)]
        public string City { get; set; }


        [Required, MaxLength(100)]
        public string State { get; set; }


        [Required, MaxLength(20)]
        public string PinCode { get; set; }


        [Required, MaxLength(100)]
        public string Country { get; set; }


        public User User { get; set; }
    }
}
