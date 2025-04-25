using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class Pedido
    {
        [Key]
        public int Pedido_ID {  get; set; }
        public string NumeroPedido { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public string Status {  get; set; }

        [ForeignKey("Instituicao")]
        public int instituicao_ID {  get; set; }
        public Instituicao instituicao { get; set; }

    }
}
