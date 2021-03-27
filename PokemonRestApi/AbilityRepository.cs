using Dapper;
using MySqlConnector;
using Pokemon_REST_Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi
{
    public interface IAbililtyRepository
    {
        Task<Ability[]> GetAbilityData();

        Task UpdateOrAddAblityData(Ability ability);

        Task DeleteAbilityData(int id);
    }

    public class MySqlAbilityRepository : IAbililtyRepository
    {
        private MySqlConnecter connection;

        public MySqlAbilityRepository(string connectionString)
        {
            this.connection = new MySqlConnecter(connectionString);
        }

        public async Task DeleteAbilityData(int abilId) 
        {
            await using var conn = await connection.OpenConnection();

            await conn.QueryAsync("Delete from abilities where abil_id = @id;", new { id = abilId });
        }

        public async Task AddAbilityData(Ability ability) 
        {
            await using var conn = await connection.OpenConnection();

            await conn.QueryAsync("insert into ablities (abil_id, abil_name) values(@id, @name);"
                , new { id = ability.abilityId, name = ability.abilityName});
        }

        public async Task UpdateAbilityData(Ability ability)
        {
            await using var conn = await connection.OpenConnection();
            await conn.QueryAsync("update ablities set abil_name = @name"
               , new { name = ability.abilityName });
        }

        public async Task UpdateOrAddAblityData(Ability ability) 
        {
            await using var conn = await connection.OpenConnection();
            var count = await conn.ExecuteScalarAsync<int>(@"
                select count(*)
                from abilities
                where abil_id LIKE @abil
            ", new {abil = ability.abilityName } );
            if (count > 0)
                await UpdateAbilityData(ability);
            else
                await AddAbilityData(ability);
        }

        public async Task<Ability[]> GetAbilityData(int id)
        {
            await using var conn = await connection.OpenConnection();

            var sql = "select abil_id abilityId, abil_name abilityName from abilities";

            if (id == 0)
                sql += "where abil_id = @id";

            return (await conn.QueryAsync<Ability>(sql, new { id = id })).ToArray();
        }
    }
}
