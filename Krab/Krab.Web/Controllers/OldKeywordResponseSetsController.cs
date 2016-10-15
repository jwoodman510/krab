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
using Krab.DataAccess.KeywordResponseSet;

namespace Krab.Web.Controllers
{
    public class OldKeywordResponseSetsController : ApiController
    {
        private KeywordResponseSetsDb db = new KeywordResponseSetsDb();

        // GET: api/OldKeywordResponseSets
        public IQueryable<KeywordResponseSet> GetKeywordResponseSets()
        {
            return db.KeywordResponseSets;
        }

        // GET: api/OldKeywordResponseSets/5
        [ResponseType(typeof(KeywordResponseSet))]
        public IHttpActionResult GetKeywordResponseSet(int id)
        {
            KeywordResponseSet keywordResponseSet = db.KeywordResponseSets.Find(id);
            if (keywordResponseSet == null)
            {
                return NotFound();
            }

            return Ok(keywordResponseSet);
        }

        // PUT: api/OldKeywordResponseSets/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutKeywordResponseSet(int id, KeywordResponseSet keywordResponseSet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != keywordResponseSet.Id)
            {
                return BadRequest();
            }

            db.Entry(keywordResponseSet).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KeywordResponseSetExists(id))
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

        // POST: api/OldKeywordResponseSets
        [ResponseType(typeof(KeywordResponseSet))]
        public IHttpActionResult PostKeywordResponseSet(KeywordResponseSet keywordResponseSet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.KeywordResponseSets.Add(keywordResponseSet);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (KeywordResponseSetExists(keywordResponseSet.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = keywordResponseSet.Id }, keywordResponseSet);
        }

        // DELETE: api/OldKeywordResponseSets/5
        [ResponseType(typeof(KeywordResponseSet))]
        public IHttpActionResult DeleteKeywordResponseSet(int id)
        {
            KeywordResponseSet keywordResponseSet = db.KeywordResponseSets.Find(id);
            if (keywordResponseSet == null)
            {
                return NotFound();
            }

            db.KeywordResponseSets.Remove(keywordResponseSet);
            db.SaveChanges();

            return Ok(keywordResponseSet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool KeywordResponseSetExists(int id)
        {
            return db.KeywordResponseSets.Count(e => e.Id == id) > 0;
        }
    }
}