using MyWebApp.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{

    public enum Uloga { Administrator, Menadzer, Turista}
    public class Korisnik
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        public string KorisnickoIme { get; set; } = "";

        [Required(ErrorMessage = "Lozinka je obavezna")]
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

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime DatumRodjenja { get; set; }
        public Uloga uloga { get; set; }

        public List<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();


        public List<int> KreiraniAranzmani { get; set; } = new List<int>();


        public Korisnik()
        {
        }

        public Korisnik(string korisnickoIme, string lozinka, string ime, string prezime, string pol, string email, DateTime datumRodjenja, Uloga uloga, List<Rezervacija> rezervacije, List<int> kreiraniAranzmani)
        {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Email = email;
            DatumRodjenja = datumRodjenja;
            this.uloga = uloga;
            Rezervacije = rezervacije;
            KreiraniAranzmani = kreiraniAranzmani;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

