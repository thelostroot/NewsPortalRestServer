using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public class Comment : DataModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public DateTime pub_date { get; set; }
        public string text { get; set; }
    }
}
