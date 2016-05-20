using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class FilesController : ApiController
    {
        // GET: api/Files
        public IHttpActionResult Get()
        {


            File rootFile = MvcApplication.fileSystem.root;
            if (rootFile == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(rootFile);
            }
         }

        // GET: api/Files/5
        public HttpResponseMessage Get(int id)
        {
            File getFile = MvcApplication.fileSystem.GetFile(id);
            if (getFile == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) ; 
            }

            byte[] fileInBytes = MvcApplication.fileSystem.GetFileContents (id);
            
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileInBytes)
            };
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = getFile.name
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }

        // POST: api/Files
        
        public IHttpActionResult Post([FromBody]File value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            else
            {
                File newFile = MvcApplication.fileSystem.NewFile(value.parentId, value.name, value.isDirectory);

                if (newFile != null)
                {
                    return Created("api/Files/"+newFile.id, newFile);
                }
                {
                    return BadRequest();
                }

            }
        }
        

        // PUT: api/Files/5
        public void Put(int id)
        {
            long contentLength = Request.Content.Headers.ContentLength.Value;
            Type contentType = Request.Content.GetType();
           
            // TODO imporve async handling of file IO and file size issues
            byte[] fileContents =  Request.Content.ReadAsByteArrayAsync().Result;

            MvcApplication.fileSystem.UpdateFileContents(id, fileContents);
        }

        // DELETE: api/Files/5
        public void Delete(int id)
        {
            if (id == MvcApplication.fileSystem.root.id)
            {
                //TODO erro for trying to delete root

            }
            else
            {
                bool deleted = MvcApplication.fileSystem.DeleteFile(id);
                // todo not deleted excepton
            }
        }


    }
}
