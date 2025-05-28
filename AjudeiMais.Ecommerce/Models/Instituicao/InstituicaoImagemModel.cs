namespace AjudeiMais.Ecommerce.Models.Instituicao
{
    public class InstituicaoImagemModel
    {
        public int InsituicaoImagem_ID { get; set; }
        public string CaminhoImagem { get; set; }

        public string InstituicaoGuid { get; set; }
    }

    public class AtualizaFotosModel
    {
        public int InsituicaoImagem_ID { get; set; }
        
        public string Instituicao_GUID { get; set; }
        public List<IFormFile> Fotos { get; set; }
    }
}
