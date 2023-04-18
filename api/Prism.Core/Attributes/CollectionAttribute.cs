// -----------------------------------------------------------------------
//  <copyright file = "BsonCollectionAttribute.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CollectionAttribute : Attribute
{
    public CollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }

    public string CollectionName { get; }
    
    public static string GetCollectionName<T>()
    {
        var name = (typeof(T).GetCustomAttributes(typeof(CollectionAttribute), true)
                .FirstOrDefault()
            as CollectionAttribute)?.CollectionName;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new NotImplementedException("The collection type is not implemented.");
        }

        return name;
    }
}