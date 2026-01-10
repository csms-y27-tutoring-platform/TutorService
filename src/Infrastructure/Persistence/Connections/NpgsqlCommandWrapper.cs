using Infrastructure.Persistence.Abstractions;
using Npgsql;
using NpgsqlTypes;
using System.Data.Common;

namespace Infrastructure.Persistence.Connections;

internal class NpgsqlCommandWrapper : ICommand
{
    private readonly NpgsqlCommand _command;
    private readonly List<NpgsqlParameter> _parameters = new();

    public NpgsqlCommandWrapper(NpgsqlCommand command)
    {
        _command = command;
    }

    public ICommand AddParameter<T>(string name, T value)
    {
        var parameter = new NpgsqlParameter(name, GetNpgsqlDbType(typeof(T)))
        {
            Value = (object?)value ?? DBNull.Value,
        };
        _command.Parameters.Add(parameter);
        _parameters.Add(parameter);
        return this;
    }

    public ICommand AddParameter<T>(string name, IEnumerable<T> values)
    {
        var parameter = new NpgsqlParameter(name, NpgsqlDbType.Array | GetNpgsqlDbType(typeof(T)))
        {
            Value = values?.ToArray(),
        };
        _command.Parameters.Add(parameter);
        _parameters.Add(parameter);
        return this;
    }

    public async Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
    {
        return await _command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
    {
        return await _command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default)
    {
        object? result = await _command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result is DBNull;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (NpgsqlParameter parameter in _parameters)
        {
            parameter.Dispose();
        }

        await _command.DisposeAsync().ConfigureAwait(false);
    }

    private static NpgsqlDbType GetNpgsqlDbType(Type type)
    {
        return type.Name switch
        {
            "Guid" => NpgsqlDbType.Uuid,
            "String" => NpgsqlDbType.Text,
            "Int32" => NpgsqlDbType.Integer,
            "Int64" => NpgsqlDbType.Bigint,
            "Decimal" => NpgsqlDbType.Numeric,
            "Boolean" => NpgsqlDbType.Boolean,
            "DateTime" => NpgsqlDbType.TimestampTz,
            "DateTimeOffset" => NpgsqlDbType.TimestampTz,
            _ => NpgsqlDbType.Text,
        };
    }
}