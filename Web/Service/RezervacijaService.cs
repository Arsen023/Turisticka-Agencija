using MyWebApp.Models;
using MyWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWebApp.Service
{
    public class RezervacijaService
    {
        private readonly RezervacijaRepository rezervacijaRepo = new RezervacijaRepository();
        private readonly AranzmanRepository aranzmanRepo = new AranzmanRepository();
        private readonly SmestajRepository smestajRepo = new SmestajRepository();

        public List<Rezervacija> GetAll()
        {
            return rezervacijaRepo.GetAll();
        }

        public Rezervacija FindById(int id)
        {
            return rezervacijaRepo.FindById(id);
        }

        public List<Rezervacija> GetByTurista(string korisnickoIme)
        {
            return GetAll().Where(r => r.TuristaKojiVrsiRezervaciju == korisnickoIme).ToList();
        }

        public bool KreirajRezervaciju(string turista, Aranzman aranzman, int jedinicaId)
        {
            var smestaj = smestajRepo.GetById(aranzman.SmestajId.FirstOrDefault());
            if (smestaj == null) return false;

            var jedinica = smestaj.SmestajneJedinicee.FirstOrDefault(j => j.Id == jedinicaId);
            if (jedinica == null || jedinica.Status != StatusSmestajneJedinice.Slobodna)
                return false;

            int noviId = rezervacijaRepo.GetAll().Any() ? rezervacijaRepo.GetAll().Max(r => r.Id) + 1 : 1;

            var rez = new Rezervacija
            {
                Id = noviId,
                TuristaKojiVrsiRezervaciju = turista,
                status = Status.aktivna,
                AranzmanId = aranzman.Id,
                SmestajnaJedinicaId = jedinica.Id
            };

            jedinica.Status = StatusSmestajneJedinice.Zauzeta;

            rezervacijaRepo.Add(rez);
            smestajRepo.Update(smestaj);

            return true;
        }

        public bool OtkaziRezervaciju(int rezervacijaId)
        {
            var rezervacija = rezervacijaRepo.FindById(rezervacijaId);
            if (rezervacija == null) return false;

            var aranzman = aranzmanRepo.FindById(rezervacija.AranzmanId);
            if (aranzman == null) return false;

          
            if (DateTime.Now > aranzman.DatumZavrsetkaPutovanja) return false;

            
            rezervacija.status = Status.otkazana;

         
            foreach (var smestajId in aranzman.SmestajId)
            {
                var smestaj = smestajRepo.GetById(smestajId);
                if (smestaj != null)
                {
                    var jedinica = smestaj.SmestajneJedinicee
                                      .FirstOrDefault(j => j.Id == rezervacija.SmestajnaJedinicaId);
                    if (jedinica != null)
                        jedinica.Status = StatusSmestajneJedinice.Slobodna;

                    smestajRepo.Update(smestaj);
                }
            }

            rezervacijaRepo.Update(rezervacija);
            return true;
        }


        public List<Rezervacija> Pretrazi(string korisnickoIme, string id = "", string nazivAranzmana = "", string status = "")
        {
            var query = GetByTurista(korisnickoIme).AsEnumerable();

            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int rezId))
                query = query.Where(r => r.Id == rezId);

            if (!string.IsNullOrEmpty(nazivAranzmana))
            {
                query = query.Where(r =>
                {
                    var aranzman = aranzmanRepo.FindById(r.AranzmanId);
                    return aranzman != null &&
                           aranzman.Naziv.IndexOf(nazivAranzmana, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        public List<Rezervacija> Sortiraj(List<Rezervacija> rezervacije, string kriterijum)
        {
            switch (kriterijum)
            {
                case "nazivAsc":
                    return rezervacije.OrderBy(r => aranzmanRepo.FindById(r.AranzmanId)?.Naziv).ToList();
                case "nazivDesc":
                    return rezervacije.OrderByDescending(r => aranzmanRepo.FindById(r.AranzmanId)?.Naziv).ToList();
                default:
                    return rezervacije;
            }
        }
    }
}

