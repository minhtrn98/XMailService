using System.Data;

namespace XMailService.Application.Interfaces;

public interface IDbConnectionFactory : IAsyncDisposable
{
    IDbConnection CreateConnection();
    Task<IEnumerable<TData>> QueryAsync<TData>(string sql, object param);
    Task<TData?> QueryFirstAsync<TData>(string sql, object param);
}
