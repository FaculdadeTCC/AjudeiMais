using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class EnderecoService
    {
        private readonly EnderecoRepository _enderecoRepository;
        private readonly InstituicaoRepository _instituicaoRepository;
        private readonly ILogger<EnderecoService> _logger;

        public EnderecoService(EnderecoRepository enderecoRepository, InstituicaoRepository instituicaoRepository, ILogger<EnderecoService> logger)
        {
            _enderecoRepository = enderecoRepository;
            _instituicaoRepository = instituicaoRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<Endereco>> GetById(int id)
        {
            try
            {
                var endereco = await _enderecoRepository.GetById(id);

                return new ApiResponse<Endereco>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Endereço encontrado.",
                    Data = endereco
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar endereço");

                return new ApiResponse<Endereco>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao buscar endereço.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<Endereco>>> GetAll()
        {
            try
            {
                var enderecos = await _enderecoRepository.GetAll();

                return new ApiResponse<IEnumerable<Endereco>>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Endereços carregados com sucesso.",
                    Data = enderecos.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os endereços");

                return new ApiResponse<IEnumerable<Endereco>>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao buscar todos os endereços.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<Endereco>>> GetItens()
        {
            try
            {
                var itens = await _enderecoRepository.GetItens();

                return new ApiResponse<IEnumerable<Endereco>>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Endereços ativos carregados.",
                    Data = itens
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar endereços ativos");

                return new ApiResponse<IEnumerable<Endereco>>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao buscar endereços ativos.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<object>> SaveOrUpdate(Endereco model)
        {
            try
            {
                await _enderecoRepository.SaveOrUpdate(model);

                return new ApiResponse<object>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = model.Endereco_ID == 0 ? "Endereço criado com sucesso." : "Endereço atualizado com sucesso.",
                    Data = model
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar endereço");

                return new ApiResponse<object>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao salvar ou atualizar o endereço.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<object>> AtualizarEndereçoInstituicaoAsync(EnderecoDTO model)
        {
            try
            {
                var existente = await _enderecoRepository.GetById(model.Endereco_ID);
                var instituicao = await _instituicaoRepository.GetByGUID(existente.Instituicao.GUID);

                if (existente == null || instituicao == null)
                {
                    return new ApiResponse<object>
                    {
                        Success = false,
                        Type = "Erro",
                        Message = "Endereço ou instituição não encontrados.",
                        Errors = new List<string> { "Verifique os dados enviados." }
                    };
                }

                existente.CEP = model.CEP;
                existente.Rua = model.Rua;
                existente.Numero = model.Numero;
                existente.Complemento = model.Complemento;
                existente.Cidade = model.Cidade;
                existente.Estado = model.Estado;
                existente.Instituicao_ID = instituicao.Instituicao_ID;

                await _enderecoRepository.SaveOrUpdate(existente);

                return new ApiResponse<object>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Endereço da instituição atualizado com sucesso.",
                    Data = existente
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar dados da instituição");

                return new ApiResponse<object>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao atualizar o endereço da instituição.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<Endereco>> Delete(int Id)
        {
            try
            {
                var endereco = await _enderecoRepository.GetById(Id);

                endereco.Habilitado = false;
                endereco.Excluido = true;
                
                await _enderecoRepository.Delete(endereco);

                return new ApiResponse<Endereco>
                {
                    Success = true,
                    Type = "Sucesso",
                    Message = "Endereço removido com sucesso."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar endereço");

                return new ApiResponse<Endereco>
                {
                    Success = false,
                    Type = "Erro",
                    Message = "Erro ao deletar o endereço.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
