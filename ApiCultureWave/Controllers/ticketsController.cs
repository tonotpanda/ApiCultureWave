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
    public class ticketsController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/tickets
        public IQueryable<ticket> Getticket()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.ticket;
        }

        // GET: api/tickets/5
        [ResponseType(typeof(ticket))]
        public async Task<IHttpActionResult> Getticket(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            ticket _ticket = await db.ticket
                                .Include("reserve")
                                .Where(r => r.idTicket == id)
                                .FirstOrDefaultAsync();

            if (_ticket == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_ticket);
            }

            return result;
        }

        // PUT: api/tickets/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putticket(int id, ticket ticket)
        {
            IHttpActionResult result;
            String message = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != ticket.idTicket)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(ticket).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ticketExists(id))
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

        // POST: api/tickets
        [ResponseType(typeof(ticket))]
        public async Task<IHttpActionResult> Postticket(ticket ticket)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                db.ticket.Add(ticket);
                String message = "";

                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = ticket.idTicket }, ticket);
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

        // DELETE: api/tickets/5
        [ResponseType(typeof(ticket))]
        public async Task<IHttpActionResult> Deleteticket(int id)
        {
            IHttpActionResult result;
            String message = "";


            ticket ticket = await db.ticket.FindAsync(id);
            if (ticket == null)
            {
                result = NotFound();
            }
            else
            {
                try
                {
                    db.ticket.Remove(ticket);
                    await db.SaveChangesAsync();
                    result = Ok(ticket);
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

        private bool ticketExists(int id)
        {
            return db.ticket.Count(e => e.idTicket == id) > 0;
        }
    }
}
