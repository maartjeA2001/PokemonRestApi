using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi
{
    public class MySqlConnecter
    {
        readonly string connectionString;

        public MySqlConnecter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<MySqlConnection> OpenConnection()
        {
            var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}
