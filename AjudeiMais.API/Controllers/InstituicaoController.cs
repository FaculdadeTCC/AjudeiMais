﻿using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstituicaoController : ControllerBase
    {
        private readonly InstituicaoService _instituicaoService;

        public InstituicaoController(InstituicaoService instituicaoService)
        {
            _instituicaoService = instituicaoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstituicoes()
        {
            try
            {
                var instituicoes = await _instituicaoService.GetAll();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Instituições carregadas com sucesso.",
                    Data = instituicoes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao buscar instituições.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("GetByGUID/{GUID}")]
        public async Task<IActionResult> GetInstituicaoByGuid(string GUID)
        {
            try
            {
                var instituicao = await _instituicaoService.GetByGUID(GUID);
                return Ok(instituicao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Ativos")]
        public async Task<IActionResult> GetInstituicoesAtivas()
        {
            try
            {
                var instituicoes = await _instituicaoService.GetItens();
                return Ok(instituicoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SaveOrUpdate([FromForm] InstituicaoPostDTO model)
        {
            try
            {
                await _instituicaoService.SaveOrUpdate(model);

                var response = new ApiResponse<object>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = model.Instituicao_ID == 0 ? "Instituição criada com sucesso." : "Instituição atualizada com sucesso.",
                    Data = model
                };

                if (model.Instituicao_ID == 0)
                {
                    return CreatedAtAction(nameof(GetInstituicoes), new { id = model.Instituicao_ID }, response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao salvar ou atualizar a instituição.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("AtualizarDados")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizarDados(AtualizarDadosDTO model)
        {
            try
            {
                await _instituicaoService.AtualizarDadosInstituicaoAsync(model);

                var response = new ApiResponse<object>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Dados da instituição atualizados com sucesso.",
                    Data = model
                };

                if (model.GUID == null)
                {
                    return CreatedAtAction(nameof(GetInstituicoes), new { Guid = model.GUID }, response);
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao atualizar os dados da instituição.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("AtualizarSenha")]
        public async Task<IActionResult> AtualizarSenha([FromBody] InstituicaoSenhaDTO dto)
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

                var result = await _instituicaoService.AtualizarSenha(dto);

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

        [HttpPost("ValidarSenha")]
        public async Task<IActionResult> ValidarSenha([FromBody] InstituicaoValidarSenhaDTO dto)
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

                var result = await _instituicaoService.ValidarSenha(dto);

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

        [HttpDelete("{GUID}")]
        public async Task<IActionResult> Delete(string GUID) // Renomeei para ser mais descritivo
        {
            try
            {
                var result = await _instituicaoService.Delete(GUID); // Adapte seu serviço para aceitar apenas o GUID

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
                    Message = "Ocorreu um erro interno ao excluir a instituição. Por favor, tente novamente."
                });
            }
        }


    }
}
