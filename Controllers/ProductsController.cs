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
        public async Task<IHttpActionResult> PutProduct(int id)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Invalid request. Expected multipart content.");
            }

            var root = HttpContext.Current.Server.MapPath("~/ProductImages");
            Directory.CreateDirectory(root);  // Ensure directory exists

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                var formData = provider.FormData;

                Product product = new Product
                {
                    ProductId = id,
                    Name = formData["Name"],
                    CategoryId = int.Parse(formData["CategoryId"]),
                    DiscountedPrice = decimal.Parse(formData["DiscountedPrice"]),
                    Discount = int.Parse(formData["Discount"]),
                    Description = formData["Description"],
                    Quantity = int.Parse(formData["Quantity"]),
                    ImgUrl = formData["ImgUrl"]
                };

                if (provider.FileData.Count > 0)
                {
                    var file = provider.FileData[0];
                    var extension = Path.GetExtension(file.Headers.ContentDisposition.FileName.Trim('"')).ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest("Invalid image file type.");
                    }

                    string fileName = Guid.NewGuid() + extension;
                    string filePath = Path.Combine(root, fileName);
                    File.Move(file.LocalFileName, filePath);
                    product.ImgUrl = "/ProductImages/" + fileName;
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != product.ProductId)
                {
                    return BadRequest("Product ID mismatch.");
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

                return Ok("Product updated successfully!");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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

        // GET: api/Products/ByName?name=ProductName
        [HttpGet]
        [Route("api/Products/ByName")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Product name cannot be empty.");
            }

            var product = await db.products.FirstOrDefaultAsync(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        // DELETE: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.products.Remove(product);
            await db.SaveChangesAsync();

            return Ok("Product deleted successfully!");
        }



        // GET: api/Products/5/Image
        [HttpGet]
        [Route("api/Products/{id:int}/Image")]
        public IHttpActionResult GetProductImage(int id)
        {
            var product = db.products.Find(id);
            if (product == null)
            {
                return NotFound(); 
            }

            
            string imagePath = HttpContext.Current.Server.MapPath(product.ImgUrl);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(); 
            }

           
            var imageFile = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(imageFile)
            };
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            return ResponseMessage(result);
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