using MyWebApp.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        public string KorisnickoIme { get; set; } = "";

        [MinLength(5, ErrorMessage = "Lozinka mora imati bar 5 karaktera")]
        public string Lozinka { get; set; } = "";

        [Required(ErrorMessage = "Ime je obavezno")]
        public string Ime { get; set; } = "";

        [Required(ErrorMessage = "Prezime je obavezno")]
        public string Prezime { get; set; } = "";

        [Required(ErrorMessage = "Pol je obavezan")]
        public string Pol { get; set; } = "";

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email nije validan")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Datum rođenja je obavezan")]
        [JsonConverter(typeof(MyWebApp.Service.CustomDateConverter))]
        public DateTime DatumRodjenja { get; set; }
        
        public Uloga uloga { get; set; }

        public List<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
        public List<int> KreiraniAranzmani { get; set; } = new List<int>();

        public EditUserViewModel()
        {
        }

        public EditUserViewModel(Korisnik korisnik)
        {
            KorisnickoIme = korisnik.KorisnickoIme;
            Lozinka = korisnik.Lozinka;
            Ime = korisnik.Ime;
            Prezime = korisnik.Prezime;
            Pol = korisnik.Pol;
            Email = korisnik.Email;
            DatumRodjenja = korisnik.DatumRodjenja;
            uloga = korisnik.uloga;
            Rezervacije = korisnik.Rezervacije;
            KreiraniAranzmani = korisnik.KreiraniAranzmani;
        }

        public Korisnik ToKorisnik()
        {
            return new Korisnik
            {
                KorisnickoIme = this.KorisnickoIme,
                Lozinka = this.Lozinka,
                Ime = this.Ime,
                Prezime = this.Prezime,
                Pol = this.Pol,
                Email = this.Email,
                DatumRodjenja = this.DatumRodjenja,
                uloga = this.uloga,
                Rezervacije = this.Rezervacije,
                KreiraniAranzmani = this.KreiraniAranzmani
            };
        }
    }
}
