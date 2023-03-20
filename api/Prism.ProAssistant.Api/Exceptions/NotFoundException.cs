using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.ProAssistant.Api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    [ExcludeFromCodeCoverage]
    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
}