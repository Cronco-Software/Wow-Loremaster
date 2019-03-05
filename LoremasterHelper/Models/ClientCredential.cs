using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper.Models
{
    public class ClientCredential
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}