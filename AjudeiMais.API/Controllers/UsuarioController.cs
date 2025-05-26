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

        [HttpPost("AtualizarDadosPessoais")]
        public async Task<IActionResult> AtualizarDadosPessoais([FromForm] UsuarioDadosPessoaisDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Extrai as mensagens de erro do ModelState
                    var errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Erro de validação nos dados enviados.",
                        Data = errors
                    });
                }

                var result = await _service.AtualizarDadosPessoais(dto);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro interno ao atualizar dados pessoais. Por favor, tente novamente."
                });
            }
        }
        
        [HttpPost("AtualizarEndereco")]
        public async Task<IActionResult> AtualizarEndereco([FromForm] UsuarioEnderecoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Extrai as mensagens de erro do ModelState
                    var errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Erro de validação nos dados enviados.",
                        Data = errors
                    });
                }

                var result = await _service.AtualizarEndereco(dto);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro interno ao atualizar endereço. Por favor, tente novamente."
                });
            }
        }

        [HttpPost("AtualizarSenha")]
        public async Task<IActionResult> AtualizarSenha([FromForm] UsuarioSenhaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Extrai as mensagens de erro do ModelState
                    var errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Erro de validação nos dados enviados.",
                        Data = errors
                    });
                }

                var result = await _service.AtualizarSenha(dto);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro interno ao atualizar endereço. Por favor, tente novamente."
                });
            }
        }

        [HttpDelete("{guid}")] // Removemos "Delete" da rota, o GUID já é o identificador
        public async Task<IActionResult> ExcluirUsuario(string guid) // Renomeei para ser mais descritivo
        {
            try
            {
                var result = await _service.Delete(guid); // Adapte seu serviço para aceitar apenas o GUID

                if (result.Success)
                {
                    // Para exclusão bem-sucedida, 200 OK com uma mensagem ou 204 No Content são comuns.
                    return Ok(result);
                }
                else
                {
                    // Dependendo do motivo da falha, pode ser NotFound, Forbidden, etc.
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno ao excluir o usuário. Por favor, tente novamente."
                });
            }
        }

        [HttpPost("VerificarSenha")] // Removemos "Delete" da rota, o GUID já é o identificador
        public async Task<IActionResult> VerificarSenha([FromForm] UsuarioExcluirContaDTO usuario)
        {
            try
            {

                var result = await _service.VerificarSenha(usuario);

                if (result.Success)
                {
                    // Para exclusão bem-sucedida, 200 OK com uma mensagem ou 204 No Content são comuns.
                    return Ok(result);
                }
                else
                {
                    // Dependendo do motivo da falha, pode ser NotFound, Forbidden, etc.
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno ao comparar senha. Por favor, tente novamente."
                });
            }
        }
    }
}
