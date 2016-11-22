using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NewsPortalRestServer.Models
{
    public static class SpecialQueries
    {
        private static Dictionary<string, string> expandQueries;

        private enum RangeType {Date, Number};
        private static Dictionary<KeyValuePair<string, string>, RangeType> rangeQueries;

        // Register special queries
        static SpecialQueries()
        {
            // table name <---> query
            expandQueries = new Dictionary<string, string>();
            expandQueries.Add("articles", "SELECT articles.id, articles.title, articles.pub_date, sources.name AS source, categories.name AS category, articles.article_text, articles.tags FROM articles INNER JOIN sources ON articles.source_id = sources.id INNER JOIN categories ON articles.category_id = categories.id");
            expandQueries.Add("comments", "SELECT comments.id, CONCAT_WS(' ', users.name, users.last_name) AS user, articles.title AS article_title, comments.pub_date, comments.text FROM comments INNER JOIN users ON comments.user_id = users.id INNER JOIN articles ON comments.article_id = articles.id");

            // filed name <---> where type
            rangeQueries = new Dictionary<KeyValuePair<string, string>, RangeType>();
            rangeQueries.Add(new KeyValuePair<string, string>("articles", "pub_date"), RangeType.Date );
            rangeQueries.Add(new KeyValuePair<string, string>("comments", "pub_date"), RangeType.Date );
        }

        public static string TryGetExpandQuery(string resource)
        {
            if (expandQueries.ContainsKey(resource))
                return expandQueries[resource];
            else
                return null;
        }

        public static string TryGetRangeQuery(string resource, string filed, IQueryCollection vars)
        {
            var key = new KeyValuePair<string, string>(resource, filed);
            if(rangeQueries.ContainsKey(key))
            {
                string q = "SELECT * FROM " + resource + " WHERE ";

                string startStr="";
                string endStr="";
                switch (rangeQueries[key])
                {
                    case RangeType.Date:
                        startStr = filed + ">= to_timestamp('" + vars["start"].ToString() + "', 'YYYY.MM.DD')";
                        endStr = filed + "<= to_timestamp('" + vars["end"].ToString() + "', 'YYYY.MM.DD')";
                        break;

                    case RangeType.Number:
                        startStr = filed + ">= " + vars["start"].ToString();
                        endStr = filed + "<= " + vars["end"].ToString();
                        break;
                }

                // By to border
                if (vars.ContainsKey("start") && vars.ContainsKey("end"))
                    return q + startStr + " AND " + endStr;

                // By start
                if (vars.ContainsKey("start") && !vars.ContainsKey("end"))
                    return q + startStr;

                // By end
                if (!vars.ContainsKey("start") && vars.ContainsKey("end"))
                    return q + endStr;

                return q;
            }
            else
                return null;
        }        
    }
}
