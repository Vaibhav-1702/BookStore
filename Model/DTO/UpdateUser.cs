using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class UpdateUser
    {
        [Required]
        public string? name { get; set; }

        [Required]
        public string? email { get; set; }
    }
}
