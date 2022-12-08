// -----------------------------------------------------------------------
//  <copyright file = "Topics.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Events;

public static class Topics
{
    public static class Actions
    {
        public const string Updated = "Udpated";
    }

    public static string GetExchangeName<T>(string action)
    {
        return $"ProAssistant.{action}.{typeof(T).Name}";
    }
}