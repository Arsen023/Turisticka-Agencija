using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using MyWebApp.Models;
using MyWebApp.Service;

namespace MyWebApp.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly KorisnikService korisnikService = new KorisnikService();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(string korisnickoIme, string lozinka)
        {
            var korisnik = korisnikService.Login(korisnickoIme, lozinka);
            if (korisnik != null)
            {
                Session["Korisnik"] = korisnik;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Pogrešno korisničko ime ili lozinka!";
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

       
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(Korisnik novi)
        {
        
            ModelState.Remove("DatumRodjenja");
            var rawDob = Request.Form["DatumRodjenja"];
            if (!string.IsNullOrWhiteSpace(rawDob))
            {
                if (DateTime.TryParseExact(rawDob, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                {
                    novi.DatumRodjenja = dob;
                }
                else
                {
                    ModelState.AddModelError("DatumRodjenja", "Unesite datum u formatu dd/MM/yyyy.");
                }
            }
            else
            {
           
                ModelState.AddModelError("DatumRodjenja", "Datum rođenja je obavezan.");
            }

            if (!ModelState.IsValid)
            {
                return View(novi);
            }

         
            bool ok = korisnikService.Register(novi);
            if (ok)
            {
                TempData["Success"] = "Uspešno ste se registrovali! Sada se možete prijaviti.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Korisničko ime već postoji!";
            return View(novi);
        }

        public ActionResult Logout()
        {
            Session["Korisnik"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Profil()
        {
            if (Session["Korisnik"] == null)
                return RedirectToAction("Login");

            var korisnik = (Korisnik)Session["Korisnik"];
            return View(korisnik);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profil(Korisnik izmenjen)
        {
         
            ModelState.Remove("Lozinka");

          
            ModelState.Remove("DatumRodjenja");
            var rawDob = Request.Form["DatumRodjenja"];
            if (!string.IsNullOrWhiteSpace(rawDob))
            {
                if (DateTime.TryParseExact(rawDob, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var dob))
                {
                    izmenjen.DatumRodjenja = dob;
                }
                else
                {
                    ModelState.AddModelError("DatumRodjenja", "Unesite datum u formatu dd/MM/yyyy.");
                }
            }
            else
            {
                ModelState.AddModelError("DatumRodjenja", "Datum rođenja je obavezan.");
            }

            if (!ModelState.IsValid)
                return View(izmenjen);

            
            var user = (Korisnik)Session["Korisnik"];
            if (user == null)
                return RedirectToAction("Login");

          
            user.KorisnickoIme = izmenjen.KorisnickoIme?.Trim();
            user.Ime = izmenjen.Ime?.Trim();
            user.Prezime = izmenjen.Prezime?.Trim();
            user.Email = izmenjen.Email?.Trim();
            user.Pol = izmenjen.Pol?.Trim();
            user.DatumRodjenja = izmenjen.DatumRodjenja;

        
            if (!string.IsNullOrWhiteSpace(izmenjen.Lozinka))
            {
              
                user.Lozinka = izmenjen.Lozinka;
            }

            korisnikService.Update(user);
            Session["Korisnik"] = user;

           
            TempData["Success"] = "✅ Uspešno ste ažurirali profil!";
            return RedirectToAction("Profil");
        }
    }
}

