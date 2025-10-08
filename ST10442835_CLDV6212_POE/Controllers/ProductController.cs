﻿using Microsoft.AspNetCore.Mvc;
using ST10442835_CLDV6212_POE.Models;
using ST10442835_CLDV6212_POE.Services;

namespace ST10442835_CLDV6212_POE.Controllers
{
    public class ProductController : Controller
    {
        private readonly IFunctionApi _api;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IFunctionApi api, ILogger<ProductController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _api.GetProductsAsync();
            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(product);
            try
            {
                var saved = await _api.CreateProductAsync(product, imageFile);
                TempData["Success"] = $"Product '{saved.ProductName}' created successfully with price {saved.Price:C}!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", $"Error creating product: {ex.Message}");
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();
            var product = await _api.GetProductAsync(id);
            return product is null ? NotFound() : View(product);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
        {

            if (!ModelState.IsValid) return View(product);
            try
            {
                var updated = await _api.UpdateProductAsync(product.RowKey, product, imageFile);
                TempData["Success"] = $"Product '{updated.ProductName}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                ModelState.AddModelError("", $"Error updating product: {ex.Message}");
                return View(product);
            }
        }

        [HttpPost("Product/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid product ID!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _api.DeleteProductAsync(id);
                TempData["Success"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting product: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}