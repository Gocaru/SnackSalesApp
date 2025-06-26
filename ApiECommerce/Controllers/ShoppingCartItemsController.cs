using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartItemsController : ControllerBase
    {

        private readonly AppDbContext _appDbContext;

        public ShoppingCartItemsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // GET: api/ShoppingCartItems/1
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var user = await _appDbContext.Users.FindAsync(userId);

            if (user is null)
            {
                return NotFound($"O utilizador com o id = {userId} não foi encontrado");
            }

            var shoppingCartItems = await (from s in _appDbContext.ShoppingCartItems.Where(s => s.ClientId == userId)
                                           join p in _appDbContext.Products on s.ProductId equals p.Id
                                           select new
                                           {
                                               Id = s.Id,
                                               Price = s.UnitPrice,
                                               Total = s.Total,
                                               Quantity = s.Quantity,
                                               ProductId = p.Id,
                                               ProductName = p.Name,
                                               UrlImage = p.UrlImage
                                           }).ToListAsync();

            return Ok(shoppingCartItems);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ShoppingCartItem shoppingCartItem)
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;  //Obtem o ID do utilizador a partir do token (a forma segura)
                var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

                if (user is null)
                {
                    return Unauthorized("User not found or invalid token.");
                }
                var authenticatedUserId = user.Id;

                // Ignorar o ClientId que vem do frontend e usar sempre o ID seguro
                var shoppingCart = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
                    s.ProductId == shoppingCartItem.ProductId &&
                    s.ClientId == authenticatedUserId); // <-- Usar o ID seguro

                if (shoppingCart != null)
                {
                    shoppingCart.Quantity += shoppingCartItem.Quantity;
                    shoppingCart.Total = shoppingCart.UnitPrice * shoppingCart.Quantity;
                }
                else
                {
                    var product = await _appDbContext.Products.FindAsync(shoppingCartItem.ProductId);

                    var cart = new ShoppingCartItem()
                    {
                        ClientId = authenticatedUserId,
                        ProductId = shoppingCartItem.ProductId,
                        UnitPrice = shoppingCartItem.UnitPrice,
                        Quantity = shoppingCartItem.Quantity,
                        Total = (product!.Price) * (shoppingCartItem.Quantity)
                    };

                    _appDbContext.ShoppingCartItems.Add(cart);
                }

                await _appDbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }

        /*[Authorize]
        [HttpPut("[action]{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IncreaseQuantity(int productId)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var shoppingCartItem = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
            s.ClientId == user.Id && s.ProductId == productId);

            if (shoppingCartItem != null)
            {
                shoppingCartItem.Quantity += 1;
                shoppingCartItem.Total = shoppingCartItem.UnitPrice * shoppingCartItem.Quantity;
                _appDbContext.Update(shoppingCartItem);
                await _appDbContext.SaveChangesAsync();
                return Ok("Foi aumentada a quantidade com sucesso");
            }
            else
            {
                return NotFound("Nenhum item encontrado no carrinho");
            }
        }

        [Authorize]
        [HttpPut("[action]{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DecreaseQuantity(int productId)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var shoppingCartItem = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
            s.ClientId == user.Id && s.ProductId == productId);

            if (shoppingCartItem != null)
            {
                if(shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity -= 1;
                }
                else
                {
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                    await _appDbContext.SaveChangesAsync();
                    return Ok("Item removido com sucesso");
                }

                shoppingCartItem.Total = shoppingCartItem.UnitPrice * shoppingCartItem.Quantity;
                _appDbContext.Update(shoppingCartItem);
                await _appDbContext.SaveChangesAsync();
                return Ok("Foi diminuida a quantidade com sucesso");
            }
            else
            {
                return NotFound("Nenhum item encontrado no carrinho");
            }
        }*/

        // PUT /api/ShoppingCartItems?produtoId = 1 & acao = "aumentar"
        // PUT /api/ShoppingCartItems?produtoId = 1 & acao = "diminuir"
        // PUT /api/ShoppingCartItems?produtoId = 1 & acao = "apagar"
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int productId, string action)
        {
            // Este codigo recupera o endereço de e-mail do user autenticado do token JWT decodificado,
            // Claims representa as declarações associadas ao user autenticado
            // Assim somente os users autenticados poderão aceder a este endpoint
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user is null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var shoppingCartItem = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
                                                   s.ClientId == user!.Id && s.ProductId == productId);

            if (shoppingCartItem != null)
            {
                if (action.ToLower() == "aumentar")
                {
                    shoppingCartItem.Quantity += 1;
                }
                else if (action.ToLower() == "diminuir")
                {
                    if (shoppingCartItem.Quantity > 1)
                    {
                        shoppingCartItem.Quantity -= 1;
                    }
                    else
                    {
                        _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                        await _appDbContext.SaveChangesAsync();
                        return Ok();
                    }
                }
                else if (action.ToLower() == "apagar")
                {
                    // Remove o item do carrinho
                    _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                    await _appDbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Ação Inválida. Use : 'aumentar', 'diminuir', ou 'apagar' para realizar uma ação");
                }

                shoppingCartItem.Total = shoppingCartItem.UnitPrice * shoppingCartItem.Quantity;
                await _appDbContext.SaveChangesAsync();
                return Ok($"Operacao : {action} realizada com sucesso");
            }
            else
            {
                return NotFound("Nenhum item encontrado no carrinho");
            }
        }

        /*[Authorize]
        [HttpDelete("{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int productId)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            var shoppingCartItem = await _appDbContext.ShoppingCartItems.FirstOrDefaultAsync(s =>
            s.ClientId == user.Id && s.ProductId == productId);

            if (shoppingCartItem != null)
            {
                _appDbContext.ShoppingCartItems.Remove(shoppingCartItem);
                await _appDbContext.SaveChangesAsync();
                return Ok("Item removido com sucesso");
            }
            else
            {
                return NotFound("Nenhum item encontrado no carrinho");
            }
        }*/
    }
}
