using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace DataIsland.Models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        public string Username { get; set; }

        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string RedirectUrl { get; set; }
    }
}