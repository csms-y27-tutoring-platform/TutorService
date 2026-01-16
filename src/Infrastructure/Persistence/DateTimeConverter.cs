using Google.Protobuf.WellKnownTypes;

namespace Infrastructure.Persistence;

public static class DateTimeConverter
{
    public static DateTime ToDateTime(object value)
    {
        return value switch
        {
            DateTime dateTime => dateTime,
            Timestamp timestamp => timestamp.ToDateTime(),
            _ => throw new InvalidOperationException($"Cannot convert {value.GetType()} to DateTime"),
        };
    }

    public static DateTime? ToNullableDateTime(object? value)
    {
        if (value == null)
            return null;

        return value switch
        {
            DateTime dateTime => dateTime,
            Timestamp timestamp => timestamp.ToDateTime(),
            _ => throw new InvalidOperationException($"Cannot convert {value.GetType()} to DateTime"),
        };
    }

    public static object ToDatabaseDateTime(object value)
    {
        if (value is DateTime dateTime)
            return dateTime;

        if (value is Timestamp timestamp)
            return timestamp.ToDateTime();

        return value;
    }
}