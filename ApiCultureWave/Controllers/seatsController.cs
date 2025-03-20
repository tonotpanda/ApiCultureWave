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
    public class seatsController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/seats
        public IQueryable<seat> Getseat()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.seat;
        }

        // GET: api/seats/5
        [ResponseType(typeof(seat))]
        public async Task<IHttpActionResult> Getseat(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            seat _seat = await db.seat
                                .Include("reserve")
                                .Include("space")
                                .Where(s => s.idSeat == id)
                                .FirstOrDefaultAsync();

            if (_seat == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_seat);
            }

            return result;
        }

        // PUT: api/seats/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putseat(int id, seat seat)
        {
            IHttpActionResult result;
            String message = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != seat.idSeat)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(seat).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!seatExists(id))
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

        // POST: api/seats
        [ResponseType(typeof(seat))]
        public async Task<IHttpActionResult> Postseat(seat seat)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.seat.Add(seat);
                String message = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = seat.idSeat }, seat);
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

        // DELETE: api/seats/5
        [ResponseType(typeof(seat))]
        public async Task<IHttpActionResult> Deleteseat(int id)
        {
            IHttpActionResult result;
            String message = "";


            seat _seat = await db.seat.FindAsync(id);
            if (_seat == null)
            {
                result = NotFound();
            }
            else
            {
                try
                {
                    db.seat.Remove(_seat);
                    await db.SaveChangesAsync();
                    result = Ok(_seat);
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

        private bool seatExists(int id)
        {
            return db.seat.Count(e => e.idSeat == id) > 0;
        }
    }
}
