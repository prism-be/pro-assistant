// -----------------------------------------------------------------------
//  <copyright file = "MongoDbConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.ProAssistant.Business.Storage;

public class MongoDbConfiguration
{
    public MongoDbConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; set; }
}