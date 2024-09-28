using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using FurniflexBE.Context;
using FurniflexBE.Helpers;
using FurniflexBE.Models;

namespace FurniflexBE.Controllers
{
    public class CategoriesController : ApiController
    {
        AppDbContext db; 

        public CategoriesController()
        {
            db = new AppDbContext();
        }



        // GET: api/Categories
        public IQueryable<Category> Getcategories()
        {
            return db.categories;
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            Category category = await db.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [Authorize]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCategory(int id, Category category)
        {
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);
            if (userRole != "admin")
            {
                return BadRequest("You are not admin, You cannot manage categories");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            // Check if category with the same name already exists (but is not the current category being edited)
            var existingCategory = await db.categories
                                          .FirstOrDefaultAsync(c => c.Name == category.Name && c.CategoryId != id);
            if (existingCategory != null)
            {
                return BadRequest("A category with the same name already exists.");
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        [Authorize]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(Category category)
        {
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);
            if (userRole != "admin")
            {
                return BadRequest("You are not admin, You cannot post categories");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a category with the same name already exists
            var existingCategory = await db.categories.FirstOrDefaultAsync(c => c.Name == category.Name);
            if (existingCategory != null)
            {
                return Conflict(); // Return 409 Conflict if category already exists
            }

            // If category doesn't exist, add new category
            db.categories.Add(category);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, category);
        }


        // DELETE: api/Categories/5
        [Authorize]
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);
            if (userRole != "admin")
            {
                return BadRequest("You are not admin, You cannot delete categories");
            }
            Category category = await db.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            db.categories.Remove(category);
            await db.SaveChangesAsync();

            return Ok(category);
        }

        // New API to count categories
        // GET: api/Categories/Count
        [HttpGet]
        [Route("api/Categories/Count")] 
        public IHttpActionResult GetCategoryCount()
        {
            
            int categoryCount = db.categories.Count();

            
            return Ok(new { count = categoryCount });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.categories.Count(e => e.CategoryId == id) > 0;
        }
    }
}