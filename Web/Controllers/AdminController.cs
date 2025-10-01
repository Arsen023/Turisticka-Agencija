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

         
            var rawBirth = Request.Form["DatumRodjenja"]; 

            if (!string.IsNullOrWhiteSpace(rawBirth) &&
                DateTime.TryParseExact(rawBirth, "dd/MM/yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out var dr))
            {
                vm.DatumRodjenja = dr;
                ModelState.Remove("DatumRodjenja");
            }
            else
            {
                ModelState.AddModelError("DatumRodjenja", "Unesite datum rodjenja u formatu dd/MM/yyyy.");
            }

         
            if (!ModelState.IsValid)
                return View(vm);

           
            if (_korRepo.FindByUsername(vm.KorisnickoIme) != null)
            {
                ModelState.AddModelError("KorisnickoIme", "Korisnicko ime je zauzeto.");
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

            TempData["Success"] = "Menadzer uspesno registrovan.";
            return RedirectToAction("Users");
        }

        public ActionResult Details(string id)
        {
            Curr();
            var korisnik = _korRepo.FindByUsername(id);
            if (korisnik == null)
            {
                TempData["Error"] = "Korisnik nije pronadjen.";
                return RedirectToAction("Users");
            }
            return View(korisnik);
        }

        public ActionResult Edit(string id)
        {
            Curr();
            var korisnik = _korRepo.FindByUsername(id);
            if (korisnik == null)
            {
                TempData["Error"] = "Korisnik nije pronadjen.";
                return RedirectToAction("Users");
            }
            var editViewModel = new EditUserViewModel(korisnik);
            return View(editViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel editViewModel)
        {
            Curr();
            
            // Ako je lozinka prazna, zadrži postojeću lozinku
            if (string.IsNullOrWhiteSpace(editViewModel.Lozinka))
            {
                var postojeciKorisnik = _korRepo.FindByUsername(editViewModel.KorisnickoIme);
                if (postojeciKorisnik != null)
                {
                    editViewModel.Lozinka = postojeciKorisnik.Lozinka;
                }
            }
            
            if (ModelState.IsValid)
            {
                var korisnik = editViewModel.ToKorisnik();
                _korRepo.Update(korisnik);
                TempData["Success"] = "Korisnik uspesno azuriran.";
                return RedirectToAction("Users");
            }
            return View(editViewModel);
        }

        public ActionResult Delete(string id)
        {
            Curr();
            var korisnik = _korRepo.FindByUsername(id);
            if (korisnik == null)
            {
                TempData["Error"] = "Korisnik nije pronadjen.";
                return RedirectToAction("Users");
            }
            if (korisnik.uloga == Uloga.Administrator)
            {
                TempData["Error"] = "Ne mozete obrisati administratora.";
                return RedirectToAction("Users");
            }
            return View(korisnik);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            Curr();
            var korisnik = _korRepo.FindByUsername(id);
            if (korisnik != null)
            {
                if (korisnik.uloga == Uloga.Administrator)
                {
                    TempData["Error"] = "Ne mozete obrisati administratora.";
                    return RedirectToAction("Users");
                }
                var korisnici = _korRepo.GetAll();
                korisnici.RemoveAll(k => k.KorisnickoIme == id);
                _korRepo.SaveAll(korisnici);
                TempData["Success"] = "Korisnik uspesno obrisan.";
            }
            return RedirectToAction("Users");
        }
    }
}

