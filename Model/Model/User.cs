using Model.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }


        [Required]
        public string Password { get; set; }


        [Required]
        public Role role { get; set; } = Role.USER;

    }
}
