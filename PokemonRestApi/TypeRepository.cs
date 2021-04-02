using Dapper;
using MySqlConnector;
using PokemonRESTApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonRestApi
{
    public interface ITypeRepository
    {
        Task<PokemonType[]> GetTypeData(int id);

        Task UpdateOrAddTypeData(PokemonType type);

        Task DeleteTypeData(int id);
    }

    public class MySqlTypeRepository : ITypeRepository
    {
        private MySqlConnecter connection;

        public MySqlTypeRepository(string connectionString)
        {
            this.connection = new MySqlConnecter(connectionString);
        }

        public async Task DeleteTypeData(int typeId) 
        {
            await using var conn = await connection.OpenConnection();

            await conn.QueryAsync("Delete from types where type_id = @id;", new { id = typeId });
        }

        public async Task AddTypeData(PokemonType type) 
        {
            await using var conn = await connection.OpenConnection();

            await conn.QueryAsync("insert into types (type_id, type_name) values(@id, @name);"
                , new { id = type.typeId, name = type.typeName});
        }

        public async Task UpdateTypeData(PokemonType type)
        {
            await using var conn = await connection.OpenConnection();
            await conn.QueryAsync("update types set type_name = @name where type_id = @typeId"
               , new { name = type.typeName, typeId = type.typeId });
        }

        public async Task UpdateOrAddTypeData(PokemonType type) 
        {
            if (type.typeId > 0)
            {
                await using var conn = await connection.OpenConnection();
                var count = await conn.ExecuteScalarAsync<int>(@"
                select count(*)
                from types
                where type_id = @type
            ", new { type = type.typeId });
                if (count > 0)
                    await UpdateTypeData(type);
                else
                    await AddTypeData(type);
            }
        }

        public async Task<PokemonType[]> GetTypeData(int id)
        {
            await using var conn = await connection.OpenConnection();

            var sql = "select type_id typeId, type_name typeName from types ";

            if (id != 0)
                sql += "where type_id = @id";

            return (await conn.QueryAsync<PokemonType>(sql, new { id = id })).ToArray();

        }
    }
}
