using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Service
{
    public class KorisnikService
    {
        private readonly KorisnikRepository korisnikRepo = new KorisnikRepository();

        public Korisnik Login(string username, string password)
        {
            var korisnik = korisnikRepo.FindByUsername(username);
            if (korisnik != null && korisnik.Lozinka == password)
                return korisnik;
            return null;
        }

        public bool Register(Korisnik novi)
        {
            var existing = korisnikRepo.FindByUsername(novi.KorisnickoIme);
            if (existing != null) return false; 


            novi.uloga = Uloga.Turista;
            var korisnici = korisnikRepo.GetAll();
            korisnici.Add(novi);
            korisnikRepo.SaveAll(korisnici);
            return true;
        }

        public void UpdateProfile(Korisnik updated)
        {
            korisnikRepo.Update(updated);
        }

        public List<Korisnik> GetAll()
        {
            return korisnikRepo.GetAll();
        }
        public void Update(Korisnik korisnik)
        {
            var repo = new KorisnikRepository();
            repo.Update(korisnik);
        }

    }
}

