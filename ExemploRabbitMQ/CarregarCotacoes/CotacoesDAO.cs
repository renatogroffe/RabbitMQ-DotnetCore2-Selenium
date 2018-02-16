using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;

namespace CarregarCotacoes
{
    public class CotacoesDAO
    {
        private string _strConnection;

        public CotacoesDAO(string stringConnection)
        {
            _strConnection = stringConnection;
        }

        public void CarregarDados(List<Cotacao> cotacoes)
        {
            using (SqlConnection conexao =
                new SqlConnection(_strConnection))
            {
                conexao.Execute("TRUNCATE TABLE dbo.Cotacoes");
                conexao.Insert(cotacoes);
            }
        }
    }
}