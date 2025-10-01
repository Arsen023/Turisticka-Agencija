using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Komentar
    {
        public int Id { get; set; }
        public string Turista { get; set; }  

        public string SmestajNaziv { get; set; }
        public string Tekst { get; set; } = "";
        public int Ocena { get; set; } = 1;
        public bool Odobren { get; set; } = false;

        public Komentar()
        {
        }

        public Komentar(int id, string turista, string smestajNaziv, string tekst, int ocena, bool odobren)
        {
            Id = id;
            Turista = turista;
            SmestajNaziv = smestajNaziv;
            Tekst = tekst;
            Ocena = ocena;
            Odobren = odobren;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

