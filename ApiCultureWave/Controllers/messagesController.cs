using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ApiCultureWave.Clases;
using ApiCultureWave.Models;

namespace ApiCultureWave.Controllers
{
    public class messagesController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/messages
        public IQueryable<message> Getmessage()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.message;
        }

        // GET: api/messages/5
        [ResponseType(typeof(message))]
        public async Task<IHttpActionResult> Getmessage(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            message _message = await db.message
                                .Include("user")
                                .Where(m => m.idUser == id)
                                .FirstOrDefaultAsync();

            if (_message == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_message);
            }

            return result;
        }

        // PUT: api/messages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmessage(int id, message _message)
        {
            IHttpActionResult result;
            String message = "";


            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _message.idMessage)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_message).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!messageExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                        message = Utilities.GetErrorMessage(sqlException);
                        result = BadRequest(message);
                    }
                }
            }

            return result;
        }

        // POST: api/messages
        [ResponseType(typeof(message))]
        public async Task<IHttpActionResult> Postmessage(message _message)
        {
            IHttpActionResult result;


            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.message.Add(_message);
                String message = "";

                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _message.idMessage }, _message);
                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    message = Utilities.GetErrorMessage(sqlException);
                    result = BadRequest(message);
                }
            }

            return result;
        }

        // DELETE: api/messages/5
        [ResponseType(typeof(message))]
        public async Task<IHttpActionResult> Deletemessage(int id)
        {
            IHttpActionResult result;
            String message = "";

            message _message = await db.message.FindAsync(id);
            if (_message == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    db.message.Remove(_message);
                    await db.SaveChangesAsync();
                    result = Ok(message);
                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    message = Utilities.GetErrorMessage(sqlException);
                    result = BadRequest(message);
                }
            }

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool messageExists(int id)
        {
            return db.message.Count(e => e.idMessage == id) > 0;
        }
    }
}
