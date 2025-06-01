using System.ComponentModel.DataAnnotations.Schema;
using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.AvaliacaoModel;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.API.Services
{
    public class AvaliacaoService
    {
        private readonly AvaliacaoRepository _avaliacaoRepository;
        public AvaliacaoService(AvaliacaoRepository avaliacaoRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
        }

        public async Task<ApiResponse<AvaliacaoPostDTO>> SaveOrUpdate(AvaliacaoPostDTO dto)
        {
            try
            {
                Avaliacao entity;
                string successMessage;

                // Verifica se o DTO possui um ID válido para atualização
                if (dto.Avaliacao_ID > 0)
                {
                    // Tenta buscar a avaliação existente no banco de dados
                    entity = await _avaliacaoRepository.GetById(dto.Avaliacao_ID);

                    if (entity == null)
                    {
                        // Se a avaliação não for encontrada, retorna um erro
                        return new ApiResponse<AvaliacaoPostDTO>
                        {
                            Success = false,
                            Type = "error",
                            Message = "Avaliação não encontrada para atualização."
                        };
                    }

                    // Atualiza as propriedades da entidade existente com os dados do DTO
                    entity.Nota = dto.Nota;
                    entity.Comentario = dto.Comentario;
                    entity.Usuario_ID = dto.Usuario_ID;
                    entity.Instituicao_ID = dto.Instituicao_ID;
                    entity.Tipo = (Data.Models.AvaliacaoModel.TipoAvaliacao)dto.Tipo;
                    entity.Habilitado = (bool)dto.Habilitado;
                    entity.DataAvaliacao = DateTime.Now; // Define a data de avaliação como a data/hora atual da modificação

                    // Chama o método de atualização no repositório
                    await _avaliacaoRepository.SaveOrUpdate(entity);
                    successMessage = "Avaliação editada com sucesso.";
                }
                else
                {
                    // Se o ID não for maior que 0, é uma nova avaliação a ser salva
                    entity = new Avaliacao();

                    // Mapeia as propriedades do DTO para a nova entidade
                    entity.Nota = dto.Nota;
                    entity.Comentario = dto.Comentario;
                    entity.Usuario_ID = dto.Usuario_ID;
                    entity.Instituicao_ID = dto.Instituicao_ID;
                    entity.Tipo = (Data.Models.AvaliacaoModel.TipoAvaliacao)dto.Tipo;
                    entity.Habilitado = (bool)dto.Habilitado;
                    entity.DataAvaliacao = DateTime.Now; // Define a data de avaliação como a data/hora atual da criação

                    // Chama o método de salvamento (inserção) no repositório
                    await _avaliacaoRepository.Save(entity);

                    // Se o ID da entidade for gerado pelo banco de dados, você pode atribuí-lo ao DTO aqui
                    // dto.Id = entity.Id;
                    successMessage = "Avaliação salva com sucesso.";
                }

                return new ApiResponse<AvaliacaoPostDTO>
                {
                    Success = true,
                    Type = "success",
                    Message = successMessage,
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                // É uma boa prática logar a exceção (ex) aqui para depuração
                // _logger.LogError(ex, "Ocorreu um erro ao processar a avaliação.");

                return new ApiResponse<AvaliacaoPostDTO>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao processar a avaliação. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }
    }
}
