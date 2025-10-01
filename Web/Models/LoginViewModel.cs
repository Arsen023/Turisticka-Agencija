using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        public string KorisnickoIme { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }
    }
}

