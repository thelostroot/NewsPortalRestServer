using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Models
{
    public class Article
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime timestamp { get; set; }
        public int source_id { get; set; }
        public int category_id { get; set; }
        public string article_text { get; set; }
        public string tags { get; set; }
    }
}
