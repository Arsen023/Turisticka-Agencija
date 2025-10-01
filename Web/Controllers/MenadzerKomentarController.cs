using MyWebApp.Models;
using MyWebApp.Repositories;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace MyWebApp.Controllers
{
    public class MenadzerKomentarController : Controller
    {
        private readonly KomentarRepository komentarRepo = new KomentarRepository();
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();

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

     
        private bool KomentarJeNaMojSmestaj(Komentar k, HashSet<int> myIds)
        {
            var s = smestajRepo.GetAll()
                .FirstOrDefault(x => string.Equals(x.Naziv, k.SmestajNaziv, System.StringComparison.OrdinalIgnoreCase));
            if (s == null) return false;

            return aranzmanRepo.GetAll()
                .Any(a => myIds.Contains(a.Id) && a.SmestajId != null && a.SmestajId.Contains(s.Id));
        }

        public ActionResult Index(bool? samoNeodobreni = false)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var list = komentarRepo.GetAll()
                .Where(k => KomentarJeNaMojSmestaj(k, myIds))
                .ToList();

            if (samoNeodobreni.GetValueOrDefault(false))
                list = list.Where(k => !k.Odobren).ToList();

            ViewBag.SamoNeodobreni = samoNeodobreni;
            return View(list.OrderByDescending(k => k.Id).ToList());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var k = komentarRepo.GetAll().FirstOrDefault(x => x.Id == id);
            if (k == null) return HttpNotFound();
            if (!KomentarJeNaMojSmestaj(k, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu.");

            k.Odobren = true;
            komentarRepo.Update(k);

            TempData["OK"] = "Komentar je odobren (vidljiv svima).";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var k = komentarRepo.GetAll().FirstOrDefault(x => x.Id == id);
            if (k == null) return HttpNotFound();
            if (!KomentarJeNaMojSmestaj(k, myIds))
                return new HttpUnauthorizedResult("Nemaš dozvolu.");

           
            k.Odobren = false;
            komentarRepo.Update(k);

            TempData["OK"] = "Komentar je odbijen (vidljiv samo tebi).";
            return RedirectToAction("Index");
        }
    }
}

