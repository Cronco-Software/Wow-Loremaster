using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper.Models
{
    public class Layout
    {
        private string _region = "us";
        public string Region
        {
            get
            {
                return this._region;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this._region = value;
                }
            }
        }
        public string RegionLink
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Region) || this.Region == "us")
                {
                    return string.Empty;
                }
                return this.Region;
            }
        }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string CannonicalUrl { get; set; }

    }
}