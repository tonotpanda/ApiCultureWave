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
    public class eventTablesController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/eventTables
        public IQueryable<eventTable> GeteventTable()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.eventTable;
        }

        // GET: api/eventTables/5
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> GeteventTable(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            eventTable _eventTable = await db.eventTable
                                        .Include("space")
                                        .Include("reserve")
                                        .Where(e => e.idEvent == id)
                                        .FirstOrDefaultAsync();

            if (_eventTable == null)
            {
                result = NotFound();
            }
            else 
            {
                result = Ok(_eventTable);
            }

            return result;
        }

        // GET: api/eventTables/ReservesByUser/5
        [HttpGet]
        [Route("api/eventTables/ReservesByUser/{idUser}")]
        public async Task<IHttpActionResult> GetReservesByUser(int idUser)
        {
            db.Configuration.LazyLoadingEnabled = false;

            try
            {
                // Trae reservas que pertenecen al usuario y carga el evento
                var reserves = await db.reserve
                    .Where(r => r.user.Any(u => u.idUser == idUser))
                    .Include("eventTable")
                    .Select(r => new
                    {
                        r.idReserve,
                        r.reserveDate,
                        EventName = r.eventTable.name,
                        r.idEvent
                    })
                    .ToListAsync();

                if (reserves == null || reserves.Count == 0)
                {
                    return NotFound();
                }

                return Ok(reserves);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/eventTables/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PuteventTable(int id, eventTable _eventTable)
        {
            IHttpActionResult result;
            String message = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else 
            {
                if (id != _eventTable.idEvent)
                {
                    result = BadRequest();
                }
                else 
                {
                    db.Entry(_eventTable).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!eventTableExists(id))
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

        // POST: api/eventTables
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> PosteventTable(eventTable eventTable)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else 
            {
                db.eventTable.Add(eventTable);
                String message = "";

                try 
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = eventTable.idEvent }, eventTable);
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

        // DELETE: api/eventTables/5
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> DeleteeventTable(int id)
        {
            IHttpActionResult result;
            String message = "";

            eventTable _eventTable = await db.eventTable.FindAsync(id);
            if (_eventTable == null)
            {
                return NotFound();
            }
            else 
            {
                try 
                {
                    db.eventTable.Remove(_eventTable);
                    await db.SaveChangesAsync();
                    result = Ok(_eventTable);
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

        private bool eventTableExists(int id)
        {
            return db.eventTable.Count(e => e.idEvent == id) > 0;
        }
    }
}