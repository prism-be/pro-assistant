// -----------------------------------------------------------------------
//  <copyright file = "LoggerExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Moq;

namespace Prism.ProAssistant.UnitTesting.Extensions;

public static class LoggerExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> mock, LogLevel level, Times times)
    {
        mock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            times);
    }
}