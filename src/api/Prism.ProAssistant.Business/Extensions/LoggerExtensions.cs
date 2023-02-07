// -----------------------------------------------------------------------
//  <copyright file = "LoggerExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Extensions;

public static class LoggerExtensions
{

    public static async Task LogDataDelete(this ILogger logger, User user, string itemId, Func<Task> action)
    {
        logger.LogInformation("Deleting an existing item by user {userId}({organizationId}) with id {itemId}", user.Id, user.Organization, itemId);
        await action();
        logger.LogInformation("Deleted an existing item by user {userId}({organizationId}) with id {itemId}", user.Id, user.Organization, itemId);
    }

    public static async Task<string> LogDataInsert(this ILogger logger, User user, IDataModel data, Func<Task<string>> action)
    {
        logger.LogInformation("Inserting an new item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, user.Id, user.Organization, data.Id);
        var result = await action();
        logger.LogInformation("Inserted an new item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, user.Id, user.Organization, result);
        return result;
    }

    public static async Task<string> LogDataUpdate(this ILogger logger, User user, IDataModel data, Func<Task<string>> action)
    {
        logger.LogInformation("Updating an existing item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, user.Id, user.Organization, data.Id);
        var result = await action();
        logger.LogInformation("Updated an existing item of type {itemType} by user {userId}({organizationId}) with id {itemId}", data.GetType().Name, user.Id, user.Organization, result);
        return result;
    }

    public static async Task LogPropertyManyUpdate<T>(this ILogger logger, User user, string propertyName, string filterProperty,
        Func<Task<long>> action)
    {
        logger.LogInformation("Updating many property {propertyName} of type {itemType} by user {userId}({organizationId}) with filter on property {filterProperty}", propertyName,
            typeof(T).Name,
            user.Id, user.Organization, filterProperty);
        var count = await action();
        logger.LogInformation("Updated many property {propertyName} of type {itemType} by user {userId}({organizationId}) with filter on property {filterProperty} - {matches} updates done.",
            propertyName, typeof(T).Name, user.Id, user.Organization, filterProperty, count);
    }

    public static async Task<string> LogPropertyUpdate<T>(this ILogger logger, User user, string propertyName, string id,
        Func<Task<string>> action)
    {
        logger.LogInformation("Updating property {propertyName} of type {itemType} by user {userId}({organizationId}) with id {itemId}", propertyName, typeof(T).Name, user.Id,
            user.Organization,
            id);
        var result = await action();
        logger.LogInformation("Updated property {propertyName} of type {itemType} by user {userId}({organizationId}) with id {itemId}", propertyName, typeof(T).Name, user.Id,
            user.Organization,
            result);
        return result;
    }
}