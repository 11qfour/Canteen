using ApiDomain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApiListContext _context;
        public CartController(ApiListContext applicationDBContext)
        {
            _context = applicationDBContext;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var cart = _context.Cart.ToList();
            return Ok(cart);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var cart = _context.Cart.Find(id);
            if (cart == null)
            {
                return NotFound();
            }
            return Ok(cart);
        }
    }
}
