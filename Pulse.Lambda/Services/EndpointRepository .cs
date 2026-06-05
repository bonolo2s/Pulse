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
            SELECT id, url, interval_seconds
            FROM monitored_endpoints
            WHERE interval_seconds = @IntervalSeconds
            AND is_active = true
            """;

        return await connection.QueryAsync<EndpointForCheckDto>(sql, new { IntervalSeconds = intervalSeconds });
    }
}