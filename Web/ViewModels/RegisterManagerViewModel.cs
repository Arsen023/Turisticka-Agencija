using System;
using System.ComponentModel.DataAnnotations;
using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    
    public class RegisterManagerViewModel
    {
        [Required, Display(Name = "Korisničko ime")]
        public string KorisnickoIme { get; set; }

        [Required, MinLength(5), DataType(DataType.Password), Display(Name = "Lozinka")]
        public string Lozinka { get; set; }

        [Required, Display(Name = "Ime")]
        public string Ime { get; set; }

        [Required, Display(Name = "Prezime")]
        public string Prezime { get; set; }

        [Required, Display(Name = "Pol")]
        public string Pol  { get; set; }  

        [EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Datum rođenja")]
        public DateTime DatumRodjenja { get; set; }
    }
}

