using MyWebApp.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MyWebApp.Models
{
    public enum TipAranzmana { nocenje_sa_doruckom, polupansion, pun_pansion, all_inclusive, najam_apartamana }
    public enum TipPrevoza { autobus, avion, autobus_avion, individualan, ostalo}
    public enum LokacijaGdeSePutuje { grad, drzava, regija}
    public class Aranzman
    {
       
        public int Id { get; set; }
        public string Naziv { get; set; } = "";
        public TipAranzmana tipAranzmana {  get; set; } 
        public TipPrevoza tipPrevoza { get; set; }
        public LokacijaGdeSePutuje lokacija {  get; set; }

        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime DatumPocetkaPutovanja { get; set; }
        [JsonConverter(typeof(CustomDateConverter))]
        public DateTime DatumZavrsetkaPutovanja { get; set; }
        public int MaxBrPutnika { get; set; } = 0;
        public string OpisAranzmana { get; set; } = "";
        public string ProgramPutovanja { get; set; } = "";
        public string PosterAranzmana { get; set; } = "";

        [JsonProperty("SmestajId")]
        public List<int> SmestajId { get; set; } = new List<int>();

        
        public List<Smestaj> Smestaji { get; set; } = new List<Smestaj>();

        public bool IsDeleted { get; set; } = false;




        public Aranzman()
        {
        }

        public Aranzman(int id, string naziv, TipAranzmana tipAranzmana, TipPrevoza tipPrevoza, LokacijaGdeSePutuje lokacija, DateTime datumPocetkaPutovanja, DateTime datumZavrsetkaPutovanja, int maxBrPutnika, string opisAranzmana, string programPutovanja, string posterAranzmana, List<int> smestajId, bool jeObrisan)
        {
            Id = id;
            Naziv = naziv;
            this.tipAranzmana = tipAranzmana;
            this.tipPrevoza = tipPrevoza;
            this.lokacija = lokacija;
            DatumPocetkaPutovanja = datumPocetkaPutovanja;
            DatumZavrsetkaPutovanja = datumZavrsetkaPutovanja;
            MaxBrPutnika = maxBrPutnika;
            OpisAranzmana = opisAranzmana;
            ProgramPutovanja = programPutovanja;
            PosterAranzmana = posterAranzmana;
            SmestajId = smestajId;
            IsDeleted = jeObrisan;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

