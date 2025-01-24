using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class AddressDto
    {
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
    }
}
