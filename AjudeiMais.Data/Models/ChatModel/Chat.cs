using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models.ChatModel
{
    public class Chat
    {
        [Key]
        public int Chat_ID{ get; set; }
        public DateTime DataAbertura { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public string GUID { get; set; }

        [ForeignKey("Usuario")]
        public int Usuario_ID { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Instituicao")]
        public int Instituicao_ID { get; set; }
        public Instituicao Instituicao { get; set; }

        public ICollection<MensagemChat>? MensagemChats { get; set; }
    }
}
