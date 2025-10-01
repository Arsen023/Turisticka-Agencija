using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWebApp.Models;
using MyWebApp.Repositories;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly KorisnikRepository _korRepo = new KorisnikRepository();

        private Korisnik Curr()
        {
            var k = Session["Korisnik"] as Korisnik;
            if (k == null || k.uloga != Uloga.Administrator)
                throw new HttpException(401, "Samo administrator ima pristup.");
            return k;
        }

        public ActionResult Users(string q = "", string uloga = "")
        {
            Curr();

            var list = _korRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.Trim();
                list = list
                    .Where(x =>
                        (x.Ime ?? "").IndexOf(qq, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (x.Prezime ?? "").IndexOf(qq, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(uloga))
            {
                list = list
                    .Where(x => string.Equals(x.uloga.ToString(), uloga, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Q = q;
            ViewBag.Uloga = uloga;
            return View(list.OrderBy(x => x.Prezime).ThenBy(x => x.Ime).ToList());
        }

        public ActionResult CreateManager()
        {
            Curr();
            return View(new RegisterManagerViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateManager(RegisterManagerViewModel vm)
        {
            Curr();

         
            ModelState.Remove("DatumRodjenja");
            var rawBirth = Request.Form["DatumRodjenja"]; 

            if (!string.IsNullOrWhiteSpace(rawBirth) &&
                DateTime.TryParseExact(rawBirth, "dd/MM/yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out var dr))
            {
                vm.DatumRodjenja = dr;
            }
            else
            {
                ModelState.AddModelError("DatumRodjenja", "Unesite datum rođenja u formatu dd/MM/yyyy.");
            }

         
            if (!ModelState.IsValid)
                return View(vm);

           
            if (_korRepo.FindByUsername(vm.KorisnickoIme) != null)
            {
                ModelState.AddModelError("KorisnickoIme", "Korisničko ime je zauzeto.");
                return View(vm);
            }

            var novi = new Korisnik
            {
                KorisnickoIme = vm.KorisnickoIme?.Trim(),
                Lozinka = vm.Lozinka, 
                Ime = vm.Ime?.Trim(),
                Prezime = vm.Prezime?.Trim(),
                Email = vm.Email?.Trim(),
                Pol = vm.Pol?.Trim(),
                DatumRodjenja = vm.DatumRodjenja, 
                uloga = Uloga.Menadzer
            };

            _korRepo.Add(novi);

            TempData["OK"] = "Menadžer uspešno registrovan.";
            return RedirectToAction("Users");
        }
    }
}

