using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.Core.Exceptions;

[Serializable]
public class InvalidModelException : Exception
{
    public InvalidModelException(Dictionary<string, string[]> validations) : this("The model is invalid", validations)
    {
    }

    public InvalidModelException(string message, Dictionary<string, string[]> validations) : base(message)
    {
        Validations = validations;
    }

    [ExcludeFromCodeCoverage]
    protected InvalidModelException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Validations = info.GetValue(nameof(Validations), typeof(Dictionary<string, string[]>)) as Dictionary<string, string[]> ?? new Dictionary<string, string[]>();
    }

    public Dictionary<string, string[]> Validations { get; set; }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        Validations = info.GetValue(nameof(Validations), typeof(Dictionary<string, string[]>)) as Dictionary<string, string[]> ?? new Dictionary<string, string[]>();
    }
}