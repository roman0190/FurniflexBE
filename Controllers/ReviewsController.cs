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
    public class ReviewsController : ApiController
    {
        private AppDbContext db;

        public ReviewsController()
        {
            db = new AppDbContext();
        }

        // GET: api/Reviews
        public IQueryable<Review> Getreviews()
        {
            return db.reviews;
        }
        // GET: api/Reviews/product/{productId}
        [HttpGet]
        [Route("api/Reviews/Product/{productId}")]
        [ResponseType(typeof(IEnumerable<Review>))]
        public async Task<IHttpActionResult> GetReviewsByProduct(int productId)
        {
            var reviews = await db.reviews.Where(r => r.ProductId == productId).ToListAsync();

            if (reviews == null || !reviews.Any())
            {
                return NotFound(); // No reviews found for the product
            }

            return Ok(reviews);
        }



        // GET: api/Reviews/5
        [ResponseType(typeof(Review))]
        public async Task<IHttpActionResult> GetReview(int id)
        {
            Review review = await db.reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }

        // PUT: api/Reviews/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReview(int id, Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the existing review by review id
            var existingReview = await db.reviews.FindAsync(id);

            if (existingReview == null)
            {
                return NotFound(); // Review not found
            }

            // Check if the review belongs to the authenticated user
            var user = await db.users.FindAsync(review.UserId);

            if (existingReview.UserId != review.UserId)
            {
                return Unauthorized(); // User can only edit their own review
            }

            // Ensure the user has at least one order where the product exists and the status is "Delivered"
            var hasDeliveredOrder = user.Orders
                                        .Any(order => order.OrderItems
                                                           .Any(p => p.ProductId == review.ProductId)
                                                    && order.OrderStatus == "Delivered");

           

            // Update the review's content
            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
           

            db.Entry(existingReview).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent); // Success, but no content is returned
        }
        // POST: api/Reviews
        [Authorize]
        [ResponseType(typeof(Review))]
        public async Task<IHttpActionResult> PostReview(Review review)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by the UserId from the review
            var user = await db.users.FindAsync(review.UserId);

            // Check if the user exists
            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }

           
            // Check if the user has already submitted a review for this product
            var existingReview = user.Reviews.FirstOrDefault(r => r.ProductId == review.ProductId);

            if (existingReview != null)
            {
                // Return a custom response indicating the user already reviewed this product
                return BadRequest("You have already submitted a review for this product.");
            }

            // Ensure the user has at least one order where the product exists and the status is "Delivered"
            var hasDeliveredOrder = user.Orders
                                        .Any(order => order.OrderItems
                                                           .Any(p => p.ProductId == review.ProductId)
                                                    && order.OrderStatus == "Delivered");

            if (!hasDeliveredOrder)
            {
                return BadRequest("You can only review products that you have ordered and that have been delivered.");
            }

            // Add the new review since no existing review for the product was found
            db.reviews.Add(review);
            await db.SaveChangesAsync();

            // Return the newly created review
            return CreatedAtRoute("DefaultApi", new { id = review.ReviewId }, review);
        }


        // DELETE: api/Reviews/5
        [ResponseType(typeof(Review))]
        public async Task<IHttpActionResult> DeleteReview(int id)
        {
            Review review = await db.reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            db.reviews.Remove(review);
            await db.SaveChangesAsync();

            return Ok(review);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReviewExists(int id)
        {
            return db.reviews.Count(e => e.ReviewId == id) > 0;
        }
    }
}