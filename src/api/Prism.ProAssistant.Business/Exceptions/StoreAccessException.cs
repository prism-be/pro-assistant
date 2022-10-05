// -----------------------------------------------------------------------
//  <copyright file = "MissingConfigurationException.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Prism.ProAssistant.Business.Exceptions;

[Serializable]
public class StoreAccessException : Exception
{

    public StoreAccessException(string message, string operationKey) : base(message)
    {
        OperationKey = operationKey;
    }

    [ExcludeFromCodeCoverage]
    protected StoreAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        OperationKey = info.GetString(nameof(OperationKey)) ?? "Unknown";
    }

    public string OperationKey { get; set; }

    [ExcludeFromCodeCoverage]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        OperationKey = info.GetString(nameof(OperationKey)) ?? "Unknown";
    }
}