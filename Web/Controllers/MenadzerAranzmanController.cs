using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class MenadzerAranzmanController : Controller
    {
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly RezervacijaRepository rezervacijaRepo = new RezervacijaRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();
        private readonly KorisnikRepository korisnikRepo = new KorisnikRepository();

        private Korisnik Curr()
        {
            var k = Session["Korisnik"] as Korisnik;
            if (k == null || k.uloga != Uloga.Menadzer)
                throw new HttpException(401, "Samo menadžer može ovde.");
            return k;
        }

        private HashSet<int> MyAranzmanIds(Korisnik u)
            => new HashSet<int>(u.KreiraniAranzmani ?? new List<int>());

        public ActionResult Index(string q = "", bool? samoMoji = true, string sort = "")
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var list = aranzmanRepo.GetAll()
                                   .Where(a => !a.IsDeleted)
                                   .ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.Trim();
                list = list.Where(a =>
                    (a.Naziv ?? "").IndexOf(qq, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (a.OpisAranzmana ?? "").IndexOf(qq, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }

            if (samoMoji.GetValueOrDefault(true))
                list = list.Where(a => myIds.Contains(a.Id)).ToList();

            switch (sort)
            {
                case "nazivAsc": list = list.OrderBy(a => a.Naziv).ToList(); break;
                case "nazivDesc": list = list.OrderByDescending(a => a.Naziv).ToList(); break;
                case "pocetakAsc": list = list.OrderBy(a => a.DatumPocetkaPutovanja).ToList(); break;
                case "pocetakDesc": list = list.OrderByDescending(a => a.DatumPocetkaPutovanja).ToList(); break;
                default: list = list.OrderBy(a => a.Id).ToList(); break;
            }

            ViewBag.Q = q;
            ViewBag.SamoMoji = samoMoji;
            ViewBag.Sort = sort;

            return View(list);
        }

        public ActionResult Create()
        {
            Curr();
            ViewBag.Smestaji = smestajRepo.GetAll().Where(s => !s.IsDeleted).ToList();
            return View(new Aranzman());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Aranzman form, int[] smestajiIds, HttpPostedFileBase PosterFile)
        {
            var user = Curr();

            ModelState.Remove("DatumPocetkaPutovanja");
            ModelState.Remove("DatumZavrsetkaPutovanja");

            DateTime dt;
            var rawStart = Request.Form["DatumPocetkaPutovanja"];
            var rawEnd = Request.Form["DatumZavrsetkaPutovanja"];

            if (!string.IsNullOrWhiteSpace(rawStart) &&
                DateTime.TryParseExact(rawStart, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                form.DatumPocetkaPutovanja = dt;

            if (!string.IsNullOrWhiteSpace(rawEnd) &&
                DateTime.TryParseExact(rawEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                form.DatumZavrsetkaPutovanja = dt;

            if (string.IsNullOrWhiteSpace(form.Naziv))
                ModelState.AddModelError("Naziv", "Naziv je obavezan.");
            if (string.IsNullOrWhiteSpace(Convert.ToString(form.tipAranzmana)))
                ModelState.AddModelError("tipAranzmana", "Izaberi tip aranžmana.");
            if (string.IsNullOrWhiteSpace(Convert.ToString(form.tipPrevoza)))
                ModelState.AddModelError("tipPrevoza", "Izaberi tip prevoza.");
            if (string.IsNullOrWhiteSpace(Convert.ToString(form.lokacija)))
                ModelState.AddModelError("lokacija", "Unesi lokaciju (grad, država, regija).");
            if (form.DatumPocetkaPutovanja == default(DateTime))
                ModelState.AddModelError("DatumPocetkaPutovanja", "Unesi datum početka.");
            if (form.DatumZavrsetkaPutovanja == default(DateTime))
                ModelState.AddModelError("DatumZavrsetkaPutovanja", "Unesi datum završetka.");
            if (form.DatumZavrsetkaPutovanja < form.DatumPocetkaPutovanja)
                ModelState.AddModelError("", "Datum završetka ne može biti pre početka.");
            if (form.MaxBrPutnika <= 0)
                ModelState.AddModelError("MaxBrPutnika", "Maksimalan broj putnika mora biti veći od 0.");
            if (string.IsNullOrWhiteSpace(form.OpisAranzmana))
                ModelState.AddModelError("OpisAranzmana", "Opis je obavezan.");
            if (string.IsNullOrWhiteSpace(form.ProgramPutovanja))
                ModelState.AddModelError("ProgramPutovanja", "Program putovanja je obavezan.");

            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(PosterFile.FileName) ?? "";
                if (!allowed.Contains(ext))
                    ModelState.AddModelError("PosterAranzmana", "Dozvoljeni formati: JPG, PNG, GIF, WEBP.");
                else if (PosterFile.ContentLength > 8 * 1024 * 1024)
                    ModelState.AddModelError("PosterAranzmana", "Maksimalna veličina slike je 8MB.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(form.PosterAranzmana))
                    ModelState.AddModelError("PosterAranzmana", "Unesi naziv postera ili dodaj sliku.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Smestaji = smestajRepo.GetAll().Where(s => !s.IsDeleted).ToList();
                return View(form);
            }

            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                var baseName = Path.GetFileNameWithoutExtension(PosterFile.FileName);
                var safeBase = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars()));
                var ext = Path.GetExtension(PosterFile.FileName);
                var unique = $"{safeBase}_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}{ext}";
                var relPath = "~/Content/Images/" + unique;
                var absPath = Server.MapPath(relPath);

                var dir = Path.GetDirectoryName(absPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                PosterFile.SaveAs(absPath);
                form.PosterAranzmana = unique;
            }

            var all = aranzmanRepo.GetAll();
            form.Id = all.Any() ? all.Max(a => a.Id) + 1 : 1;
            form.IsDeleted = false;
            form.SmestajId = (smestajiIds ?? new int[0]).ToList();

            aranzmanRepo.Add(form);

            var userObj = Curr();
            if (userObj.KreiraniAranzmani == null) userObj.KreiraniAranzmani = new List<int>();
            if (!userObj.KreiraniAranzmani.Contains(form.Id)) userObj.KreiraniAranzmani.Add(form.Id);

            Session["Korisnik"] = userObj;
            korisnikRepo.Update(userObj);

            TempData["AranzmanOK"] = "Aranžman je uspešno kreiran.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var a = aranzmanRepo.FindById(id);
            if (a == null) return HttpNotFound();
            if (!myIds.Contains(a.Id)) return new HttpUnauthorizedResult("Možeš uređivati samo svoje aranžmane.");

            ViewBag.Smestaji = smestajRepo.GetAll().Where(s => !s.IsDeleted).ToList();
            return View(a);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Aranzman form, HttpPostedFileBase PosterFile)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var a = aranzmanRepo.FindById(form.Id);
            if (a == null) return HttpNotFound();
            if (!myIds.Contains(a.Id)) return new HttpUnauthorizedResult("Možeš uređivati samo svoje aranžmane.");

            ModelState.Remove("DatumPocetkaPutovanja");
            ModelState.Remove("DatumZavrsetkaPutovanja");

            DateTime dt;
            var rawStart = Request.Form["DatumPocetkaPutovanja"];
            var rawEnd = Request.Form["DatumZavrsetkaPutovanja"];

            if (!string.IsNullOrWhiteSpace(rawStart) &&
                DateTime.TryParseExact(rawStart, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                form.DatumPocetkaPutovanja = dt;

            if (!string.IsNullOrWhiteSpace(rawEnd) &&
                DateTime.TryParseExact(rawEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                form.DatumZavrsetkaPutovanja = dt;

            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(PosterFile.FileName) ?? "";
                if (!allowed.Contains(ext))
                    ModelState.AddModelError("PosterAranzmana", "Dozvoljeni formati: JPG, PNG, GIF, WEBP.");
                else if (PosterFile.ContentLength > 8 * 1024 * 1024)
                    ModelState.AddModelError("PosterAranzmana", "Maksimalna veličina slike je 8MB.");
                else
                {
                    var baseName = Path.GetFileNameWithoutExtension(PosterFile.FileName);
                    var safeBase = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars()));
                    var unique = $"{safeBase}_{DateTime.UtcNow:yyyyMMdd_HHmmssfff}{ext}";
                    var relPath = "~/Content/Images/" + unique;
                    var absPath = Server.MapPath(relPath);

                    var dir = Path.GetDirectoryName(absPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                    PosterFile.SaveAs(absPath);
                    form.PosterAranzmana = unique;
                }
            }

            if (form.DatumPocetkaPutovanja == default(DateTime))
                ModelState.AddModelError("DatumPocetkaPutovanja", "Unesi datum početka.");
            if (form.DatumZavrsetkaPutovanja == default(DateTime))
                ModelState.AddModelError("DatumZavrsetkaPutovanja", "Unesi datum završetka.");
            if (form.DatumZavrsetkaPutovanja < form.DatumPocetkaPutovanja)
                ModelState.AddModelError("", "Datum završetka ne može biti pre početka.");

            if (!ModelState.IsValid)
            {
                ViewBag.Smestaji = smestajRepo.GetAll().Where(s => !s.IsDeleted).ToList();
                return View(form);
            }

            form.IsDeleted = a.IsDeleted;
            aranzmanRepo.Update(form);

            TempData["AranzmanOK"] = "Aranžman je ažuriran.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var a = aranzmanRepo.FindById(id);
            if (a == null) return HttpNotFound();
            if (!myIds.Contains(a.Id)) return new HttpUnauthorizedResult("Možeš brisati samo svoje aranžmane.");

            var imaRez = rezervacijaRepo.GetAll().Any(r => r.AranzmanId == id);
            if (imaRez)
            {
                TempData["AranzmanERR"] = "Brisanje nije dozvoljeno: postoji rezervacija za ovaj aranžman.";
                return RedirectToAction("Index");
            }

            aranzmanRepo.Remove(id);

            if (user.KreiraniAranzmani != null)
            {
                user.KreiraniAranzmani.RemoveAll(x => x == id);
                Session["Korisnik"] = user;
                korisnikRepo.Update(user);
            }

            TempData["AranzmanOK"] = "Aranžman je obrisan.";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Curr();
            var a = aranzmanRepo.FindById(id);
            if (a == null) return HttpNotFound();
            return View(a);
        }
    }
}

