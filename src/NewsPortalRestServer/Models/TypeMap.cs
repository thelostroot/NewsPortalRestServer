using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public static class TypeMap
    {
        static Dictionary<string, Type>  typeMap;
        static TypeMap()
        {
            //init type map
            typeMap = new Dictionary<string, Type>();
            typeMap.Add("articles", typeof(Article));
            typeMap.Add("categories", typeof(Category));
            typeMap.Add("comments", typeof(Comment));
            typeMap.Add("sources", typeof(Source));
            typeMap.Add("tags", typeof(Tag));
            typeMap.Add("users", typeof(User));
        }

        public static Dictionary<string, Type> GetTypeMap()
        {
            return typeMap;
        }
    }
}
