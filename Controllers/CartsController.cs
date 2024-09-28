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
    [Authorize]
    public class CartsController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        // GET: api/Carts
        [HttpGet]
        [ResponseType(typeof(List<Cart>))]
        public IHttpActionResult Getcarts()
        {

            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);

            if (userRole != "admin")
            {
                return Unauthorized(); // Returns 401 Unauthorized if the user is not an admin
            }

            var carts = db.carts.ToList(); // Get all orders as a list

            return Ok(carts);
       
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
            var identityId = IdentityHelper.GetUserId(User.Identity as ClaimsIdentity);
            
            if (identityId == null)
            {
                return Unauthorized();
            }
            

            var cart =await db.carts.FindAsync(id);
            if(cart == null)
            {
                return NotFound();
            }

            var userId = cart.UserId;
            if (userId != identityId)
            {
                return BadRequest("Cart is not Yours, You cannot modify");
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
                        c.Product,
                        ProductName = c.Product.Name,
                        c.Quantity
                    }).ToList()
                }
            });
        }

        // DELETE: api/Carts/DeleteByUserId/5
        [HttpDelete]
        [Route("api/Carts/DeleteByUserId/{userId}")]
        public async Task<IHttpActionResult> DeleteCartByUserId(int userId)
        {
            var identityId = IdentityHelper.GetUserId(User.Identity as ClaimsIdentity);
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);
            if (identityId == null)
            {
                return Unauthorized();
            }
            if (userId != identityId && userRole != "admin")
            {
                return BadRequest("Cart is not Yours, nor you are an admin. You can't delete the cart");
            }
            var carts = db.carts.Where(c => c.UserId == userId).ToList();

            if (!carts.Any())
            {
                return NotFound();
            }

            db.carts.RemoveRange(carts);
            await db.SaveChangesAsync();

            return Ok("Cart items deleted successfully.");
        }

        // DELETE: api/Carts/5
        [ResponseType(typeof(Cart))]
        public async Task<IHttpActionResult> DeleteCart(int id)
        {
            var userId = IdentityHelper.GetUserId(User.Identity as ClaimsIdentity);
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);
            if (userId == null)
            {
                return Unauthorized();
            }

            Cart cart = await db.carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            var cartUserId = cart.User.UserId;
            if (userId != cartUserId && userRole !="admin")
            {
                return BadRequest("Cart is not Yours, nor you are an admin. You can't delete the cart");
            }

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