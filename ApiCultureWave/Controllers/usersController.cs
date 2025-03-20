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
using ApiCultureWave.Models;
using ApiCultureWave.Clases;

namespace ApiCultureWave.Controllers
{
    public class usersController : ApiController
    {
        private cultureWaveEntities db = new cultureWaveEntities();

        // GET: api/users
        public IQueryable<user> Getuser()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.user;
        }

        // GET: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Getuser(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            user _user = await db.user
                        .Include("message")
                        .Include("rol1")
                        .Include("reserve")
                        .Where(u => u.idUser == id)
                        .FirstOrDefaultAsync();

            if (_user == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_user);
            }

            return result;
        }

        [HttpGet]
        [Route("api/users/name/{name}")]
        public async Task<IHttpActionResult> FindByName(String name)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            List<user> _user = db.user
                                .Where(u => u.name.Contains(name))
                                .ToList();
            return Ok(_user);
        }

        // PUT: api/users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putuser(int id, user _user)
        {
            IHttpActionResult result;
            String message = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _user.idUser)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_user).State = EntityState.Modified;


                    try
                    {
                        await db.SaveChangesAsync();
                        result = StatusCode(HttpStatusCode.NoContent);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!userExists(id))
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

        // POST: api/users
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Postuser(user _user)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.user.Add(_user);
                String message = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _user.idUser }, _user);
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

        // DELETE: api/users/5
        [ResponseType(typeof(user))]
        public async Task<IHttpActionResult> Deleteuser(int id)
        {
            IHttpActionResult result;
            String message = "";

            user _user = await db.user.FindAsync(id);
            if (_user == null)
            {
                result = NotFound();
            }
            else
            {
                try
                {
                    db.user.Remove(_user);
                    await db.SaveChangesAsync();
                    result = Ok(_user);
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

        private bool userExists(int id)
        {
            return db.user.Count(e => e.idUser == id) > 0;
        }
    }
}
