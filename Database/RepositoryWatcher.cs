using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Database
{
    public class RepositoryWatcher : IRepositoryWatcher
    {
        const string FetchSql = "SELECT * FROM Users";

        
        private readonly string _connectionString;

       
        public RepositoryWatcher(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Guid>> GetNewUsers()
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.QueryFirstOrDefaultAsync(FetchSql);

            return new List<Guid>{Guid.NewGuid()};
        }
    }
}