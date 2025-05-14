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
        public async Task<IActionResult> SaveOrUpdate(Usuario model)
        {
            try
            {
                await _service.SaveOrUpdate(model);

                if (model.Usuario_ID == 0)  // Verifica se é uma criação
                {
                    return CreatedAtAction(nameof(GetUsuarios), new { id = model.Usuario_ID }, model);  // Retorna 201 para criação
                }
                else
                {
                    return Ok(model);  // Retorna 200 para atualização
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.Delete(id);
                return Ok();  // Retorna 200 para exclusão bem-sucedida
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}
