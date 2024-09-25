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
using FurniflexBE.Context;
using FurniflexBE.Models;

namespace FurniflexBE.Controllers
{
    public class CartsController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        // GET: api/Carts
        public IQueryable<Cart> Getcarts()
        {
            return db.carts;
        }

        // GET: api/Carts/5
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> GetCart(int id)
        {
            Cart cart = await db.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        // PUT: api/Carts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCart(int id, int changeValue)
        {


            var cart =await db.carts.FindAsync(id);
            if(cart == null)
            {
                return NotFound();
            }

            cart.Quantity += changeValue;

            if (cart.Quantity < 1)
            {

                db.carts.Remove(cart);
            }
            else
            {

            db.Entry(cart).State = EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/Carts
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> PostCart(Cart cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Add the cart to the database
            db.carts.Add(cart);
            await db.SaveChangesAsync();

            // Fetch the product and user again, and include their relationships (eager loading)
            var product = await db.products.FindAsync(cart.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            var user = await db.users
                .Include(u => u.CartItems)  // Eagerly load cart items
                .FirstOrDefaultAsync(u => u.UserId == cart.UserId);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Map the loaded user and product back to the cart
            cart.User = user;
            cart.Product = product;

            // Return the newly created cart with the updated user and product
            return CreatedAtRoute("DefaultApi", new { id = cart.CartId }, new
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                ProductId = cart.ProductId,
                ProductName = product.Name,
                Quantity = cart.Quantity,
                User = new
                {
                    user.UserId,
                    user.FirstName,
                    user.LastName,
                    CartItems = user.CartItems.Select(c => new
                    {
                        c.CartId,
                        c.ProductId,
                        ProductName = c.Product.Name,
                        c.Quantity
                    }).ToList()
                }
            });
        }


        // DELETE: api/Carts/5
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> DeleteCart(int id)
        {
            Cart cart = await db.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            var userId = cart.User.UserId;

            db.carts.Remove(cart);
            await db.SaveChangesAsync();

            return Ok(cart);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CartExists(int id)
        {
            return db.carts.Count(e => e.CartId == id) > 0;
        }
    }
}