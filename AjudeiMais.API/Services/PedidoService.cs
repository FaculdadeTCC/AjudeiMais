using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.PedidoModel;
using AjudeiMais.Data.Models.PedidoProdutoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.IO;
using System.Security.Claims;

namespace AjudeiMais.API.Services
{
	public class PedidoService
	{
		private readonly PedidoRepository _pedidoRepository;
		private readonly InstituicaoRepository _instituicaoRepository;
		private readonly UsuarioRepository _usuariosRepository;
		private readonly ProdutoRepository _produtoRepository;

		public PedidoService(PedidoRepository pedidoRepository, InstituicaoRepository instituicaoRepository, UsuarioRepository usuarioRepository, ProdutoRepository produtoRepository)
		{
			_pedidoRepository = pedidoRepository;
			_instituicaoRepository = instituicaoRepository;
			_usuariosRepository = usuarioRepository;
			_produtoRepository = produtoRepository;

		}

		public async Task<ApiResponse<List<GetPedidoDTO>>> GetAllAsync()
		{
			var pedidos = await _pedidoRepository.GetAllAsync();

			if (pedidos == null)
			{
				return new ApiResponse<List<GetPedidoDTO>>
				{
					Success = false,
					Message = "Pedidos não encontrado",
					Type = "NotFound"
				};
			}

			var pedidosDTO = pedidos.Select(pedido => new GetPedidoDTO
			{
				Pedido_ID = pedido.Pedido_ID,
				Pedido_GUID = pedido.GUID,
				Usuario_GUID = pedido.Usuario?.GUID,
				Instituicao_GUID = pedido.Instituicao?.GUID,

				UsuarioContato = pedido.Usuario?.Telefone,
				InstituicaoContato = pedido.Instituicao?.Telefone,

				UsuarioEmail = pedido.Usuario?.Email,
				InstituicaoEmail = pedido.Instituicao?.Email,

				Produto = new ProdutoGetDTO
				{
					Produto_ID = pedido.Produto.Produto_ID,
					Guid = pedido.Produto.Guid,
					Nome = pedido.Produto.Nome,
					Descricao = pedido.Produto.Descricao,
					Condicao = pedido.Produto.Condicao,
					Validade = pedido.Produto.Validade,
					Quantidade = pedido.Produto.Quantidade,
					Peso = pedido.Produto.Peso,
					Disponivel = pedido.Produto.Disponivel,
					Habilitado = pedido.Produto.Habilitado,
					Excluido = pedido.Produto.Excluido,
					DataCriacao = pedido.Produto.DataCriacao,
					DataUpdate = pedido.Produto.DataUpdate,
					UnidadeMedida = pedido.Produto.UnidadeMedida,

					ProdutoImagens = pedido.Produto.ProdutoImagens
				}
			}).ToList();

			return new ApiResponse<List<GetPedidoDTO>>
			{
				Success = true,
				Message = "Pedidos encontrados com sucesso",
				Data = pedidosDTO
			};
		}

		public async Task<ApiResponse<List<GetPedidoDTO>>> GetItensAsync()
		{
			var pedidos = await _pedidoRepository.GetAllAsync();

			if (pedidos == null)
			{
				return new ApiResponse<List<GetPedidoDTO>>
				{
					Success = false,
					Message = "Pedidos não encontrado",
					Type = "NotFound"
				};
			}

			var pedidosDTO = pedidos.Select(pedido => new GetPedidoDTO
			{
				Pedido_ID = pedido.Pedido_ID,
				Pedido_GUID = pedido.GUID,
				Usuario_GUID = pedido.Usuario?.GUID,
				Instituicao_GUID = pedido.Instituicao?.GUID,

				UsuarioContato = pedido.Usuario?.Telefone,
				InstituicaoContato = pedido.Instituicao?.Telefone,

				UsuarioEmail = pedido.Usuario?.Email,
				InstituicaoEmail = pedido.Instituicao?.Email,

				Produto = new ProdutoGetDTO
				{
					Produto_ID = pedido.Produto.Produto_ID,
					Guid = pedido.Produto.Guid,
					Nome = pedido.Produto.Nome,
					Descricao = pedido.Produto.Descricao,
					Condicao = pedido.Produto.Condicao,
					Validade = pedido.Produto.Validade,
					Quantidade = pedido.Produto.Quantidade,
					Peso = pedido.Produto.Peso,
					Disponivel = pedido.Produto.Disponivel,
					Habilitado = pedido.Produto.Habilitado,
					Excluido = pedido.Produto.Excluido,
					DataCriacao = pedido.Produto.DataCriacao,
					DataUpdate = pedido.Produto.DataUpdate,
					UnidadeMedida = pedido.Produto.UnidadeMedida,

					ProdutoImagens = pedido.Produto.ProdutoImagens
				}
			}).ToList();

			return new ApiResponse<List<GetPedidoDTO>>
			{
				Success = true,
				Message = "Pedidos encontrados com sucesso",
				Data = pedidosDTO
			};
		}

		public async Task<ApiResponse<GetPedidoDTO>> GetByIdAsync(int id)
		{
			var pedido = await _pedidoRepository.GetByIdAsync(id);

			if (pedido == null)
			{
				return new ApiResponse<GetPedidoDTO>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			var pedidoDTO = new GetPedidoDTO
			{
				Pedido_ID = pedido.Pedido_ID,
				Pedido_GUID = pedido.GUID,
				Usuario_GUID = pedido.Usuario?.GUID,
				Instituicao_GUID = pedido.Instituicao?.GUID,

				UsuarioContato = pedido.Usuario?.Telefone,
				InstituicaoContato = pedido.Instituicao?.Telefone,

				UsuarioEmail = pedido.Usuario?.Email,
				InstituicaoEmail = pedido.Instituicao?.Email,

				Produto = new ProdutoGetDTO
				{
					Produto_ID = pedido.Produto.Produto_ID,
					Guid = pedido.Produto.Guid,
					Nome = pedido.Produto.Nome,
					Descricao = pedido.Produto.Descricao,
					Condicao = pedido.Produto.Condicao,
					Validade = pedido.Produto.Validade,
					Quantidade = pedido.Produto.Quantidade,
					Peso = pedido.Produto.Peso,
					Disponivel = pedido.Produto.Disponivel,
					Habilitado = pedido.Produto.Habilitado,
					Excluido = pedido.Produto.Excluido,
					DataCriacao = pedido.Produto.DataCriacao,
					DataUpdate = pedido.Produto.DataUpdate,
					UnidadeMedida = pedido.Produto.UnidadeMedida,

					ProdutoImagens = pedido.Produto.ProdutoImagens
				}
			};

			return new ApiResponse<GetPedidoDTO>
			{
				Success = true,
				Message = "Pedido encontrado com sucesso",
				Data = pedidoDTO
			};
		}


		public async Task<ApiResponse<GetPedidoDTO>> GetByGUIDAsync(string GUID)
		{
			var pedido = await _pedidoRepository.GetByGUIDAsync(GUID);

			if (pedido == null)
			{
				return new ApiResponse<GetPedidoDTO>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			var pedidoDTO = new GetPedidoDTO
			{
				Pedido_ID = pedido.Pedido_ID,
				Pedido_GUID = pedido.GUID,
				Usuario_GUID = pedido.Usuario?.GUID,
				Instituicao_GUID = pedido.Instituicao?.GUID,

				UsuarioContato = pedido.Usuario?.Telefone,
				InstituicaoContato = pedido.Instituicao?.Telefone,

				UsuarioEmail = pedido.Usuario?.Email,
				InstituicaoEmail = pedido.Instituicao?.Email,

				Produto = new ProdutoGetDTO
				{
					Produto_ID = pedido.Produto.Produto_ID,
					Guid = pedido.Produto.Guid,
					Nome = pedido.Produto.Nome,
					Descricao = pedido.Produto.Descricao,
					Condicao = pedido.Produto.Condicao,
					Validade = pedido.Produto.Validade,
					Quantidade = pedido.Produto.Quantidade,
					Peso = pedido.Produto.Peso,
					Disponivel = pedido.Produto.Disponivel,
					Habilitado = pedido.Produto.Habilitado,
					Excluido = pedido.Produto.Excluido,
					DataCriacao = pedido.Produto.DataCriacao,
					DataUpdate = pedido.Produto.DataUpdate,
					UnidadeMedida = pedido.Produto.UnidadeMedida,

					ProdutoImagens = pedido.Produto.ProdutoImagens
				}
			};

			return new ApiResponse<GetPedidoDTO>
			{
				Success = true,
				Message = "Pedido encontrado com sucesso",
				Data = pedidoDTO
			};
		}


		public async Task<ApiResponse<string>> SaveOrUpdate(PedidoDTO dto)
		{
			try
			{
				var instituicao = await _instituicaoRepository.GetByGUID(dto.Instituicao_GUID);
				if (instituicao == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Instituição não encontrada"
					};
				}

				var usuario = await  _usuariosRepository.GetByGUID(dto.Usuario_GUID);
				if (usuario == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Usuário não encontrado"
					};
				}

				var produto = await  _produtoRepository.GetById(dto.Produto_ID);
				if (produto == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Produto não encontrado"
					};
				}

					if (dto.Pedido_ID == 0 || dto.Pedido_ID ==  null)
				{
					var pedido = new Pedido
					{
						Status = "Pendente",
						GUID = Guid.NewGuid().ToString(),
						DataCriacao = DateTime.UtcNow,
						DataUpdate = DateTime.UtcNow,
						Habilitado = true,
						Excluido = false,
						UsuarioContato = usuario.Telefone,
						InstituicaoContato = instituicao.Telefone,
						UsuarioEmail = usuario.Email,
						InstituicaoEmail = instituicao.Email,

						// Relacionamentos corretos
						Instituicao_ID = instituicao.Instituicao_ID,
						Usuario_ID = usuario.Usuario_ID,
						Produto_ID = produto.Produto_ID
					};
					await _pedidoRepository.AddAsync(pedido);
				}
				else
				{
					var pedido = new Pedido
					{
						Pedido_ID = dto.Pedido_ID,
						Status = "Pendente",
						GUID = Guid.NewGuid().ToString(),
						UsuarioContato = usuario.Telefone,
						InstituicaoContato = instituicao.Telefone,
						UsuarioEmail = usuario.Email,
						InstituicaoEmail = instituicao.Email,
						DataCriacao = DateTime.UtcNow,
						DataUpdate = DateTime.UtcNow,
						Habilitado = true,
						Excluido = false,

						// Relacionamentos corretos
						Instituicao_ID = instituicao.Instituicao_ID,
						Usuario_ID = usuario.Usuario_ID,
						Produto_ID = produto.Produto_ID
					};

					await _pedidoRepository.UpdateAsync(pedido);
				}


				return new ApiResponse<string>
				{
					Success = true,
					Message = "Pedido criado com sucesso"

				};
			}
			catch
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Erro ao criar pedido",
					Type = "Exception"
				};
			}
		}


		public async Task<ApiResponse<string>> Atualizar(Pedido pedido)
		{
			var existente = await _pedidoRepository.GetByIdAsync(pedido.Pedido_ID);

			if (existente == null)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			existente.Status = pedido.Status;
			existente.DataUpdate = DateTime.Now;
			existente.Habilitado = pedido.Habilitado;
			existente.Excluido = pedido.Excluido;

			await _pedidoRepository.UpdateAsync(existente);

			return new ApiResponse<string>
			{
				Success = true,
				Message = "Pedido atualizado com sucesso"
			};
		}

		public async Task<ApiResponse<string>> DeleteAsync(int id)
		{	
			var pedido = await _pedidoRepository.GetByIdAsync(id);

			if (pedido == null)
			{
				return new ApiResponse<string>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			pedido.Habilitado = false;
			pedido.Excluido = true;
			pedido.DataUpdate = DateTime.Now;

			await _pedidoRepository.DeleteAsync(pedido);

			return new ApiResponse<string>
			{
				Success = true,
				Message = "Pedido excluído com sucesso"
			};
		}
	}
}
