using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TeddySite.Models
{
    public class FeedbackEntry
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [Required(ErrorMessage ="Comment cannot be empty")]
        [MinLength(4,ErrorMessage ="Comment must be atleast 4 characters long!")]
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }
    }
}