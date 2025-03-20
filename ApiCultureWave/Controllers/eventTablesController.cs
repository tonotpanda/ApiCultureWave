using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ApiCultureWave.Models;

namespace ApiCultureWave.Controllers
{
    public class eventTablesController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/eventTables
        public IQueryable<eventTable> GeteventTable()
        {
            return db.eventTable;
        }

        // GET: api/eventTables/5
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> GeteventTable(int id)
        {
            eventTable eventTable = await db.eventTable.FindAsync(id);
            if (eventTable == null)
            {
                return NotFound();
            }

            return Ok(eventTable);
        }

        // PUT: api/eventTables/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PuteventTable(int id, eventTable eventTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != eventTable.idEvent)
            {
                return BadRequest();
            }

            db.Entry(eventTable).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!eventTableExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/eventTables
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> PosteventTable(eventTable eventTable)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.eventTable.Add(eventTable);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = eventTable.idEvent }, eventTable);
        }

        // DELETE: api/eventTables/5
        [ResponseType(typeof(eventTable))]
        public async Task<IHttpActionResult> DeleteeventTable(int id)
        {
            eventTable eventTable = await db.eventTable.FindAsync(id);
            if (eventTable == null)
            {
                return NotFound();
            }

            db.eventTable.Remove(eventTable);
            await db.SaveChangesAsync();

            return Ok(eventTable);
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