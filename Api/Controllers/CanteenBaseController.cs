using ApiDomain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/canteen")]
    [ApiController]
    public class CanteenBaseController : ControllerBase //общая логика, зависимость и свойства всех контроллеров - не написан
    {
        private readonly ApiListContext _context;
        public CanteenBaseController(ApiListContext applicationDBContext)
        {
            _context = applicationDBContext;
        }
    }
}
