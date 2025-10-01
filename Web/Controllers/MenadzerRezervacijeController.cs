using MyWebApp.Models;
using MyWebApp.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyWebApp.Controllers
{
    public class MenadzerRezervacijeController : Controller
    {
        private readonly RezervacijaRepository rezervacijaRepo = new RezervacijaRepository();
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();
        private readonly SmestajnaJedinicaRepository jedinicaRepo = new SmestajnaJedinicaRepository();

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
                foreach (var id in u.KreiraniAranzmani)
                    set.Add(id);
            return set;
        }

        public ActionResult Index()
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var list = rezervacijaRepo.GetAll()
                .Where(r => myIds.Contains(r.AranzmanId))
                .OrderByDescending(r => r.Id)
                .ToList();

            return View(list);
        }

        public ActionResult Details(int id)
        {
            var user = Curr();
            var myIds = MyAranzmanIds(user);

            var r = rezervacijaRepo.FindById(id);
            if (r == null) return HttpNotFound();
            if (!myIds.Contains(r.AranzmanId)) return new HttpUnauthorizedResult("Nemaš dozvolu.");

            var a = aranzmanRepo.FindById(r.AranzmanId);

           
            var s = smestajRepo.GetAll()
                .FirstOrDefault(sm => (sm.SmestajneJedinicee ?? new System.Collections.Generic.List<SmestajnaJedinica>())
                    .Any(jj => jj.Id == r.SmestajnaJedinicaId));

           
            if (s == null && a != null && a.SmestajId != null)
                s = smestajRepo.GetAll().FirstOrDefault(sm => a.SmestajId.Contains(sm.Id));

            SmestajnaJedinica jedinica = null;
            if (s != null)
                jedinica = (s.SmestajneJedinicee ?? new System.Collections.Generic.List<SmestajnaJedinica>())
                    .FirstOrDefault(x => x.Id == r.SmestajnaJedinicaId);

            ViewBag.Aranzman = a;
            ViewBag.Smestaj = s;
            ViewBag.Jedinica = jedinica;

            return View(r);
        }
    }
}

