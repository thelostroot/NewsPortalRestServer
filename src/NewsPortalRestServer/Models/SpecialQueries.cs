using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public static class SpecialQueries
    {
        private static Dictionary<string, string> expandQueries;

        static SpecialQueries()
        {
            expandQueries = new Dictionary<string, string>();
            expandQueries.Add("articles", "SELECT articles.id, articles.title, articles.pub_date, sources.name AS source, categories.name AS category, articles.article_text, articles.tags FROM articles INNER JOIN sources ON articles.source_id = sources.id INNER JOIN categories ON articles.category_id = categories.id");
            expandQueries.Add("comments", "SELECT comments.id, CONCAT_WS(' ', users.name, users.last_name) AS user, articles.title AS article_title, comments.pub_date, comments.text FROM comments INNER JOIN users ON comments.user_id = users.id INNER JOIN articles ON comments.article_id = articles.id");
        }

        public static string TryGetExpandQuery(string resource)
        {
            if (expandQueries.ContainsKey(resource))
                return expandQueries[resource];
            else
                return null;
        }
    }
}
