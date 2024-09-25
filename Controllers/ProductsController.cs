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
    public class ProductsController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        // GET: api/Products using this................don't touch
        public IQueryable<Product> Getproducts()
        {
            return db.products;
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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
        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct()
        {
           
            Product product = new Product();

            try
            {
               
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count == 0)
                {
                    return BadRequest("No image file uploaded.");
                }

                var postedFile = httpRequest.Files[0];

                var imageFolder = HttpContext.Current.Server.MapPath("~/Images/Products/");
                var fileName = Path.GetFileName(postedFile.FileName);
                var fullPath = Path.Combine(imageFolder, fileName);

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                postedFile.SaveAs(fullPath);

                product.Name = httpRequest.Form["Name"];
                product.ImgUrl = $"/Images/Products/{fileName}";
                product.CategoryId = int.Parse(httpRequest.Form["CategoryId"]);
                product.DiscountedPrice = decimal.Parse(httpRequest.Form["DiscountedPrice"]);
                product.Discount = string.IsNullOrEmpty(httpRequest.Form["Discount"]) ? 0 : decimal.Parse(httpRequest.Form["Discount"]);
                product.Description = httpRequest.Form["Description"];
                product.Quantity = int.Parse(httpRequest.Form["Quantity"]);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.products.Add(product);
                await db.SaveChangesAsync();

                return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex); 
            }
        }


        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.products.Count(e => e.ProductId == id) > 0;
        }
    }
}