using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsPortalRestServer.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.ComponentModel;
using NewsPortalRestServer.Service;

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

            try
            {
                dbProvider.Connect("localhost", "5432", "postgres", "1", "ASP");
            }
            catch(DBProviderConnectException)
            {
                StatusCode(500);
            }                    
        }

        // GET api/users
        [HttpGet("{resource}")]
        public IActionResult Get(string resource)
        {
            try
            {
                return Json(dbProvider.Select("SELECT * FROM " + resource));
            }
            catch(Npgsql.PostgresException)
            {
                return StatusCode(400);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }
            
        }

        // GET api/users/3
        [HttpGet("{source}/{id:int}")]
        public IActionResult Get(string source, int id)
        {
            try
            {
                return Json(dbProvider.Select("SELECT * FROM " + source + " WHERE id='" + id.ToString() + "'"));
            }
            catch (Npgsql.PostgresException)
            {
                return StatusCode(400);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }            
        }        

        // POST api/users
        [HttpPost("{resource}")]
        public IActionResult Post(string resource, [FromBody] JObject RequestData)
        {

            DataModel requestModel;            
            try
            {
                requestModel = (DataModel)RequestData.ToObject(ResourceMap.GetResourceType(resource));
            }
            catch
            {
                return StatusCode(400);
            }

            int insertID=0;
            try
            {
                insertID = dbProvider.Insert(resource, requestModel);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }
            
            return Json(new KeyValuePair<string, int>(key: "insert id", value: insertID) );
        }

        // PUT api/users/3
        [HttpPut("{resource}/{id:int}")]
        public IActionResult Put(string resource, int id, [FromBody] JObject RequestData)
        {
            DataModel requestModel;
            try
            {
                requestModel = (DataModel)RequestData.ToObject(ResourceMap.GetResourceType(resource));
            }
            catch
            {
                return StatusCode(400);
            }

            try
            {
                dbProvider.Update(resource, "id=" + id.ToString() + "", requestModel);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }

            return Json(new KeyValuePair<string, int>(key: "update id", value: id));
        }

        // DELETE api/users/3
        [HttpDelete("{resource}/{id:int}")]
        public IActionResult Delete(string resource, int id)
        {
            try
            {
                dbProvider.Delete(resource, "id='" + id.ToString() + "'");
                return Json(new KeyValuePair<string, int>(key: "delete id", value: id));
            }
            catch (Npgsql.PostgresException)
            {
                return StatusCode(400);
            }
            catch (DBProviderExecuteException)
            {
                return StatusCode(500);
            }           
        }        
    }
}
