namespace AjudeiMais.API.DTO
{
	public class EnviarMensagemDTO
	{
		public int Chat_ID { get; set; }
		public string Mensagem { get; set; }
		public string TipoRemetente { get; set; } // "Usuario" ou "Instituicao"
		public int Remetente_ID { get; set; }
	}
}
