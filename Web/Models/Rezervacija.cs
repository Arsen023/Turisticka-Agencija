using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public enum Status {aktivna,otkazana,zavrsena}
    public class Rezervacija
    {
        public int Id { get; set; } = 0;
        public string TuristaKojiVrsiRezervaciju { get; set; } = "";
        public Status status { get; set; }
        public int AranzmanId { get; set; }
        public int SmestajnaJedinicaId { get; set; }

        public Rezervacija()
        {
        }

        public Rezervacija(int id, string turistaKojiVrsiRezervaciju, Status status, int aranzmanId, int smestajnaJedinicaId)
        {
            Id = id;
            TuristaKojiVrsiRezervaciju = turistaKojiVrsiRezervaciju;
            this.status = status;
            AranzmanId = aranzmanId;
            SmestajnaJedinicaId = smestajnaJedinicaId;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

