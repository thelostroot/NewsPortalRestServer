using NewsPortalRestServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public static class ResourceMap
    {
        static Dictionary<string, Resource>  typeMap;
        static ResourceMap()
        {
            //init type map
            typeMap = new Dictionary<string, Resource>();
            typeMap.Add("articles", new Resource(typeof(Article), "article_id") );
            typeMap.Add("categories", new Resource(typeof(Category), "category_id") );
            typeMap.Add("comments", new Resource(typeof(Comment), "comment_id") );
            typeMap.Add("sources", new Resource(typeof(Source), "source_id") );
            typeMap.Add("tags", new Resource(typeof(Tag), "tag_id") );
            typeMap.Add("users", new Resource(typeof(User), "user_id") );
        }

        public static Type GetResourceType(string name)
        {
            try
            {
                return typeMap[name].Type;
            }
            catch
            {
                return null;
            }
        }

        public static string GetResourceFK(string name)
        {
            try
            {
                return typeMap[name].FK;
            }
            catch
            {
                return "";
            }
        }
    }

    public class Resource
    {
        public Type Type { get; set; }
        public string FK { get; set; }

        public Resource(Type type, string fk)
        {
            Type = type;
            FK = fk;
        }
    }
}
