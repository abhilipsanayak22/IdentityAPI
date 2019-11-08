using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Fullname can not be empty")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Username can not be empty")]
        [MinLength(6, ErrorMessage ="Minimum 6 characters required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Username can not be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email can not be empty")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Role { get; set; }

        public string Status { get; set; }
    }
}
