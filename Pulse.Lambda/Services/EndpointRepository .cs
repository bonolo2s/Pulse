using Dapper;
using Npgsql;
using Pulse.Lambda.Interfaces;
using Pulse.Lambda.Models;

namespace Pulse.Lambda.Services;

public class EndpointRepository : IEndpointRepository
{
    private readonly string _connectionString;

    public EndpointRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<EndpointForCheckDto>> GetEndpointsByIntervalAsync(int intervalSeconds)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT "Id", "UserId", "Url", "IntervalSeconds"
            FROM "MonitoredEndpoints"
            WHERE "IntervalSeconds" = @IntervalSeconds
            AND "IsActive" = true
            """;

        return await connection.QueryAsync<EndpointForCheckDto>(sql, new { IntervalSeconds = intervalSeconds });
    }
}