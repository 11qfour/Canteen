using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class AccountController:HomeController //отвечает за бизнес-логику по авторизации и регистрации, отображение страниц - не написан
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
