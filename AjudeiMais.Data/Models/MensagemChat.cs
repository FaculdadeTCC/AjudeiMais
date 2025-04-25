using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class MensagemChat
    {
        [Key]
        public int MensagemChat_ID { get; set; }
        public string Mensagem {  get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public string GUID { get; set; }
        public string TipoRemente { get; set; }
        public int Remetente_ID { get; set; }

        [ForeignKey("Chat")]
        public int Chat_ID { get; set; }
        public Chat Chat { get; set; }


    }
}
