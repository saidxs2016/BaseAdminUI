using Core.Services.CacheService.MicrosoftInMemory;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCacheService _memoryCacheService;

        public HomeController(ILogger<HomeController> logger, IMemoryCacheService memoryCacheService)
        {
            _logger = logger;
            _memoryCacheService = memoryCacheService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            await Task.Delay(10);

            var getAllMemory = _memoryCacheService.GetByPattern($"^super-admin\\.");

            return Ok(getAllMemory);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}