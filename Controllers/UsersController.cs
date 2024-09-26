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
using BCrypt.Net;
using FurniflexBE.DTOModels;
using System.Data.Entity.Validation;

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
        [HttpGet]
        [Route("api/Users/{id:int}", Name = "GetUser")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            // Find the user by ID
            User user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        // GET: api/Users/{id:int}/Image
        [HttpGet]
        [Route("api/Users/{id:int}/Image")]
        public IHttpActionResult GetUserImage(int id)
        {
            var user = db.users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Assuming the ProfilePicture property contains the relative path to the image
            string imagePath = HttpContext.Current.Server.MapPath(user.ProfilePicture);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound();
            }

            var imageFile = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(imageFile)
            };

            // Adjust the ContentType based on the image type you are serving
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            return ResponseMessage(result);
        }

        // PUT: api/Users/5  using..........
        [HttpPut]
        [Route("api/Users/{id:int}", Name = "PutUser")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id)
        {
            var user = await db.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                var file = httpRequest.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadsDirectory = HttpContext.Current.Server.MapPath("~/Uploads");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var filePath = Path.Combine(uploadsDirectory, fileName);
                    file.SaveAs(filePath);

                    user.ProfilePicture = "/Uploads/" + fileName;
                }
            }

            user.FirstName = httpRequest.Form["FirstName"];
            user.LastName = httpRequest.Form["LastName"];
            user.Email = httpRequest.Form["Email"];
            user.Phone = httpRequest.Form["Phone"];
            user.Location = httpRequest.Form["Location"];

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                // Log validation errors
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }

                return InternalServerError(ex);
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







        // DELETE: api/Users/5
        [HttpDelete]
        [Route("api/Users/{id:int}", Name = "DeleteUser")]
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
