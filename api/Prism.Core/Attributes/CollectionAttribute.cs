// -----------------------------------------------------------------------
//  <copyright file = "BsonCollectionAttribute.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Core.Exceptions;

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
        return GetCollectionName(typeof(T));
    }
    
    public static string GetCollectionName(Type type)
    {
        var name = (type.GetCustomAttributes(typeof(CollectionAttribute), true)
                .FirstOrDefault()
            as CollectionAttribute)?.CollectionName;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingConfigurationException("The collection name is not specified", type.FullName);
        }

        return name;
    }
}