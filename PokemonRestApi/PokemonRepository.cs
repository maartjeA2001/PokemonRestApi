using MySqlConnector;
using Pokemon_REST_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi
{
    public interface IPokemonRepository
    {
        Task<PokemonResponse> GetPokemonData(FilterModel filter);
    }

    public class MySqlPokemonRepository : IPokemonRepository
    {
        readonly string connectionString;

        public MySqlPokemonRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        async Task<MySqlConnection> OpenConnection()
        {
            var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }

        public Task<PokemonResponse> GetPokemonData(FilterModel filter)
        {
            throw new NotImplementedException();
        }
    }
}
