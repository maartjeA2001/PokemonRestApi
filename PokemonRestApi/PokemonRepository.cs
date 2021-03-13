using Dapper;
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

        public async Task<PokemonResponse> GetPokemonData(FilterModel filter)
        {
            await using var conn = await OpenConnection();

            var pokemons = await GetPokemons(conn);

            var types = await GetTypeLookup(conn);



            foreach (var p in pokemons)
            {
                p.pokemonTypeIds = types[p.pokemonId].ToArray();
            }

            

            return new PokemonResponse
            {
                Pokemons = pokemons,
                Types = await GetTypes(conn)
            };
        }

        private async Task<Pokemon_REST_Api.Models.Type[]> GetTypes(MySqlConnection conn)
         => (await conn.QueryAsync<Pokemon_REST_Api.Models.Type>(@"
                select type_id typeId
                    ,type_name typeName
                from types
            ")).ToArray();

        private static async Task<ILookup<int, int>> GetTypeLookup(MySqlConnection conn)
            => (await conn.QueryAsync<(int pok_id, int type_id)>(@"
                select pt.pok_id, pt.type_id from pokemon_types pt
            ")).ToLookup(pt => pt.pok_id, pt => pt.type_id);

        private static async Task<Pokemon[]> GetPokemons(MySqlConnection conn)
            => (await conn.QueryAsync<Pokemon>(@"
                select
                    p.pok_id pokemonId
                    , p.pok_name pokemonName
                    , p.pok_height height
                    , p.pok_weight weight
                    , pok_base_experience baseExperience
                from pokemon p
            ")).ToArray();
    }
}
