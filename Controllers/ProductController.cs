using API_JWTToken_Products.Context;
using API_JWTToken_Products.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_JWTToken_Products.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly productContext _context;

        public ProductController(productContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get() => Ok(await _context.Products.ToListAsync());


        [HttpGet("GetbyID{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> Post(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("UpdateProduct{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id)
                return BadRequest("Product ID mismatch");

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Product updated successfully",
                Product = existingProduct
            });
        }

        [HttpDelete("DeleteProduct{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Product deleted successfully" });
        }
    }
}

