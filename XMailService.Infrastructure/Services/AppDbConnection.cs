using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using XMailService.Application.Interfaces;

namespace XMailService.Infrastructure.Services;

public sealed class AppDbConnection(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string? _connectionString = configuration.GetConnectionString("Default");
    private NpgsqlConnection? _connection;

    public IDbConnection CreateConnection() => _connection ??= new NpgsqlConnection(_connectionString);

    public Task<IEnumerable<TData>> QueryAsync<TData>(string sql, object param)
        => CreateConnection().QueryAsync<TData>(sql, param);

    public Task<TData?> QueryFirstAsync<TData>(string sql, object param)
        => CreateConnection().QueryFirstOrDefaultAsync<TData>(sql, param);

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }
}
