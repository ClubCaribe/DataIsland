using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using System.Web.Mvc;

namespace DataIsland.Models
{
    public class RegisterDataislandModel
    {
        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        public string DataIslandName { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public string WebPage { get; set; }
        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        public string Domain { get; set; }

        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        [Remote("doesUserNameExist", "home", ErrorMessage = "[tr]Username already exists.[/tr]", HttpMethod = "POST")]
        [MinLength(5,ErrorMessage="[tr]Username must be at least 5 characters long[/tr]")]
        public string Username { get; set; }
        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        public string AdministratorPassword { get; set; }

        [Required(ErrorMessage = "[tr]This field is required[/tr]")]
        [Email(ErrorMessage="[tr]Please provide valid email address[/tr]")]
        public string Email { get; set; }
    }
}