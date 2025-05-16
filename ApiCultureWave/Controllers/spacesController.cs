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
    public class spacesController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/spaces
        public IQueryable<space> Getspace()
        {
            db.Configuration.LazyLoadingEnabled = false;
            var spaces = db.space.ToList();
            return db.space;
        }

        // GET: api/spaces/5
        [ResponseType(typeof(space))]
        public async Task<IHttpActionResult> Getspace(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            space _space = await db.space
                            .Include("event")
                            .Include("seat")
                            .Where(s => s.idSpace == id)
                            .FirstOrDefaultAsync();

            if (_space == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_space);
            }

            return result;
        }

        // PUT: api/spaces/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putspace(int id, space space)
        {
            IHttpActionResult result;
            String message = "";


            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != space.idSpace)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(space).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!spaceExists(id))
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

        // POST: api/spaces
        [ResponseType(typeof(space))]
        public async Task<IHttpActionResult> Postspace(space space)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.space.Add(space);
                String message = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = space.idSpace }, space);
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

        // DELETE: api/spaces/5
        [ResponseType(typeof(space))]
        public async Task<IHttpActionResult> Deletespace(int id)
        {
            IHttpActionResult result;
            String message = "";

            space _space = await db.space.FindAsync(id);
            if (_space == null)
            {
                result = NotFound();
            }
            else
            {
                try
                {
                    db.space.Remove(_space);
                    await db.SaveChangesAsync();
                    result = Ok(_space);
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

        private bool spaceExists(int id)
        {
            return db.space.Count(e => e.idSpace == id) > 0;
        }
    }
}
