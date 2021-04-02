using Dapper;
using MySqlConnector;
using PokemonRESTApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi
{
    public interface IAbilityRepository
    {
        Task<Ability[]> GetAbilityData(int id);

        Task UpdateOrAddAblityData(Ability ability);

        Task DeleteAbilityData(int id);
    }

    public class MySqlAbilityRepository : IAbilityRepository
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

            await conn.QueryAsync("insert into abilities (abil_id, abil_name) values(@id, @name);"
                , new { id = ability.abilityId, name = ability.abilityName});
        }

        public async Task UpdateAbilityData(Ability ability)
        {
            await using var conn = await connection.OpenConnection();
            await conn.QueryAsync("update abilities set abil_name = @name where abil_id = @abil"
               , new { name = ability.abilityName , abil = ability.abilityId});
        }

        public async Task UpdateOrAddAblityData(Ability ability) 
        {
            if (ability.abilityId > 0)
            {
                await using var conn = await connection.OpenConnection();
                var count = await conn.ExecuteScalarAsync<int>(@"
                select count(*)
                from abilities
                where abil_id = @abil
            ", new { abil = ability.abilityId });
                if (count > 0)
                    await UpdateAbilityData(ability);
                else
                    await AddAbilityData(ability);
            }
        }

        public async Task<Ability[]> GetAbilityData(int id)
        {
            await using var conn = await connection.OpenConnection();

            var sql = "select abil_id abilityId, abil_name abilityName from abilities ";

            if (id != 0)
                sql += "where abil_id = @id";

            return (await conn.QueryAsync<Ability>(sql, new { id = id })).ToArray();
        }
    }
}
