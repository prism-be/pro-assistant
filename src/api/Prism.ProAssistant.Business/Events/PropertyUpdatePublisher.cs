// -----------------------------------------------------------------------
//  <copyright file = "PropertyUpdatePublisher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Immutable;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Events;

public interface IPropertyUpdatePublisher
{
    void Publish(PropertyUpdated propertyUpdated);
}

public class PropertyUpdatePublisher : IPropertyUpdatePublisher
{
    public static readonly ImmutableHashSet<string> WatchedProperties = ImmutableHashSet.CreateRange(new List<string>
    {
        $"{nameof(Tariff)}.{nameof(Tariff.BackgroundColor)}",
        $"{nameof(Contact)}.{nameof(Contact.BirthDate)}",
        $"{nameof(Contact)}.{nameof(Contact.PhoneNumber)}"
    });

    private readonly IPublisher _publisher;

    public PropertyUpdatePublisher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public void Publish(PropertyUpdated propertyUpdated)
    {
        if (WatchedProperties.Contains($"{propertyUpdated.ItemType}.{propertyUpdated.Property}"))
        {
            _publisher.Publish($"Property.Updated.{propertyUpdated.ItemType}.{propertyUpdated.Property}", propertyUpdated);
        }
    }
}