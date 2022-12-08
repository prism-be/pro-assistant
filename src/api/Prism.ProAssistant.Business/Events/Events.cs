// -----------------------------------------------------------------------
//  <copyright file = "Events.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Events;

public record UpsertedItem<T>(T? Previous, T Current, string Organisation);