using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;

namespace MyWebApp.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly RezervacijaRepository rezervacijaRepo = new RezervacijaRepository();
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();
        private readonly SmestajnaJedinicaRepository smestajnaJedinicaRepo = new SmestajnaJedinicaRepository();

      
        public ActionResult Index(string searchId, string searchNaziv, string searchStatus, string sort)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");
            var aranzmani = aranzmanRepo.GetAll();
            var smestaji = smestajRepo.GetAll();
            var jedinice = smestajnaJedinicaRepo.GetAll(); 
            var mojeRezervacije = rezervacijaRepo.GetAll()
                .Where(r => r.TuristaKojiVrsiRezervaciju == user.KorisnickoIme)
                .ToList();

            ViewBag.Aranzmani = aranzmani;
            ViewBag.Smestaji = smestaji;
            ViewBag.SmestajneJedinice = jedinice;



            
            if (!string.IsNullOrEmpty(searchId) && int.TryParse(searchId, out int id))
            {
                mojeRezervacije = mojeRezervacije
                    .Where(r => r.Id == id)
                    .ToList();
            }

         
            if (!string.IsNullOrEmpty(searchNaziv))
            {
                mojeRezervacije = mojeRezervacije
                    .Where(r =>
                    {
                        var aranzman = aranzmanRepo.FindById(r.AranzmanId);
                        return aranzman != null &&
                               aranzman.Naziv.IndexOf(searchNaziv, StringComparison.OrdinalIgnoreCase) >= 0;
                    })
                    .ToList();
            }

            
            if (!string.IsNullOrEmpty(searchStatus))
            {
                if (Enum.TryParse(searchStatus, true, out Status statusFilter))
                {
                    mojeRezervacije = mojeRezervacije
                        .Where(r => r.status == statusFilter)
                        .ToList();
                }
            }

           
            switch (sort)
            {
                case "nazivAsc":
                    mojeRezervacije = mojeRezervacije
                        .OrderBy(r => aranzmanRepo.FindById(r.AranzmanId)?.Naziv)
                        .ToList();
                    break;
                case "nazivDesc":
                    mojeRezervacije = mojeRezervacije
                        .OrderByDescending(r => aranzmanRepo.FindById(r.AranzmanId)?.Naziv)
                        .ToList();
                    break;
            }

            return View(mojeRezervacije);
        }
        [HttpGet]
        

        
        public ActionResult Create(int aranzmanID, int smestajId, int jedinicaId)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");

            var aranzman = aranzmanRepo.FindById(aranzmanID);
            if (aranzman == null) return HttpNotFound();

            if (!aranzman.SmestajId.Contains(smestajId)) return HttpNotFound();

           
            var jedinica = smestajnaJedinicaRepo.GetAll().FirstOrDefault(j => j.Id == jedinicaId);
            if (jedinica == null || jedinica.Status != StatusSmestajneJedinice.Slobodna)
            {
                TempData["Error"] = "Jedinica nije slobodna.";
                return RedirectToAction("Details", "Aranzman", new { id = aranzmanID });
            }

          
            int noviId = rezervacijaRepo.GetAll().Any() ? rezervacijaRepo.GetAll().Max(r => r.Id) + 1 : 1;
            var rezervacija = new Rezervacija
            {
                Id = noviId,
                TuristaKojiVrsiRezervaciju = user.KorisnickoIme,
                status = Status.aktivna,
                AranzmanId = aranzman.Id,
                SmestajnaJedinicaId = jedinica.Id
            };

         
            jedinica.Status = StatusSmestajneJedinice.Zauzeta;
            smestajnaJedinicaRepo.Update(jedinica); 

            rezervacijaRepo.Add(rezervacija);         

            TempData["RezervacijaOK"] = "Rezervacija uspešno kreirana!";
           
            return RedirectToAction("Details", "Aranzman", new { id = aranzmanID });
        }

       
        public ActionResult Otkazi(int id)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");

            var rezervacija = rezervacijaRepo.FindById(id);
            if (rezervacija == null || rezervacija.TuristaKojiVrsiRezervaciju != user.KorisnickoIme)
                return HttpNotFound();

            var aranzman = aranzmanRepo.FindById(rezervacija.AranzmanId);
            if (aranzman == null) return HttpNotFound();

            if (aranzman.DatumZavrsetkaPutovanja <= DateTime.Now)
            {
                TempData["RezervacijaERR"] = "Ne možete otkazati rezervaciju jer je aranžman već prošao.";
                return RedirectToAction("Index");
            }

            rezervacija.status = Status.otkazana;
            rezervacijaRepo.Update(rezervacija);

           
            var jedinica = smestajnaJedinicaRepo.GetAll()
                            .FirstOrDefault(j => j.Id == rezervacija.SmestajnaJedinicaId);
            if (jedinica != null)
            {
                jedinica.Status = StatusSmestajneJedinice.Slobodna;
                smestajnaJedinicaRepo.Update(jedinica);
            }

            TempData["RezervacijaOK"] = "Rezervacija uspešno otkazana!";
         
            return RedirectToAction("Index");
        }

      
        public ActionResult Details(int id)
        {
            var user = (Korisnik)Session["Korisnik"];
            if (user == null || user.uloga != Uloga.Turista)
                return RedirectToAction("Login", "Korisnik");

            var rezervacija = rezervacijaRepo.FindById(id);
            if (rezervacija == null || rezervacija.TuristaKojiVrsiRezervaciju != user.KorisnickoIme)
                return HttpNotFound();

            var aranzman = aranzmanRepo.FindById(rezervacija.AranzmanId);
            if (aranzman == null) return HttpNotFound();

           
            var jedinica = smestajnaJedinicaRepo.FindById(rezervacija.SmestajnaJedinicaId);

       
            Smestaj smestaj = null;
            if (jedinica != null)
            {
                smestaj = smestajRepo.GetAll().FirstOrDefault(s => s.Id == jedinica.SmestajID);
            }

           
            ViewBag.Aranzman = aranzman;
            ViewBag.SmestajnaJedinica = jedinica; 
            ViewBag.SmestajNaziv = smestaj?.Naziv;
            ViewBag.DatumPocetka = aranzman.DatumPocetkaPutovanja.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            ViewBag.DatumZavrsetka = aranzman.DatumZavrsetkaPutovanja.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            
            ViewBag.MozeKomentar = aranzman.DatumZavrsetkaPutovanja <= DateTime.Now;

            return View(rezervacija);

        }
    }
}

