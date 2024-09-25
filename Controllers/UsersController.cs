using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using FurniflexBE.Context;
using FurniflexBE.Models;

namespace FurniflexBE.Controllers
{
    public class UsersController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        // GET: api/Users
        public IQueryable<User> Getusers()
        {
            return db.users;
        }

        // GET: api/Users/5  using..........
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            // Find the user by ID
            User user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Assuming ProfilePicture contains the relative path, e.g. "uploads/image.jpg"
            // Construct the full URL for the profile picture
            string baseUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}:{Request.RequestUri.Port}/"; // Base URL
            string fullImageUrl = string.IsNullOrEmpty(user.ProfilePicture) ? null : baseUrl + user.ProfilePicture;

            // Create a new object to include the user data and the full image URL
            var userDto = new
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                phone = user.phone,
                location = user.location,
                ProfilePictureUrl = fullImageUrl // Send the full URL of the profile picture
            };

            // Return the modified user object
            return Ok(userDto);
        }

        // PUT: api/Users/5  using..........
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id)
        {
            // Check if the user exists
            var user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Get the multipart form data
            var httpRequest = HttpContext.Current.Request;

            // Check if there's a file uploaded
            if (httpRequest.Files.Count > 0)
            {
                var file = httpRequest.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    // Generate a unique filename
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadsDirectory = HttpContext.Current.Server.MapPath("~/Uploads");

                    // Ensure the uploads directory exists
                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    // Save the file to the server
                    var filePath = Path.Combine(uploadsDirectory, fileName);
                    file.SaveAs(filePath);

                    // Save the file path to the user's profile picture
                    user.ProfilePicture = "/Uploads/" + fileName; // Adjust this path based on your hosting structure
                }
            }

            // Update other user properties
            user.FirstName = httpRequest.Form["FirstName"];
            user.LastName = httpRequest.Form["LastName"];
            user.Email = httpRequest.Form["Email"];
            user.phone = httpRequest.Form["phone"];
            user.location = httpRequest.Form["location"];

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Update the user in the database
            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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




        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.users.Count(e => e.UserId == id) > 0;
        }
    }
}
