using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class MenadzerSmestajController : Controller
    {
        private readonly SmestajRepository smestajRepo = new SmestajRepository();
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly RezervacijaRepository rezervacijaRepo = new RezervacijaRepository();

        private Korisnik Curr()
        {
            var k = Session["Korisnik"] as Korisnik;
            if (k == null || k.uloga != Uloga.Menadzer)
                throw new HttpException(401, "Samo menadžer može ovde.");
            return k;
        }

        private HashSet<int> MyAranzmanIds(Korisnik u)
        {
            var set = new HashSet<int>();
            if (u.KreiraniAranzmani != null)
            {
             
                foreach (var id in u.KreiraniAranzmani)
                    set.Add(id);
            }
            return set;
        }

        private bool SmestajPripadaMeni(int smestajId, HashSet<int> myArIds)
        {
            var svi = aranzmanRepo.GetAll();
            foreach (var a in svi)
            {
                if (!myArIds.Contains(a.Id)) continue;
                if (a.SmestajId != null && a.SmestajId.Contains(smestajId))
                    return true;
            }
            return false;
        }

     
        public ActionResult Index(string q = "", bool? samoMoji = true, bool? prikaziObrisane = false, string sort = "")
        {
            var user = Curr();
            var myArIds = MyAranzmanIds(user);

            var list = smestajRepo.GetAll();
            if (!prikaziObrisane.GetValueOrDefault(false))
                list = list.Where(s => !s.IsDeleted).ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var qq = q.Trim();
                list = list.Where(s => (s.Naziv ?? "").IndexOf(qq, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (samoMoji.GetValueOrDefault(true))
            {
                list = list.Where(s => SmestajPripadaMeni(s.Id, myArIds)).ToList();
            }

            switch (sort)
            {
                case "nazivAsc": list = list.OrderBy(s => s.Naziv).ToList(); break;
                case "nazivDesc": list = list.OrderByDescending(s => s.Naziv).ToList(); break;
                default: list = list.OrderBy(s => s.Id).ToList(); break;
            }

            ViewBag.Q = q;
            ViewBag.SamoMoji = samoMoji;
            ViewBag.PrikaziObrisane = prikaziObrisane;
            ViewBag.Sort = sort;

            return View(list);
        }

      
        public ActionResult Create()
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            var mojiAr = aranzmanRepo.GetAll().Where(a => myIds.Contains(a.Id) && !a.IsDeleted).ToList();
            ViewBag.MojiAranzmani = new SelectList(mojiAr, "Id", "Naziv");
            return View(new Smestaj());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Smestaj form, int aranzmanId)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            if (!myIds.Contains(aranzmanId))
                return new HttpUnauthorizedResult("Smeštaj možeš dodati samo u svoje aranžmane.");

            var all = smestajRepo.GetAll();
            form.Id = all.Any() ? all.Max(s => s.Id) + 1 : 1;
            form.IsDeleted = false;

            smestajRepo.Add(form);

     
            var a = aranzmanRepo.FindById(aranzmanId);
            if (a.SmestajId == null) a.SmestajId = new List<int>();
            if (!a.SmestajId.Contains(form.Id)) a.SmestajId.Add(form.Id);
            aranzmanRepo.Update(a);

            TempData["SmestajOK"] = "Smeštaj je dodat.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            var s = smestajRepo.GetById(id);
            if (s == null) return HttpNotFound();
            if (!SmestajPripadaMeni(id, myIds))
                return new HttpUnauthorizedResult("Možeš uređivati samo smeštaj u okviru svojih aranžmana.");

            return View(s);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Smestaj form)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            if (!SmestajPripadaMeni(form.Id, myIds))
                return new HttpUnauthorizedResult("Možeš uređivati samo smeštaj u okviru svojih aranžmana.");

            var stari = smestajRepo.GetById(form.Id);
            if (stari == null) return HttpNotFound();

            form.IsDeleted = stari.IsDeleted;
            smestajRepo.Update(form);

            TempData["SmestajOK"] = "Smeštaj je ažuriran.";
            return RedirectToAction("Index");
        }

     
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var s = smestajRepo.GetById(id);
            if (s == null) return HttpNotFound();

            
            var jedinicaRepo = new SmestajnaJedinicaRepository();
            var idsJedinica = jedinicaRepo.GetAll()
                                          .Where(j => j.SmestajID == id && !j.IsDeleted)
                                          .Select(j => j.Id)
                                          .ToList();

          
            var rezervacije = rezervacijaRepo.GetAll()
                                             .Where(r => idsJedinica.Contains(r.SmestajnaJedinicaId))
                                             .ToList();

            
            var danas = DateTime.Today;
            var imaBuducuRez = rezervacije.Any(r =>
            {
                var ar = aranzmanRepo.FindById(r.AranzmanId);
                return ar != null && ar.DatumPocetkaPutovanja >= danas;
            });

            if (imaBuducuRez)
            {
                TempData["SmestajERR"] = "Brisanje nije dozvoljeno: postoji buduća rezervacija za ovaj smeštaj.";
                return RedirectToAction("Index");
            }

            
            aranzmanRepo.RemoveSmestajFromAll(id);
            smestajRepo.Remove(id);

            TempData["SmestajOK"] = "Smeštaj je obrisan.";
            return RedirectToAction("Index");
        }
    }
}

