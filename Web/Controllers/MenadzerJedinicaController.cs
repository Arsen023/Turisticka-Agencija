using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class MenadzerJedinicaController : Controller
    {
        private readonly SmestajnaJedinicaRepository jedinicaRepo = new SmestajnaJedinicaRepository();
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
                foreach (var id in u.KreiraniAranzmani) set.Add(id); 
            return set;
        }

        private bool SmestajJeMoj(int smestajId, HashSet<int> myArIds)
        {
            foreach (var a in aranzmanRepo.GetAll())
            {
                if (!myArIds.Contains(a.Id)) continue;
                if (a.SmestajId != null && a.SmestajId.Contains(smestajId))
                    return true;
            }
            return false;
        }

     
        public ActionResult Index(int smestajId)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            if (!SmestajJeMoj(smestajId, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovaj smeštaj.");

            var s = smestajRepo.GetById(smestajId);
            if (s == null) return HttpNotFound();

            var list = jedinicaRepo.GetAll()
                                   .Where(j => !j.IsDeleted && j.SmestajID == smestajId)
                                   .OrderBy(j => j.Id)
                                   .ToList();

            ViewBag.Smestaj = s;
            return View(list);
        }

      
        public ActionResult Create(int smestajId)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            if (!SmestajJeMoj(smestajId, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovaj smeštaj.");

            var s = smestajRepo.GetById(smestajId);
            if (s == null) return HttpNotFound();

            ViewBag.Smestaj = s;

            return View(new SmestajnaJedinica
            {
                SmestajID = smestajId,
                Status = StatusSmestajneJedinice.Slobodna,
                DozvoljeniKucniLjubimci = false,
                CenaZaCeluSmestajnuJedinicu = 0
            });
        }

     
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(SmestajnaJedinica form)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);
            if (!SmestajJeMoj(form.SmestajID, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovaj smeštaj.");

            var all = jedinicaRepo.GetAll();
            form.Id = all.Any() ? all.Max(j => j.Id) + 1 : 1;
            form.IsDeleted = false;

            jedinicaRepo.Add(form);

         
            var s = smestajRepo.GetById(form.SmestajID);
            if (s == null) return HttpNotFound();
            if (s.SmestajneJedinice == null) s.SmestajneJedinice = new List<int>();
            if (!s.SmestajneJedinice.Contains(form.Id)) s.SmestajneJedinice.Add(form.Id);
            smestajRepo.Update(s);

            TempData["OK"] = "Jedinica je dodata.";
            return RedirectToAction("Index", new { smestajId = form.SmestajID });
        }

      
        public ActionResult Edit(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var j = jedinicaRepo.FindById(id);
            if (j == null) return HttpNotFound();
            if (!SmestajJeMoj(j.SmestajID, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovu jedinicu.");

            ViewBag.Smestaj = smestajRepo.GetById(j.SmestajID);
            return View(j);
        }

       
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(SmestajnaJedinica form)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var j = jedinicaRepo.FindById(form.Id);
            if (j == null) return HttpNotFound();
            if (!SmestajJeMoj(j.SmestajID, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovu jedinicu.");

         
            bool blok = rezervacijaRepo.GetAll().Any(r =>
                r.SmestajnaJedinicaId == j.Id &&
                r.status == Status.aktivna &&
                aranzmanRepo.FindById(r.AranzmanId) != null &&
                aranzmanRepo.FindById(r.AranzmanId).DatumPocetkaPutovanja > DateTime.Now
            );

            if (blok && form.DozvoljenBrojGostiju != j.DozvoljenBrojGostiju)
            {
                TempData["ERR"] = "Nije dozvoljena izmena broja gostiju: postoji aktivna rezervacija u budućem aranžmanu.";
                return RedirectToAction("Edit", new { id = j.Id });
            }

        
            form.SmestajID = j.SmestajID;
            form.IsDeleted = j.IsDeleted;

            jedinicaRepo.Update(form);

            TempData["OK"] = "Jedinica je ažurirana.";
            return RedirectToAction("Index", new { smestajId = j.SmestajID });
        }

     
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var j = jedinicaRepo.FindById(id);
            if (j == null) return HttpNotFound();
            if (!SmestajJeMoj(j.SmestajID, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu za ovu jedinicu.");

           
            bool blok = rezervacijaRepo.GetAll().Any(r =>
                r.SmestajnaJedinicaId == j.Id &&
                r.status == Status.aktivna &&
                aranzmanRepo.FindById(r.AranzmanId) != null &&
                aranzmanRepo.FindById(r.AranzmanId).DatumPocetkaPutovanja > DateTime.Now
            );

            if (blok)
            {
                TempData["ERR"] = "Brisanje nije dozvoljeno: aktivna rezervacija u budućem aranžmanu.";
                return RedirectToAction("Index", new { smestajId = j.SmestajID });
            }

            j.IsDeleted = true;
            jedinicaRepo.Update(j);

        
            var s = smestajRepo.GetById(j.SmestajID);
            if (s != null && s.SmestajneJedinice != null && s.SmestajneJedinice.Contains(j.Id))
            {
                s.SmestajneJedinice.Remove(j.Id);
                smestajRepo.Update(s);
            }

            TempData["OK"] = "Jedinica je obrisana (logički).";
            return RedirectToAction("Index", new { smestajId = j.SmestajID });
        }
    }
}

