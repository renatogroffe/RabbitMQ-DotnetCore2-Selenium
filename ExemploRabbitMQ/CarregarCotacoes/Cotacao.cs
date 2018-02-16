using System;
using Dapper.Contrib.Extensions;

namespace CarregarCotacoes
{
    [Table("dbo.Cotacoes")]
    public class Cotacao
    {
        [ExplicitKey]
        public string NomeMoeda { get; set; }
        public DateTime DtUltimaCarga { get; set; }
        public double ValorCompra { get; set; }
        public double ValorVenda { get; set; }
        public string Variacao { get; set; }
    }
}