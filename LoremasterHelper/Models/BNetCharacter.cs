using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LoremasterHelper.Models
{
    public class BNetCharacter
    {
        public string Region { get; set; }
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Class { get; set; }
        public string Race { get; set; }
        public int Level { get; set; }
        public Faction Faction { get; set; }
        public string ThumbnailPath { get; set; }
    }
}