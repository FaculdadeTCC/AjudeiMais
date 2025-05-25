using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService usuarioService)
        {
            _service = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _service.GetAll();
                return Ok(usuarios);  // Retorna todos os usuários
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Retorna erro em caso de falha
            }
        }

        [HttpGet("GetByGUID/{GUID}")]
        public async Task<IActionResult> GetUsuarioByGuid(string GUID)
        {
            try
            {
                var usuarios = await _service.GetByGUID(GUID);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Retorna erro em caso de falha
            }
        }

        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetUsuarioByEmail(string email)
        {
            try
            {
                var usuario = await _service.GetByEmail(email);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Retorna erro em caso de falha
            }
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> GetUsuariosAtivos()
        {
            try
            {
                var usuariosAtivos = await _service.GetItens();

                return Ok(usuariosAtivos);  // Retorna apenas usuários ativos
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Retorna erro em caso de falha
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SaveOrUpdate([FromForm] UsuarioDTO model)
        {
            try
            {
                var result = await _service.SaveOrUpdate(model);

                if (result.Success) // Se a operação no serviço foi um sucesso
                {
                    return Ok(result); // Retorna 200 OK com a mensagem de sucesso
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
               var result = await _service.Delete(id);

                if (result.Success) // Se a operação no serviço foi um sucesso
                {
                    return Ok(result); // Retorna 200 OK com a mensagem de sucesso
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
