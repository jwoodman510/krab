using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using REST.Models;

namespace REST.Controllers
{
    public class KeywordResponseSetsModelsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/KeywordResponseSetsModels
        public IQueryable<KeywordResponseSetsModel> GetKeywordResponseSetsModels()
        {
            return db.KeywordResponseSetsModels;
        }

        // GET: api/KeywordResponseSetsModels/5
        [ResponseType(typeof(KeywordResponseSetsModel))]
        public IHttpActionResult GetKeywordResponseSetsModel(int id)
        {
            KeywordResponseSetsModel keywordResponseSetsModel = db.KeywordResponseSetsModels.Find(id);
            if (keywordResponseSetsModel == null)
            {
                return NotFound();
            }

            return Ok(keywordResponseSetsModel);
        }

        // PUT: api/KeywordResponseSetsModels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKeywordResponseSetsModel(int id, KeywordResponseSetsModel keywordResponseSetsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != keywordResponseSetsModel.Id)
            {
                return BadRequest();
            }

            db.Entry(keywordResponseSetsModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeywordResponseSetsModelExists(id))
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

        // POST: api/KeywordResponseSetsModels
        [ResponseType(typeof(KeywordResponseSetsModel))]
        public IHttpActionResult PostKeywordResponseSetsModel(KeywordResponseSetsModel keywordResponseSetsModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.KeywordResponseSetsModels.Add(keywordResponseSetsModel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (KeywordResponseSetsModelExists(keywordResponseSetsModel.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = keywordResponseSetsModel.Id }, keywordResponseSetsModel);
        }

        // DELETE: api/KeywordResponseSetsModels/5
        [ResponseType(typeof(KeywordResponseSetsModel))]
        public IHttpActionResult DeleteKeywordResponseSetsModel(int id)
        {
            KeywordResponseSetsModel keywordResponseSetsModel = db.KeywordResponseSetsModels.Find(id);
            if (keywordResponseSetsModel == null)
            {
                return NotFound();
            }

            db.KeywordResponseSetsModels.Remove(keywordResponseSetsModel);
            db.SaveChanges();

            return Ok(keywordResponseSetsModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KeywordResponseSetsModelExists(int id)
        {
            return db.KeywordResponseSetsModels.Count(e => e.Id == id) > 0;
        }
    }
}