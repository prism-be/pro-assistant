using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Core.Exceptions;

[Serializable]
public class MissingConfigurationException : Exception
{

    public MissingConfigurationException(string message, string? missingConfigurationKey) : base(message)
    {
        MissingConfigurationKey = missingConfigurationKey ?? "Unknown";
    }

    [ExcludeFromCodeCoverage]
    protected MissingConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        MissingConfigurationKey = info.GetString(nameof(MissingConfigurationKey)) ?? "Unknown";
    }

    public string MissingConfigurationKey { get; set; }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        MissingConfigurationKey = info.GetString(nameof(MissingConfigurationKey)) ?? "Unknown";
    }
}