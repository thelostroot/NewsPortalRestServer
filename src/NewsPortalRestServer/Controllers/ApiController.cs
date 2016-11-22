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
using NewsPortalRestServer;

namespace NewsPortalRestServer.Controllers
{
    [Route("[controller]")]
    public class ApiController : Controller
    {        

        public ApiController()
        {
           
        }

        // GET api/users/?name=denis&age=22
        [HttpGet("{resource:regex(^[[a-zA-Z]]+$)}")]
        public IActionResult GetAll(string resource)
        {      
            try
            {
                // Check filter query
                if (Request.Query.Count > 0)
                    return Json(DBProvider.SelectByFilter(resource, Request.Query.ToList() ));
                        
                return Json(DBProvider.Select("SELECT * FROM " + resource));                    
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

        // GET api/users/3
        [HttpGet("{resource:regex(^[[a-zA-Z]]+$)}/{id:int}")]
        public IActionResult GetById(string resource, int id)
        {            
            try
            {
                return Json(DBProvider.Select("SELECT * FROM " + resource + " WHERE id='" + id.ToString() + "'"));
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

        // GET api/articles/expand
        [HttpGet("{resource}/expand")]
        public IActionResult ExpandGet(string resource)
        {
            try
            {                
                // Try find expand query
                string q = SpecialQueries.TryGetExpandQuery(resource);

                if (q != null)
                    return Json(DBProvider.Select(q));
                else
                    return Json(DBProvider.Select("SELECT * FROM " + resource));                
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

        // GET api/comments/pub_date?start=2016.01.27&end=2016.11.29
        [HttpGet("{resource}/{field}")]
        public IActionResult RangeGet(string resource, string field)
        {
            try
            {
                if (Request.Query.ContainsKey("start") || Request.Query.ContainsKey("end"))
                {
                    var q = SpecialQueries.TryGetRangeQuery(resource, field, Request.Query);
                    if(q == null)
                        return StatusCode(400);

                    return Json(DBProvider.Select(q));
                }
                else
                    return StatusCode(400);
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

        // GET api/users/3/comments
        [HttpGet("{resource}/{id:int}/{subResource:regex(^[[a-zA-Z_]]+$)}")]
        public IActionResult GetSubResource(string resource, int id, string subResource)
        {
            try
            {
                return Json(DBProvider.Select("SELECT * FROM " + subResource + " WHERE " + ResourceMap.TryGetResourceFK(resource) + "=" + id.ToString() ));
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

        // POST api/users
        [HttpPost("{resource}")]
        public IActionResult Add(string resource, [FromBody] JObject RequestData)
        {
            DataModel requestModel;
            try
            {                
                requestModel = (DataModel)RequestData.ToObject(ResourceMap.TryGetResourceType(resource));
            }
            catch
            {
                return StatusCode(400);
            }

            int insertID=0;
            try
            {
                insertID = DBProvider.Insert(resource, requestModel);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }
            
            return Json(new KeyValuePair<string, int>(key: "insert id", value: insertID) );
        }

        // PUT api/users/3
        [HttpPut("{resource}/{id:int}")]
        public IActionResult Update(string resource, int id, [FromBody] JObject RequestData)
        {
            DataModel requestModel;
            try
            {
                requestModel = (DataModel)RequestData.ToObject(ResourceMap.TryGetResourceType(resource));
            }
            catch
            {
                return StatusCode(400);
            }

            try
            {
                DBProvider.Update(resource, "id=" + id.ToString() + "", requestModel);
            }
            catch(DBProviderExecuteException)
            {
                return StatusCode(500);
            }

            return Json(new KeyValuePair<string, int>(key: "update id", value: id));
        }

        // DELETE api/users/3
        [HttpDelete("{resource:regex(^[[a-zA-Z]]+$)}/{id:int}")]
        public IActionResult Delete(string resource, int id)
        {
            try
            {
                DBProvider.Delete(resource, "id='" + id.ToString() + "'");
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
