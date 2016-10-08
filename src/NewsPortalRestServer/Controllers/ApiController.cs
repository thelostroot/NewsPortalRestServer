using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Models;

namespace NewsPortalRestServer.Controllers
{
    [Route("[controller]")]
    public class ApiController : Controller
    {
        DBProvider dbProvider;

        public ApiController()
        {
            // init db provider
            dbProvider = new DBProvider();
            dbProvider.Connect("localhost", "5432", "postgres", "1", "ASP");
        }

        // GET api/users
        [HttpGet("{source}")]
        public JsonResult Get(string source)
        {
            return Json(dbProvider.Select("SELECT * FROM " + source));
        }

        // GET api/users/3
        [HttpGet("{source}/{id}")]
        public JsonResult Get(string source, int id)
        {
            return Json(dbProvider.Select("SELECT * FROM " + source + " WHERE id='" + id.ToString() + "'"));
        }

        // POST api/users
        [HttpPost("{source}")]
        public JsonResult Post(string source)
        {
            Dictionary<string, int> res = new Dictionary<string, int>();
            res.Add("Insert id:", dbProvider.Insert(source, GetRequestVars() ) );
            return Json(res);
        }

        // PUT api/users/3
        [HttpPut("{source}/{id}")]
        public JsonResult Put(string source, int id)
        {            
            return Json(dbProvider.Update(source, "id='"+id.ToString()+"'", GetRequestVars() ));
        }

        // DELETE api/users/3
        [HttpDelete("{source}/{id}")]
        public JsonResult Delete(string source, int id)
        {
            return Json(dbProvider.Delete(source, "id='" + id.ToString() + "'"));
        }

        [NonAction]
        private Dictionary<string, string> GetRequestVars()
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            List<string> keys = Request.Form.Keys.ToList();
            foreach (var key in keys)            
                vars.Add(key, Request.Form[key]); 

            return vars;
        }
    }
}
