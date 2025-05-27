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
        public async Task<ApiResponse<ProdutoDto>> SaveOrUpdate(ProdutoDto model, List<IFormFile> imagens)
        {
            try
            {
                Produto produto;

                if (model.Produto_ID == 0) // Lógica para CRIAÇÃO de um novo produto
                {
                    produto = new Produto
                    {
                        // Mapeia propriedades do DTO para a nova entidade Produto
                        Nome = model.Nome,
                        Descricao = model.Descricao,
                        Condicao = model.Condicao,
                        Validade = model.Validade,
                        Quantidade = model.Quantidade,
                        Peso = model.Peso,
                        Disponivel = model.Disponivel,
                        Usuario_ID = model.Usuario_ID,
                        CategoriaProduto_ID = model.CategoriaProduto_ID,
                        DataCriacao = DateTime.Now,
                        Habilitado = true,
                        Excluido = false
                    };
                }
                else // Lógica para ATUALIZAÇÃO de um produto existente
                {
                    // Tenta carregar o produto existente do repositório
                    produto = await _produtoRepository.GetById(model.Produto_ID);

                    if (produto == null)
                    {
                        return new ApiResponse<ProdutoDto>
                        {
                            Success = false,
                            Type = "error",
                            Message = "Produto não encontrado para atualização."
                        };
                    }

                    // Atualiza as propriedades do produto existente com base no DTO
                    produto.Nome = model.Nome;
                    produto.Descricao = model.Descricao;
                    produto.Condicao = model.Condicao;
                    produto.Validade = model.Validade;
                    produto.Quantidade = model.Quantidade;
                    produto.Peso = model.Peso;
                    produto.Disponivel = model.Disponivel;
                    // produto.Usuario_ID = model.Usuario_ID;
                    // produto.CategoriaProduto_ID = model.CategoriaProduto_ID;
                    produto.DataUpdate = DateTime.Now; // Define DataUpdate apenas na atualização
                    produto.Habilitado = model.Habilitado;
                    produto.Excluido = model.Excluido;
                }

                // Salva ou atualiza o produto no banco de dados
                await _produtoRepository.SaveOrUpdate(produto);

                // Após salvar/atualizar, o produto.Produto_ID deve estar preenchido (se for criação)
                // ou já ter seu valor (se for atualização).
                // Agora salva as imagens associadas ao produto salvo
                foreach (var imagem in imagens)
                {
                    var dto = new ProdutoImagemUploadDto
                    {
                        ProdutoId = produto.Produto_ID,
                        Imagem = imagem
                    };

                    await _produtoImagemService.SaveOrUpdate(dto);
                }

                ProdutoDto produtoRetorno = new ProdutoDto
                {
                    Produto_ID = produto.Produto_ID,
                    CategoriaProduto_ID = produto.CategoriaProduto_ID,
                    Nome = produto.Nome,
                    DataCriacao = produto.DataCriacao,
                    DataUpdate = produto.DataUpdate
                };

                return new ApiResponse<ProdutoDto>
                {
                    Success = true,
                    Type = "success",
                    Message = (model.Produto_ID == 0) ? "Produto cadastrado com sucesso." : "Produto atualizado com sucesso.",
                    Data = produtoRetorno
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar o produto.");

                return new ApiResponse<ProdutoDto>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao salvar ou atualizar o produto. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        //        /// <summary>
        //        /// Busca todos os produtos pertencentes a um usuário específico.
        //        /// </summary>
        //        /// <param name="id">ID do usuário.</param>
        //        /// <returns>Uma coleção de produtos do usuário.</returns>
        //        public async Task<IEnumerable<Produto>> GetByUsuarioId(int id)
        //        {
        //            try
        //            {
        //                return await _produtoRepository.GetByUsuarioId(id);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao excluir o produto com ID {ProdutoId}", id);
        //                throw new Exception("Erro ao excluir o produto.");
        //            }
        //        }

        //        /// <summary>
        //        /// Exclui logicamente um produto pelo seu ID.
        //        /// </summary>
        //        /// <param name="id">ID do produto a ser excluído.</param>
        //        public async Task<ApiResponse<object>> Delete(int id)
        //        {
        //            try
        //            {
        //                var produto = await GetById(id);

        //                if (produto == null)
        //                {
        //                    return new ApiResponse<object>
        //                    {
        //                        Success = false,
        //                        Type = "error",
        //                        Message = "Produto não encontrado."
        //                    };
        //                }

        //                produto.Habilitado = false;
        //                produto.Excluido = true;
        //                produto.DataUpdate = DateTime.Now;

        //                await _produtoRepository.Delete(produto);

        //                return new ApiResponse<object>
        //                {
        //                    Success = true,
        //                    Type = "success",
        //                    Message = "Produto excluído com sucesso."
        //                };
        //            }
        //            catch (Exception ex)
        //            {
        //                return new ApiResponse<object>
        //                {
        //                    Success = false,
        //                    Type = "error",
        //                    Message = $"Não foi possível excluir o produto. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
        //                };
        //            }
        //        }

        //        /// <summary>
        //        /// Busca produtos próximos a uma determinada localização (latitude e longitude).
        //        /// </summary>
        //        /// <param name="lat">Latitude do ponto de referência.</param>
        //        /// <param name="lng">Longitude do ponto de referência.</param>
        //        /// <returns>Uma coleção de produtos próximos.</returns>
        //        public async Task<IEnumerable<Produto>> GetProdutosProximos(double lat, double lng)
        //        {
        //            var todosUsuarios = await _usuarioRepository.GetItens();
        //            var raioBuscaKm = 30;
        //            var produtosProximos = new List<Produto>();

        //            var usuariosProximos = todosUsuarios
        //                .Where(u => !string.IsNullOrEmpty(u.Latitude) && !string.IsNullOrEmpty(u.Longitude) && u.Produtos != null && u.Produtos.Any(p => !p.Excluido))
        //                .Where(u => Helpers.CalcularDistancia(lat, lng, double.Parse(u.Latitude, CultureInfo.InvariantCulture), double.Parse(u.Longitude, CultureInfo.InvariantCulture)) <= raioBuscaKm)
        //                .ToList();

        //            foreach (var usuario in usuariosProximos)
        //            {
        //                var produtosDoUsuario = await _produtoRepository.GetByUsuarioId(usuario.Usuario_ID);
        //                produtosProximos.AddRange(produtosDoUsuario.Where(p => !p.Excluido)); // Garante que apenas produtos não excluídos sejam adicionados
        //            }

        //            return produtosProximos;
        //        }
    }
}