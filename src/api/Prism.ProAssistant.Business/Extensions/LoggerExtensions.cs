// -----------------------------------------------------------------------
//  <copyright file = "LoggerExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Prism.ProAssistant.Business.Models;

namespace Prism.ProAssistant.Business.Extensions;

public static class LoggerExtensions
{

    public static async Task LogDataDelete(this ILogger logger, string organizationId, string userId, string itemId, Func<Task> action)
    {
        logger.LogInformation("Deleting an existing item by user {userId}({organizationId}) with id {itemId}", userId, organizationId, itemId);
        await action();
        logger.LogInformation("Deleted an existing item by user {userId}({organizationId}) with id {itemId}", userId, organizationId, itemId);
    }

    public static async Task<string> LogDataInsert(this ILogger logger, string organizationId, string userId, IDataModel data, Func<Task<string>> action)
    {
        logger.LogInformation("Inserting an new item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, userId, organizationId, data.Id);
        var result = await action();
        logger.LogInformation("Inserted an new item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, userId, organizationId, result);
        return result;
    }

    public static async Task<string> LogDataUpdate(this ILogger logger, string organizationId, string userId, IDataModel data, Func<Task<string>> action)
    {
        logger.LogInformation("Updating an existing item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, userId, organizationId, data.Id);
        var result = await action();
        logger.LogInformation("Updated an existing item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, userId, organizationId, result);
        return result;
    }

    public static async Task LogPropertyManyUpdate<T>(this ILogger logger, string organizationId, string userId, string propertyName, string filterProperty,
        Func<Task<long>> action)
    {
        logger.LogInformation("Updating many property {propertyName} of type {itemType} by user {userId}({organizationId}) with filter on property {filterProperty}", propertyName,
            typeof(T).Name,
            userId, organizationId, filterProperty);
        var count = await action();
        logger.LogInformation("Updated many property {propertyName} of type {itemType} by user {userId}({organizationId}) with filter on property {filterProperty} - {matches} updates done.",
            propertyName, typeof(T).Name, userId, organizationId, filterProperty, count);
    }

    public static async Task<string> LogPropertyUpdate<T>(this ILogger logger, string organizationId, string userId, string propertyName, string id,
        Func<Task<string>> action)
    {
        logger.LogInformation("Updating property {propertyName} of type {itemType} by user {userId}({organizationId}) with id {itemId}", propertyName, typeof(T).Name, userId, organizationId,
            id);
        var result = await action();
        logger.LogInformation("Updated property {propertyName} of type {itemType} by user {userId}({organizationId}) with id {itemId}", propertyName, typeof(T).Name, userId, organizationId,
            result);
        return result;
    }
}