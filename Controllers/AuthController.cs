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

namespace FurniflexBE.Controllers
{
    public class AuthController : ApiController
    {
        private AppDbContext db;
        public AuthController()
        {
            db = new AppDbContext();
        }



        [HttpPost, Route("api/Auth/Login")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Login(LoginDTOModel loginModel)
        {
            if (loginModel == null)
            {
                return BadRequest("Invalid login details.");

            }

            // Find user by email
            var user = await db.users.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                return NotFound(); // User not found
            }

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized(); // Incorrect password
            }

            // Login successful
            return Ok(user);
        }


        // POST: api/Users

        [HttpPost, Route("api/Auth/Register")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Register(User user)
        {
            user.RoleId = 2;
            var userRole = await db.roles.FindAsync(2);
            user.Role = userRole;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            db.users.Add(user);
            await db.SaveChangesAsync();


            return CreatedAtRoute("GetUser", new { id = user.UserId }, user);
        }
    }
}
