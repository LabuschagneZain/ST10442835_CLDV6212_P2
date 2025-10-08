using ST10442835_CLDV6212_POE.Models;
using ST10442835_CLDV6212_POE.Models.ViewModels;
using ST10442835_CLDV6212_POE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ST10442835_CLDV6212_POE.Controllers;
public class HomeController : Controller
{
    private readonly IFunctionApi _api;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IFunctionApi api, ILogger<HomeController> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var productsTask = _api.GetProductsAsync();
            var customersTask = _api.GetCustomersAsync();
            var ordersTask = _api.GetOrdersAsync();

            await Task.WhenAll(productsTask, customersTask, ordersTask);

            var products = productsTask.Result ?? new List<Product>();
            var customers = customersTask.Result ?? new List<Customer>();
            var orders = ordersTask.Result ?? new List<Order>();

            var vm = new HomeViewModel
            {
                FeaturedProducts = products.Take(8).ToList(),
                ProductCount = products.Count,
                CustomerCount = customers.Count,
                OrderCount = orders.Count
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data from Functions API.");
            TempData["Error"] = "Could not load dashboard data. Please try again.";
            return View(new HomeViewModel());
        }
    }

    public IActionResult Privacy() => View();
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

}






