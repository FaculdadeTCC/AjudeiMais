using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.PedidoModel;
using AjudeiMais.Data.Models.PedidoProdutoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
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

		public async Task<ApiResponse<List<Pedido>>> GetAllAsync()
		{
			var pedidos = await _pedidoRepository.GetAllAsync();
			return new ApiResponse<List<Pedido>>
			{
				Success = true,
				Message = "Pedidos encontrados com sucesso",
				Data = pedidos
			};
		}

		public async Task<ApiResponse<Pedido>> GetByIdAsync(int id)
		{
			var pedido = await _pedidoRepository.GetByIdAsync(id);

			if (pedido == null)
			{
				return new ApiResponse<Pedido>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			return new ApiResponse<Pedido>
			{
				Success = true,
				Message = "Pedido encontrado com sucesso",
				Data = pedido
			};
		}

		public async Task<ApiResponse<Pedido>> GetByGUIDAsync(string GUID)
		{
			var pedido = await _pedidoRepository.GetByGUIDAsync(GUID);

			if (pedido == null)
			{
				return new ApiResponse<Pedido>
				{
					Success = false,
					Message = "Pedido não encontrado",
					Type = "NotFound"
				};
			}

			return new ApiResponse<Pedido>
			{
				Success = true,
				Message = "Pedido encontrado com sucesso",
				Data = pedido
			};
		}

		public async Task<ApiResponse<string>> SaveOrUpdate(CriarPedidoDTO dto)
		{
			try
			{
				var instituicao = _instituicaoRepository.GetByGUID(dto.Instituicao_GUID);
				if (instituicao == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Instituição não encontrada"
					};
				}

				var usuario = _usuariosRepository.GetByGUID(dto.Usuario_GUID);
				if (usuario == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Usuário não encontrado"
					};
				}

				var produto = _produtoRepository.GetById(dto.Produto_ID);
				if (usuario == null)
				{
					return new ApiResponse<string>
					{
						Success = false,
						Message = "Produto não encontrado"
					};
				}
				var pedido = new Pedido
				{
					Habilitado = true,
					Excluido = false,
					Status = "Pendente",
					GUID = Guid.NewGuid().ToString(),
					DataCriacao = DateTime.UtcNow,
					DataUpdate = DateTime.UtcNow,
					Instituicao  = new Instituicao
					{
						GUID = instituicao.Result.GUID,
						Nome = instituicao.Result.Nome,
						Descricao = instituicao.Result.Descricao,
						Telefone = instituicao.Result.Telefone,
						Email = instituicao.Result.Email,
						FotoPerfil = instituicao.Result.FotoPerfil,
						Documento = instituicao.Result.Documento,
						Role = instituicao.Result.Role
					},
					Usuario = new Usuario
					{
						Usuario_ID = usuario.Result.Usuario_ID,
						NomeCompleto = usuario.Result.NomeCompleto,
						Documento = usuario.Result.Documento,
						Email = usuario.Result.Email,
						Senha = usuario.Result.Senha,
						GUID = usuario.Result.GUID,
						Role = usuario.Result.Role,
						CEP = usuario.Result.CEP,
						Rua = usuario.Result.Rua,
						Numero = usuario.Result.Numero,
						Complemento = usuario.Result.Complemento ?? "",
						Bairro = usuario.Result.Bairro,
						Cidade = usuario.Result.Cidade,
						Estado = usuario.Result.Estado,
						FotoDePerfil = usuario.Result.FotoDePerfil,
						Telefone = usuario.Result.Telefone ?? "",
						TelefoneFixo = usuario.Result.TelefoneFixo ?? "",
						Latitude = usuario.Result.Latitude ?? "",
						Longitude = usuario.Result.Longitude ?? "",
						Habilitado = usuario.Result.Habilitado,
						Excluido = usuario.Result.Excluido,
						DataCriacao = usuario.Result.DataCriacao,
						DataUpdate = usuario.Result.DataUpdate,
						Produtos = usuario.Result.Produtos,
					},
					Produto = new Produto
					{
						Produto_ID = produto.Result.Produto_ID,
						Guid = produto.Result.Guid,
						Nome = produto.Result.Nome,
						Descricao = produto.Result.Descricao,
						Condicao = produto.Result.Condicao,
						Validade = produto.Result.Validade,
						Quantidade = produto.Result.Quantidade,
						Peso = produto.Result.Peso,
						Disponivel = produto.Result.Disponivel,
						Habilitado = produto.Result.Habilitado,
						Excluido = produto.Result.Excluido,
						DataCriacao = produto.Result.DataCriacao,
						DataUpdate = produto.Result.DataUpdate,
						UnidadeMedida = produto.Result.UnidadeMedida,
						Usuario_ID = produto.Result.Usuario_ID,
						CategoriaProduto_ID = produto.Result.CategoriaProduto_ID,
						Usuario = null, // você pode mapear isso se quiser, como fez com o usuário principal
						CategoriaProduto = null, // idem, se quiser incluir o nome da categoria, etc.
						ProdutoImagens = produto.Result.ProdutoImagens
					}
					



				};

				await _pedidoRepository.AddAsync(pedido);

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

			await _pedidoRepository.DeleteAsync(pedido);

			return new ApiResponse<string>
			{
				Success = true,
				Message = "Pedido excluído com sucesso"
			};
		}
	}
}
