using MyWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;   
using System.Linq;   
using System.Web;

namespace MyWebApp.Service
{
   
    using Newtonsoft.Json;
    using System;
    using System.Globalization;

    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private readonly string[] formats = new[] { 
            "dd/MM/yyyy", 
            "dd/MM/yyyy HH:mm:ss",
            "d.M.yyyy",
            "d.M.yyyy HH:mm:ss",
            "d.M.yyyy. HH:mm:ss",
            "dd.MM.yyyy",
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy. HH:mm:ss",
            "M/d/yyyy h:mm:ss tt",
            "M/d/yyyy",
            "MM/dd/yyyy h:mm:ss tt",
            "MM/dd/yyyy"
        };

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("dd/MM/yyyy"));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return default;

            var str = reader.Value.ToString().Trim();

            if (DateTime.TryParseExact(str, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;

            throw new FormatException($"Ne mogu parsirati datum '{str}' u ocekivane formate.");
        }
    }


    public class AranzmanService
    {
        private List<Aranzman> aranzmani;
        private List<Smestaj> smestaji;
        private List<SmestajnaJedinica> jedinice;

        public AranzmanService()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new CustomDateConverter() }
            };

           
            aranzmani = JsonConvert.DeserializeObject<List<Aranzman>>(
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/aranzmani.json")),
                settings
            );
            smestaji = JsonConvert.DeserializeObject<List<Smestaj>>(
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/smestaji.json"))
            );
            jedinice = JsonConvert.DeserializeObject<List<SmestajnaJedinica>>(
                File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/smestajneJedinice.json"))
            );

        
            foreach (var sm in smestaji)
            {
                sm.SmestajneJedinicee = jedinice.Where(j => j.SmestajID == sm.Id).ToList();
            }

           
            foreach (var a in aranzmani)
            {
                a.Smestaji = smestaji.Where(s => a.SmestajId.Contains(s.Id)).ToList();
            }
        }

        public List<Aranzman> GetAll() => aranzmani;

        public Aranzman GetById(int id)
        {
         
            var aranzman = aranzmani.FirstOrDefault(a => a.Id == id);
            if (aranzman == null)
                return null;

            
            foreach (var sm in aranzman.Smestaji)
            {
                sm.SmestajneJedinicee = jedinice
                    .Where(j => sm.SmestajneJedinice.Contains(j.Id))
                    .ToList();
            }

            return aranzman;
        }

    }
}
