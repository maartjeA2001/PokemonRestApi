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

        Task UpdateOrAddPokemonData(Pokemon pokemon);

        Task DeletePokemonData(int id);

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

        public async Task DeletePokemonData(int pokId) 
        {
            await using var conn = await OpenConnection();

            await conn.QueryAsync("Delete from pokemon where pok_id = @id; " +
                "Delete from base_stats where pok_id = @id;" +
                "Delete from pokemon_abilities where pok_id = @id;" +
                "Delete from pokemon_types where pok_id = @id;", new { id = pokId });
        }

        public async Task AddPokemonData(Pokemon pokemon) 
        {
            await using var conn = await OpenConnection();

            await conn.QueryAsync("insert into pokemon (pok_id, pok_name, pok_height, pok_weight, pok_base_experience) values(@id, @name , @height, @weight, @exp); " +
                "insert into base_stats (pok_id, b_hp, b_atk, b_def, b_sp_atk, b_sp_def, b_speed) VALUES(@id, @b_hp, @b_atk, @b_def, @b_sp_atk, @b_sp_def, @b_speed);"
                , new { id = pokemon.pokemonId, name = pokemon.pokemonName, height = pokemon.height, weight = pokemon.weight, exp = pokemon.baseExperience
                , b_hp = pokemon.baseStats.hp, b_atk = pokemon.baseStats.atk, b_def = pokemon.baseStats.def, b_sp_atk = pokemon.baseStats.spAtk, b_sp_def = pokemon.baseStats.spDef, b_speed = pokemon.baseStats.speed
                });
            foreach (Ability abil in pokemon.abilities) {
                await conn.QueryAsync("insert into pokemon_abilities (pok_id, abil_id, is_hidden) values(@id, (select abil_id from abilities where abil_name Like @abilName) , @isHidden); ",
                    new {id = pokemon.pokemonId, abilName = abil.abilityName, isHidden = (abil.isHidden ? 1 : 0) });
            }
            foreach (int typeId in pokemon.pokemonTypeIds)
            {
                await conn.QueryAsync("insert into pokemon_types (pok_id, type_id) values(@id, @typeId); ",
                    new { id = pokemon.pokemonId, typeId = typeId});
            }
        }

        public async Task UpdatePokemonData(Pokemon pokemon)
        {
            await using var conn = await OpenConnection();

            await conn.QueryAsync("update pokemon set pok_name=@name , pok_height=@height , pok_weight=@weight , pok_base_experience=@exp where pok_id = @id; " +
                "update base_stats set b_hp=@b_hp, b_atk=@b_atk, b_def=@b_def, b_sp_atk=@b_sp_atk, b_sp_def=@b_sp_def, b_speed=@b_speed where pok_id = @id;"
                , new
                {
                    id = pokemon.pokemonId,
                    name = pokemon.pokemonName,
                    height = pokemon.height,
                    weight = pokemon.weight,
                    exp = pokemon.baseExperience,

                    b_hp = pokemon.baseStats.hp,
                    b_atk = pokemon.baseStats.atk,
                    b_def = pokemon.baseStats.def,
                    b_sp_atk = pokemon.baseStats.spAtk,
                    b_sp_def = pokemon.baseStats.spDef,
                    b_speed = pokemon.baseStats.speed
                });
            foreach (Ability abil in pokemon.abilities)
            {
                await conn.QueryAsync("update pokemon_abilities set abil_id=(select abil_id from abilities where abil_name Like @abilName), is_hidden=@isHidden where pok_id = @id; ",
                    new { id = pokemon.pokemonId, abilName = abil.abilityName, isHidden = (abil.isHidden ? 1 : 0) });
            }
            foreach (int typeId in pokemon.pokemonTypeIds)
            {
                await conn.QueryAsync("update pokemon_types set type_id=@typeId where pok_id = @id; ",
                    new { id = pokemon.pokemonId, typeId = typeId });
            }
        }

        public async Task UpdateOrAddPokemonData(Pokemon pokemon) 
        {
            await using var conn = await OpenConnection();
            var count = await conn.ExecuteScalarAsync<int>(@"
                select count(*)
                from pokemon
                where pok_id=@id
            ", new {id = pokemon.pokemonId } );
            if (count > 0)
                await UpdatePokemonData(pokemon);
            else
                await AddPokemonData(pokemon);
        }


        public async Task<PokemonResponse> GetPokemonData(FilterModel filter)
        {
            await using var conn = await OpenConnection();

            var pokemons = await GetPokemons(conn, filter);
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

        static string GetOrderByColumn(SortOrder sortOrder)
        {
            var orderByColumn = sortOrder.FieldName switch
            {
                Field.Name => "p.pok_name",
                Field.Height => "p.pok_Height",
                Field.Weight => "p.pok_Weight",
                Field.Hp => "bs.b_hp",
                Field.Atk => "bs.b_atk",
                Field.Def => "bs.b_def",
                Field.Sp_Atk => "bs.b_sp_atk",
                Field.Sp_Def => "bs.b_sp_def",
                Field.Spd => "bs.b_speed",
                _ => "p.pok_id",
            };
            return sortOrder.Descending ? orderByColumn + " desc" : orderByColumn;
        }

        private static async Task<Pokemon[]> GetPokemons(MySqlConnection conn, FilterModel filter)
        {
            var sql = @"
                select
                    p.pok_id pokemonId
                    , p.pok_name pokemonName
                    , p.pok_height height
                    , p.pok_weight weight
                    , pok_base_experience baseExperience
                from pokemon p
                join base_stats bs on bs.pok_id = p.pok_id
                where 1=1 ";

            if (filter.CanHaveAbility.Length > 0)
            {
                sql += @"and p.pok_id in (
                    select pa.pok_id
                    from pokemon_abilities pa
                    join abilities a on a.abil_id = pa.abil_id
                    where a.abil_name in @CanHaveAbility 
                    ) ";
            }
            if (filter.HasType.Length > 0)
            {
                sql += @"and p.pok_id in (
                    select pt.pok_id
                    from pokemon_types pt
                    join types t on t.type_id = pt.type_id
                    where t.type_name in @HasType 
                    ) ";
            }

            if (filter.HasId.Length > 0)
            {
                sql += @"and p.pok_id in @HasId ";
            }

            sql += @"
                order by " + GetOrderByColumn(filter.Sort) + @"
                limit @limit
            ";
            return (await conn.QueryAsync<Pokemon>(sql, new { filter.CanHaveAbility, filter.HasType, limit = filter.Amount <= 0 ? 10 :filter.Amount, Hasid = filter.HasId })).ToArray();
        }
    }
}
