using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public enum StatusSmestajneJedinice { Slobodna, Zauzeta}
    public class SmestajnaJedinica
    {
        public int Id { get; set; }

        [JsonProperty("SmestajID")]
        public int SmestajID { get; set; }
        public int DozvoljenBrojGostiju { get; set; } = 0;
        public bool DozvoljeniKucniLjubimci { get; set; } = false;
        public float CenaZaCeluSmestajnuJedinicu { get; set; } = 0;
        public StatusSmestajneJedinice Status { get; set; }
        public bool IsDeleted { get; set; } = false;


        public SmestajnaJedinica()
        {
        }

        public SmestajnaJedinica(int id, int smestajID, int dozvoljenBrojGostiju, bool dozvoljeniKucniLjubimci, float cenaZaCeluSmestajnuJedinicu, StatusSmestajneJedinice status, bool isDeleted)
        {
            Id = id;
            SmestajID = smestajID;
            DozvoljenBrojGostiju = dozvoljenBrojGostiju;
            DozvoljeniKucniLjubimci = dozvoljeniKucniLjubimci;
            CenaZaCeluSmestajnuJedinicu = cenaZaCeluSmestajnuJedinicu;
            Status = status;
            IsDeleted = isDeleted;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

