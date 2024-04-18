using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMSProjectCode.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class BookInformationViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
    }



}