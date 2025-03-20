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
    public class reservesController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/reserves
        public IQueryable<reserve> Getreserve()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.reserve;
        }

        // GET: api/reserves/5
        [ResponseType(typeof(reserve))]
        public async Task<IHttpActionResult> Getreserve(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            reserve _reserve = await db.reserve
                                .Include("event")
                                .Include("seat")
                                .Include("ticket")
                                .Include("user")
                                .Where(r => r.idReserve == id)
                                .FirstOrDefaultAsync();

            if (_reserve == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_reserve);
            }

            return result;
        }

        // PUT: api/reserves/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putreserve(int id, reserve _reserve)
        {
            IHttpActionResult result;
            String message = "";


            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _reserve.idReserve)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_reserve).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!reserveExists(id))
                        {
                            result = NotFound();
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

        // POST: api/reserves
        [ResponseType(typeof(reserve))]
        public async Task<IHttpActionResult> Postreserve(reserve reserve)
        {
            IHttpActionResult result;


            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.reserve.Add(reserve);
                String message = "";

                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = reserve.idReserve }, reserve);
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

        // DELETE: api/reserves/5
        [ResponseType(typeof(reserve))]
        public async Task<IHttpActionResult> Deletereserve(int id)
        {
            IHttpActionResult result;
            String message = "";


            reserve _reserve = await db.reserve.FindAsync(id);
            if (_reserve == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    db.reserve.Remove(_reserve);
                    await db.SaveChangesAsync();
                    result = Ok(_reserve);
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

        private bool reserveExists(int id)
        {
            return db.reserve.Count(e => e.idReserve == id) > 0;
        }
    }
}
