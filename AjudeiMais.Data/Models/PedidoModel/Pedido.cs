using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.InstituicaoModel;

namespace AjudeiMais.Data.Models.PedidoModel
{
    public class Pedido
    {
        [Key]
        public int Pedido_ID {  get; set; }
        public string NumeroPedido { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public string Status {  get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }

        [ForeignKey("Instituicao")]
        public int Instituicao_ID {  get; set; }
        public Instituicao Instituicao { get; set; }

    }
}
