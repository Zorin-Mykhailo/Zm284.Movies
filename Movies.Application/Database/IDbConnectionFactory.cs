using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Npgsql;

namespace Movies.Application.Database;
public interface IDbConnectionFactory {
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}


public class NpgsqlConnectionFactory : IDbConnectionFactory {

    private readonly string _connectionString;

    public NpgsqlConnectionFactory(string connectionString) {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default) {

        NpgsqlConnection connection = new (_connectionString);
        await connection.OpenAsync(token);
        return connection;
    } 
}