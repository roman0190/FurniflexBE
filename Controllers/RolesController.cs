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
    
    public class RolesController : ApiController
    {
        private AppDbContext db;

        public RolesController()
        {
            db = new AppDbContext();
        }

        // GET: api/Roles
        [AllowAnonymous]
        public IQueryable<Role> GetRoles()
        {
            return db.roles;
        }



        // PUT: api/Roles/5
        [Authorize]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRole(int id, Role role)
        {
         

            var identity = User.Identity as ClaimsIdentity;
            var roleClaim = identity?.FindFirst("roleName");
            if (roleClaim == null || roleClaim.Value != "admin")
            {
                return BadRequest($"User Role is {roleClaim.Value}. and not 'admin'. User cannot do the action");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != role.RoleId)
            {
                return BadRequest();
            }

            db.Entry(role).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // POST: api/Roles

        [ResponseType(typeof(Role))]
        public async Task<IHttpActionResult> PostRole(Role role)
        {
            var userRole = IdentityHelper.GetRoleName(User.Identity as ClaimsIdentity);

            if (userRole != "admin")
            {
                return Unauthorized(); // Returns 401 Unauthorized if the user is not an admin
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.roles.Add(role);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = role.RoleId }, role);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RoleExists(int id)
        {
            return db.roles.Count(e => e.RoleId == id) > 0;
        }
    }
}