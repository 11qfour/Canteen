using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AccountController:HomeController //отвечает за бизнес-логику по авторизации и регистрации, отобрадение страниц
    {
        public AccountController(ILogger<HomeController> logger) : base(logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
