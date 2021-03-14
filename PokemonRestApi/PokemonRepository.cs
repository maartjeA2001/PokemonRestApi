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
            var ids = pokemons.Select(p => p.pokemonId).ToArray();

            var types = await GetTypeLookup(conn, ids);
            var baseStats = await GetBaseStatsLookup(conn, ids);
            var abilities = await GetAbilityLookup(conn, ids);

            foreach (var p in pokemons)
            {
                p.pokemonTypeIds = types[p.pokemonId].ToArray();
                p.baseStats = baseStats[p.pokemonId];
                p.abilities = abilities[p.pokemonId].ToArray();
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

        private static async Task<ILookup<int, int>> GetTypeLookup(MySqlConnection conn, int[] pokemonIds)
            => (await conn.QueryAsync<(int pok_id, int type_id)>(@"
                select pt.pok_id, pt.type_id from pokemon_types pt
                where pt.pok_id in @pokemonIds
            ", new { pokemonIds })).ToLookup(pt => pt.pok_id, pt => pt.type_id);

        private static async Task<Dictionary<int, BaseStats>> GetBaseStatsLookup(MySqlConnection conn, int[] pokemonIds)
            => (await conn.QueryAsync<(int pok_id, int b_hp, int b_atk, int b_def, int b_sp_atk, int b_sp_def, int b_speed)>(@"
                select
                    pok_id
                    , b_hp
                    , b_atk
                    , b_def
                    , b_sp_atk
                    , b_sp_def
                    , b_speed
                from base_stats
                where pok_id in @pokemonIds
            ", new { pokemonIds })).ToDictionary(bs => bs.pok_id, bs => new BaseStats
            {
                hp = bs.b_hp,
                atk = bs.b_atk,
                def = bs.b_def,
                spAtk = bs.b_sp_atk,
                spDef = bs.b_sp_def,
                speed = bs.b_speed,
            });

        private static async Task<ILookup<int, Ability>> GetAbilityLookup(MySqlConnection conn, int[] pokemonIds)
            => (await conn.QueryAsync<(int pok_id, bool is_hidden, string abil_name)>(@"
                select
                    pa.pok_id
                    , pa.is_hidden
                    , a.abil_name
                from pokemon_abilities pa
                join abilities a on a.abil_id = pa.abil_id
                where pa.pok_id in @pokemonIds
            ", new { pokemonIds })).ToLookup(a => a.pok_id, a => new Ability
            {
                abilityName = a.abil_name,
                isHidden = a.is_hidden,
            });

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
