using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortal.Models
{
    public class Comment
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int article_id { get; set; }
        public DateTime date { get; set; }
    }
}
