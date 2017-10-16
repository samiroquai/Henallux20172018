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
using ToDoDemo.Models;

namespace ToDoDemo.Controllers
{
    public class ToDoItemsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ToDoItems
        public IQueryable<ToDoItem> GetToDos()
        {
            return db.ToDos;
        }

        // GET: api/ToDoItems/5
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> GetToDoItem(long id)
        {
            ToDoItem toDoItem = await db.ToDos.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            return Ok(toDoItem);
        }

        // PUT: api/ToDoItems/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutToDoItem(long id, ToDoItem toDoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != toDoItem.Id)
            {
                return BadRequest();
            }

            db.Entry(toDoItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
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

        // POST: api/ToDoItems
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> PostToDoItem(ToDoItem toDoItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ToDos.Add(toDoItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = toDoItem.Id }, toDoItem);
        }

        // DELETE: api/ToDoItems/5
        [ResponseType(typeof(ToDoItem))]
        public async Task<IHttpActionResult> DeleteToDoItem(long id)
        {
            ToDoItem toDoItem = await db.ToDos.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            db.ToDos.Remove(toDoItem);
            await db.SaveChangesAsync();

            return Ok(toDoItem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ToDoItemExists(long id)
        {
            return db.ToDos.Count(e => e.Id == id) > 0;
        }
    }
}