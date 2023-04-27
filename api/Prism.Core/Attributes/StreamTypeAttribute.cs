namespace Prism.Core.Attributes;

using Exceptions;

public class StreamTypeAttribute : Attribute
{
    public StreamTypeAttribute(string streamType)
    {
        StreamType = streamType;
    }

    public string StreamType { get; }

    public static string GetStreamType<T>()
    {
        return GetStreamType(typeof(T));
    }

    public static string GetStreamType(Type type)
    {
        var streamType = (type.GetCustomAttributes(typeof(StreamTypeAttribute), true)
                .FirstOrDefault()
            as StreamTypeAttribute)?.StreamType;

        if (string.IsNullOrWhiteSpace(streamType))
        {
            throw new MissingConfigurationException("StreamTypeAttribute is missing from type", type.Name);
        }

        return streamType;
    }
}