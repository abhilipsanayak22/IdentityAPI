using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAPI.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage = "Username can not be empty")]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string username { get; set; }

        [Required(ErrorMessage = "Username can not be empty")]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required")]
        public string password { get; set; }
    }
}
