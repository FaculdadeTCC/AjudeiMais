using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.PedidoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PedidoController : ControllerBase
	{
		private readonly PedidoService _pedidoService;

		public PedidoController(PedidoService pedidoService)
		{
			_pedidoService = pedidoService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var response = await _pedidoService.GetAllAsync();
				return Ok(new ApiResponse<object>
				{
					Success = true,
					Type = "success",
					Message = "Pedidos recuperados com sucesso.",
					Data = response
				});
			}
			catch
			{
				return StatusCode(500, new ApiResponse<object>
				{
					Success = false,
					Type = "error",
					Message = "Erro interno ao buscar pedidos."
				});
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var pedido = await _pedidoService.GetByIdAsync(id);

				if (pedido == null)
				{
					return NotFound(new ApiResponse<object>
					{
						Success = false,
						Type = "not_found",
						Message = "Pedido não encontrado."
					});
				}

				return Ok(new ApiResponse<object>
				{
					Success = true,
					Type = "success",
					Message = "Pedido recuperado com sucesso.",
					Data = pedido
				});
			}
			catch
			{
				return StatusCode(500, new ApiResponse<object>
				{
					Success = false,
					Type = "error",
					Message = "Erro interno ao buscar pedido."
				});
			}
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CriarPedidoDTO pedido)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Type = "validation",
					Message = "Erro de validação nos dados enviados.",
					Data = errors
				});
			}

			try
			{
				var response = await _pedidoService.SaveOrUpdate(pedido);
				return Created("", new ApiResponse<object>
				{
					Success = true,
					Type = "success",
					Message = "Pedido criado com sucesso.",
					Data = response
				});
			}
			catch
			{
				return StatusCode(500, new ApiResponse<object>
				{
					Success = false,
					Type = "error",
					Message = "Erro interno ao criar pedido."
				});
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Pedido pedido)
		{
			if (id != pedido.Pedido_ID)
			{
				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Type = "validation",
					Message = "O ID do pedido não confere.",
					Data = new List<string> { "ID fornecido diferente do objeto enviado." }
				});
			}

			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

				return BadRequest(new ApiResponse<object>
				{
					Success = false,
					Type = "validation",
					Message = "Erro de validação nos dados enviados.",
					Data = errors
				});
			}

			try
			{
				var updated = await _pedidoService.Atualizar(pedido);

				if (!updated.Success)
				{
					return NotFound(new ApiResponse<object>
					{
						Success = false,
						Type = "not_found",
						Message = "Pedido não encontrado para atualização."
					});
				}

				return Ok(new ApiResponse<object>
				{
					Success = true,
					Type = "success",
					Message = "Pedido atualizado com sucesso.",
					Data = updated.Data
				});
			}
			catch
			{
				return StatusCode(500, new ApiResponse<object>
				{
					Success = false,
					Type = "error",
					Message = "Erro interno ao atualizar pedido."
				});
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var result = await _pedidoService.DeleteAsync(id);

				if (!result.Success)
				{
					return NotFound(new ApiResponse<object>
					{
						Success = false,
						Type = "not_found",
						Message = "Pedido não encontrado para exclusão."
					});
				}

				return Ok(new ApiResponse<object>
				{
					Success = true,
					Type = "success",
					Message = "Pedido excluído com sucesso."
				});
			}
			catch
			{
				return StatusCode(500, new ApiResponse<object>
				{
					Success = false,
					Type = "error",
					Message = "Erro interno ao excluir pedido."
				});
			}
		}
	}
}
