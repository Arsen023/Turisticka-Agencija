using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MyWebApp.Models;
using Newtonsoft.Json;

namespace MyWebApp.Repositories
{
    public class SmestajRepository
    {
        private static readonly object _lock = new object();
        private readonly string filePath = HttpContext.Current.Server.MapPath("~/App_Data/smestaji.json");

        private List<Smestaj> Read()
        {
            if (!File.Exists(filePath)) return new List<Smestaj>();
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Smestaj>>(json) ?? new List<Smestaj>();
        }

        private void Write(List<Smestaj> smestaji)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var json = JsonConvert.SerializeObject(smestaji, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public List<Smestaj> GetAll()
        {
            lock (_lock) { return Read(); }
        }

        public Smestaj GetById(int id)
        {
            lock (_lock) { return Read().FirstOrDefault(s => s.Id == id); }
        }

        public void Add(Smestaj smestaj)
        {
            lock (_lock)
            {
                var all = Read();
                smestaj.Id = all.Any() ? all.Max(x => x.Id) + 1 : 1; 
                if (smestaj.SmestajneJedinice == null) smestaj.SmestajneJedinice = new List<int>();
                all.Add(smestaj);
                Write(all);
            }
        }

        public void Update(Smestaj updated)
        {
            lock (_lock)
            {
                var all = Read();
                var ix = all.FindIndex(s => s.Id == updated.Id);     
                if (ix == -1) return;
                all[ix] = updated;
                Write(all);
            }
        }

        public void LogicalDelete(int id)
        {
            lock (_lock)
            {
                var all = Read();
                var s = all.FirstOrDefault(x => x.Id == id);
                if (s == null) return;
                s.IsDeleted = true;
                Write(all);
            }
        }
        public void Remove(int id)
        {
            lock (_lock)
            {
                var all = Read();
                all.RemoveAll(s => s.Id == id);
                Write(all);             
            }
        }

        public Smestaj FindByNaziv(string naziv)
        {
            lock (_lock)
            {
                return Read().FirstOrDefault(s =>
                    string.Equals(s.Naziv, naziv, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}

