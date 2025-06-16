namespace AjudeiMais.Ecommerce.Models.Instituicao
{
    public class EnderecoModel
    {
        public int Endereco_ID { get; set; }
        public string CEP { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }

        public string? instituicao_GUID { get; set; }

    }
}
