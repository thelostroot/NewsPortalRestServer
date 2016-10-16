using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public class Category : DataModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
