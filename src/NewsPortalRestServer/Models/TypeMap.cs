using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public class TypeMap
    {
        Dictionary<string, DataModel> map;
        public TypeMap()
        {
            map = new Dictionary<string, DataModel>();
            map.Add("articles", new Article());
            map.Add("categories", new Category());
            map.Add("comments", new Comment());
            map.Add("sources", new Source());
            map.Add("tags", new Tag());
            map.Add("users", new User());
        }
    }
}
