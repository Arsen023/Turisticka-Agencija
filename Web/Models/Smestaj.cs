using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public enum TipSmestaja { hotel, motel, vila}
    public class Smestaj
    {
        public int Id { get; set; }   
        public int AranzmanId { get; set; } 
        public TipSmestaja tipSmestaja { get; set; } 
        public string Naziv { get; set; } = "";
        public int? BrojZvezdica { get; set; } = 0;
        public bool PostojanjeBazena { get; set; } = false;
        public bool PostojanjeSpaCentra { get; set; } = false;
        public bool PrilagodjenoZaOsobeSaInvaliditeton { get; set; } = false;
        public bool wifi {  get; set; } = false;
      
        public List<int> SmestajneJedinice { get; set; } = new List<int>();

        public List<Komentar> Komentari { get; set; } = new List<Komentar>();
        
        [JsonIgnore]
        public List<SmestajnaJedinica> SmestajneJedinicee { get; set; } = new List<SmestajnaJedinica>();
        public bool IsDeleted { get; set; } = false;
        public Smestaj()
        {
        }

        public Smestaj(int id, int aranzmanId, TipSmestaja tipSmestaja, string naziv, int? brojZvezdica, bool postojanjeBazena, bool postojanjeSpaCentra, bool prilagodjenoZaOsobeSaInvaliditeton, bool wifi, List<int> smestajneJedinicee, List<Komentar> komentari, bool obrisan)
        {
            Id = id;
            AranzmanId = aranzmanId;
            this.tipSmestaja = tipSmestaja;
            Naziv = naziv;
            BrojZvezdica = brojZvezdica;
            PostojanjeBazena = postojanjeBazena;
            PostojanjeSpaCentra = postojanjeSpaCentra;
            PrilagodjenoZaOsobeSaInvaliditeton = prilagodjenoZaOsobeSaInvaliditeton;
            this.wifi = wifi;
            SmestajneJedinice = smestajneJedinicee;
            Komentari = komentari;
            IsDeleted = obrisan;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

