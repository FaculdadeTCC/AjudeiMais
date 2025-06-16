using System.Globalization;
using AjudeiMais.API.DTO;
using AjudeiMais.API.Models;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _produtoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ProdutoImagemService _produtoImagemService;
        private readonly ILogger<ProdutoService> _logger;

        public ProdutoService(
            ProdutoRepository produtoRepository,
            ProdutoImagemService produtoImagemService,
            UsuarioRepository usuarioRepository,
            ILogger<ProdutoService> logger)
        {
            _produtoRepository = produtoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoImagemService = produtoImagemService;
            _logger = logger;
        }

        //        /// <summary>
        //        /// Busca um produto pelo seu ID.
        //        /// </summary>
        //        /// <param name="id">ID do produto.</param>
        //        /// <returns>O produto encontrado.</returns>
        //        public async Task<Produto> GetById(int id)
        //        {
        //            try
        //            {
        //                var produto = await _produtoRepository.GetById(id);
        //                return produto;
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar produtos por ID");
        //                throw new Exception("Erro ao buscar produtos.");
        //            }
        //        }

        //        /// <summary>
        //        /// Busca todos os produtos.
        //        /// </summary>
        //        /// <returns>Uma coleção de todos os produtos.</returns>
        //        public async Task<IEnumerable<Produto>> GetAll()
        //        {
        //            try
        //            {
        //                return await _produtoRepository.GetAll();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar todos os produtos");
        //                throw new Exception("Erro ao buscar produtos.");
        //            }
        //        }

        //        /// <summary>
        //        /// Busca todos os produtos ativos (habilitados e não excluídos).
        //        /// </summary>
        //        /// <returns>Uma coleção de produtos ativos.</returns>
        //        public async Task<IEnumerable<Produto>> GetItens()
        //        {
        //            try
        //            {
        //                return await _produtoRepository.GetItens();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar todos os produtos");
        //                throw new Exception("Erro ao buscar produtos.");
        //            }
        //        }
        public async Task<ApiResponse<ProdutoPostDto>> SaveOrUpdate(ProdutoPostDto model, List<IFormFile> imagens)
        {
            try
            {
                Produto produto;

                var usuario = await _usuarioRepository.GetByGUID(model.Usuario_GUID);
                if (usuario == null)
                {
                    return new ApiResponse<ProdutoPostDto>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado para associar ao produto."
                    };
                }

                if (model.Produto_ID == 0 || model.Produto_ID == null)
                {
                    // CRIAÇÃO
                    produto = new Produto
                    {
                        Nome = model.Nome,
                        Guid = Guid.NewGuid().ToString(),
                        Descricao = model.Descricao,
                        Condicao = model.Condicao,
                        Validade = model.Validade ?? null,
                        Quantidade = model.Quantidade,
                        Peso = model.Peso,
                        Disponivel = true,
                        UnidadeMedida = model.UnidadeMedida,
                        Usuario_ID = usuario.Usuario_ID,
                        CategoriaProduto_ID = model.CategoriaProduto_ID ?? 0,
                        DataCriacao = DateTime.Now,
                        Habilitado = true,
                        Excluido = false
                    };
                }
                else
                {
                    // ATUALIZAÇÃO
                    produto = await _produtoRepository.GetById((int)model.Produto_ID);

                    if (produto == null)
                    {
                        return new ApiResponse<ProdutoPostDto>
                        {
                            Success = false,
                            Type = "error",
                            Message = "Produto não encontrado para atualização."
                        };
                    }

                    // Atualiza os campos apenas se novos valores forem fornecidos
                    produto.Nome = model.Nome ?? produto.Nome;
                    produto.Guid = model.Guid ?? produto.Guid;
                    produto.Descricao = model.Descricao ?? produto.Descricao;
                    produto.Condicao = model.Condicao ?? produto.Condicao;
                    produto.Validade = model.Validade ?? produto.Validade;
                    produto.Quantidade = model.Quantidade != null ? model.Quantidade : produto.Quantidade;
                    produto.Peso = model.Peso != null ? model.Peso : produto.Peso;
                    produto.Disponivel = model.Disponivel ?? produto.Disponivel;
                    produto.UnidadeMedida = model.UnidadeMedida ?? produto.UnidadeMedida;
                    produto.Usuario_ID = usuario.Usuario_ID;
                    produto.CategoriaProduto_ID = model.CategoriaProduto_ID ?? produto.CategoriaProduto_ID;
                    produto.DataUpdate = DateTime.Now;
                }

                // Salva ou atualiza no banco
                await _produtoRepository.SaveOrUpdate(produto);


                var retorno = new ProdutoPostDto
                {
                    Produto_ID = produto.Produto_ID,
                    CategoriaProduto_ID = produto.CategoriaProduto_ID,
                    Nome = produto.Nome,
                    DataCriacao = produto.DataCriacao,
                    DataUpdate = produto.DataUpdate
                };

                return new ApiResponse<ProdutoPostDto>
                {
                    Success = true,
                    Type = "success",
                    Message = (model.Produto_ID == 0) ? "Produto cadastrado com sucesso." : "Produto atualizado com sucesso.",
                    Data = retorno
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar o produto.");

                return new ApiResponse<ProdutoPostDto>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao processar sua solicitação. Tente novamente mais tarde ou entre em contato com o suporte."
                };
            }
        }

        /// <summary>
        /// Busca todos os produtos pertencentes a um usuário específico.
        /// </summary>
        /// <param name="guid">GUID do usuário.</param>
        /// <returns>ApiResponse contendo a coleção de produtos do usuário.</returns>
        public async Task<ApiResponse<IEnumerable<ProdutoResponseDto>>> GetProdutosByUsuarioGuid(string guid)
        {
            try
            {
                var produtos = await _produtoRepository.GetByUsuarioGuid(guid);


                var produtosReturn = produtos.Select(p => new ProdutoResponseDto
                {
                    Produto_ID = p.Produto_ID,
                    Guid = p.Guid.ToString(),
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Condicao = p.Condicao,
                    Validade = p.Validade,
                    Quantidade = p.Quantidade,
                    Peso = (double)p.Peso,
                    Disponivel = p.Disponivel,
                    Habilitado = p.Habilitado,
                    Excluido = p.Excluido,
                    DataCriacao = p.DataCriacao,
                    DataUpdate = p.DataUpdate,
                    UnidadeMedida = p.UnidadeMedida,
                    Imagens = p.ProdutoImagens?.Select(i => i.Imagem).ToList(),

                    CategoriaProduto = p.CategoriaProduto != null ? new CategoriaProdutoDto
                    {
                        CategoriaProduto_ID = p.CategoriaProduto.CategoriaProduto_ID,
                        Nome = p.CategoriaProduto.Nome,
                        Icone = p.CategoriaProduto.Icone
                    } : null,

                    Usuario = p.Usuario != null ? new UsuarioDto
                    {
                        Usuario_ID = p.Usuario.Usuario_ID,
                        NomeCompleto = p.Usuario.NomeCompleto,
                        Email = p.Usuario.Email,
                        Guid = p.Usuario.GUID.ToString(),
                        Telefone = p.Usuario.Telefone,
                        FotoDePerfil = p.Usuario.FotoDePerfil,
                        Cidade = p.Usuario.Cidade,
                        Estado = p.Usuario.Estado
                    } : null
                }).ToList();


                return new ApiResponse<IEnumerable<ProdutoResponseDto>>
                {
                    Success = true,
                    Message = "Produtos recuperados com sucesso.",
                    Data = produtosReturn
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProdutoResponseDto>>
                {
                    Success = false,
                    Message = $"Erro ao recuperar produtos: {ex.Message}",
                    Data = null
                };
            }
        }
        public async Task<ApiResponse<Produto>> GetByGuid(string guid)
        {
            try
            {
                var produto = await _produtoRepository.GetByGuid(guid);

                if (produto != null)
                {
                    return new ApiResponse<Produto>
                    {
                        Success = true,
                        Message = "Produto recuperado com sucesso.",
                        Data = produto
                    };
                } else
                {
                    return new ApiResponse<Produto>
                    {
                        Success = false,
                        Message = $"O Guid informado não corresponde a nenhum produto",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Produto>
                {
                    Success = false,
                    Message = $"Erro ao recuperar produtos: {ex.Message}",
                    Data = null
                };
            }
        }

        /// <summary>
        /// Exclui logicamente um produto pelo seu GUID.
        /// </summary>
        public async Task<ApiResponse<object>> Delete(string guid)
        {
            try
            {
                var produto = await _produtoRepository.GetByGuid(guid);

                if (produto == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Produto não encontrado."
                    };
                }

                produto.Habilitado = false;
                produto.Excluido = true;
                produto.DataUpdate = DateTime.Now;

                await _produtoRepository.Delete(produto);

                return new ApiResponse<object>
                {
                    Success = true,
                    Type = "success",
                    Message = "Produto excluído com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = $"Não foi possível excluir o produto. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        /// <summary>
        /// Busca produtos próximos a uma determinada localização (latitude e longitude).
        /// </summary>
        /// <param name="lat">Latitude do ponto de referência.</param>
        /// <param name="lng">Longitude do ponto de referência.</param>
        /// <returns>Uma coleção de produtos próximos.</returns>
        public async Task<ApiResponse<IEnumerable<ProdutoGetDTO>>> GetProdutosProximos(double latInstituicao, double lngInstituicao)
        {
            var raioBuscaKm = 200;
            var produtosProximos = new List<Produto>();

            // 1. Pegar todos os usuários com latitude e longitude válidas
            var todosUsuarios = await _usuarioRepository.GetItens();

            var usuariosComLocalizacao = todosUsuarios
                .Where(u => !string.IsNullOrWhiteSpace(u.Latitude) && !string.IsNullOrWhiteSpace(u.Longitude))
                .ToList();

            // 2. Comparar distância do usuário até a instituição
            foreach (var usuario in usuariosComLocalizacao)
            {
                double latUsuario = double.Parse(usuario.Latitude, CultureInfo.InvariantCulture);
                double lngUsuario = double.Parse(usuario.Longitude, CultureInfo.InvariantCulture);


                double distancia = Helpers.CalcularDistancia(latUsuario, lngUsuario, latInstituicao, lngInstituicao);

                if (distancia <= raioBuscaKm)
                {
                    // 3. Se estiver dentro do raio, pega os produtos desse usuário
                    var produtosDoUsuario = await _produtoRepository.GetByUsuarioGuid(usuario.GUID);
                    produtosProximos.AddRange(produtosDoUsuario.Where(p => !p.Excluido));
                }
            }

            // 4. Monta os DTOs para retorno
            var produtosDto = produtosProximos.Select(p => new ProdutoGetDTO
            {
                Produto_ID = p.Produto_ID,
                Guid = p.Guid,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Condicao = p.Condicao,
                Validade = p.Validade,
                Quantidade = p.Quantidade,
                Peso = p.Peso,
                Disponivel = p.Disponivel,
                Habilitado = p.Habilitado,
                Excluido = p.Excluido,
                DataCriacao = p.DataCriacao,
                DataUpdate = p.DataUpdate,
                UnidadeMedida = p.UnidadeMedida,
                ProdutoImagens = p.ProdutoImagens,
                Usuario = new UsuarioResumoDTO
                {
                    NomeCompleto = p.Usuario?.NomeCompleto ?? "",
                    Email = p.Usuario?.Email ?? "",
                    GUID = p.Usuario?.GUID ?? "",
                    Cidade = p.Usuario?.Cidade ?? "",
                    Estado = p.Usuario?.Estado ?? "",
                    FotoDePerfil = p.Usuario?.FotoDePerfil ?? "",
                    Telefone = p.Usuario?.Telefone ?? "",
                    TelefoneFixo = p.Usuario?.TelefoneFixo ?? "",
                },
                CategoriaProduto = new CategoriaProdutoDTO
                {
                    CategoriaProduto_ID = p.CategoriaProduto?.CategoriaProduto_ID ?? 0,
                    Nome = p.CategoriaProduto?.Nome ?? "",
                    Icone = p.CategoriaProduto?.Icone ?? "",
                }
            });

            return new ApiResponse<IEnumerable<ProdutoGetDTO>>
            {
                Success = true,
                Data = produtosDto,
                Message = "Produtos dos usuários mais próximos da instituição retornados com sucesso."
            };
        }


    }
}