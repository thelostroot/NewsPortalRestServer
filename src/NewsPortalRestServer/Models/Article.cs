using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public class Article : DataModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime pub_date { get; set; }
        public int source_id { get; set; }
        public int category_id { get; set; }
        public string article_text { get; set; }
        public string tags { get; set; }
    }
}
