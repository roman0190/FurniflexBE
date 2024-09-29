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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Claims;
using FurniflexBE.Helpers;

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
            var user = await db.users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized(); // Incorrect password
            }

            var jwt_token = GetToken(user);
            // Create a response object that includes only the necessary properties
            var response = new
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                AuthToken = jwt_token,
                FirstName=user.FirstName,
                LastName = user.LastName,
                Location = user.Location,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                CartItems = user.CartItems,
                Orders = user.Orders,
                Reviews=user.Reviews,

                Role = new // Create an anonymous object for the role
                {
                    RoleId = user.Role.RoleId,
                    Name = user.Role.Name
                }
            };
            return Ok(response);
        }


        [Authorize]
        [HttpGet, Route("api/Auth/GetMyData")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetMyData()
        {
            // use these functions for jwt authorization
            var userId = IdentityHelper.GetUserId(User.Identity as ClaimsIdentity);
            var roleId = IdentityHelper.GetRoleId(User.Identity as ClaimsIdentity);

            // Find the user by ID
            User user = await db.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }


            var response = new
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Location = user.Location,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                CartItems = user.CartItems,
                Orders = user.Orders,
                Reviews = user.Reviews,

                Role = new // Create an anonymous object for the role
                {
                    RoleId = user.Role.RoleId,
                    Name = user.Role.Name
                },
                identity = userId,
                role = roleId
            };
            return Ok(response);
        }


        // POST: api/Users

        [HttpPost, Route("api/Auth/Register")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Register(User user)
        {
            user.RoleId = 2; // Example role ID
            var userRole = await db.roles.FindAsync(user.RoleId);
            user.Role = userRole;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            db.users.Add(user);
            await db.SaveChangesAsync();

            var jwt_token = GetToken(user);

            var response = new
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                AuthToken = jwt_token,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Location = user.Location,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                CartItems = user.CartItems,
                Orders = user.Orders,
                Reviews = user.Reviews,

                Role = new // Create an anonymous object for the role
                {
                    RoleId = user.Role.RoleId,
                    Name = user.Role.Name
                }
            };
            return CreatedAtRoute("GetUser", new { id = user.UserId }, response);
        }



        [Authorize]
        [HttpPost, Route("api/Auth/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDTOModel model)
        {
            var userId = IdentityHelper.GetUserId(User.Identity as ClaimsIdentity);


            var user = await db.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                return BadRequest("Current password is incorrect.");
            }

            // Hash the new password
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }



        private String GetToken(User user)
        {
            string key = "my_super_secret_key_1234567890123456"; // 32 characters long key
            var issuer = "http://furniflex.com";  // normally this will be your site URL

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a list of claims
            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("valid", "1"),
                new Claim("userid", user.UserId.ToString()),
                new Claim("roleId", user.RoleId.ToString()), // Include RoleId
                new Claim("roleName", user.Role.Name) // Include RoleName, assuming Role is an object with RoleName property
            };

            // Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, issuer, permClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }




    }
}
