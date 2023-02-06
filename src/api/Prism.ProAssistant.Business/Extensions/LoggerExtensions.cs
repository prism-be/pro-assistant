﻿// -----------------------------------------------------------------------
//  <copyright file = "LoggerExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Extensions;

public static class LoggerExtensions
{

    public static async Task LogDataDelete(this ILogger logger, IUserContextAccessor userContextAccessor, string itemId, Func<Task> action)
    {
        logger.LogInformation("Deleting an existing item by user {userId} with id {itemId}", userContextAccessor.UserId, itemId);
        await action();
        logger.LogInformation("Deleted an existing item by user {userId} with id {itemId}", userContextAccessor.UserId, itemId);
    }

    public static async Task<UpsertResult> LogDataInsert(this ILogger logger, IUserContextAccessor userContextAccessor, IDataModel data, Func<Task<UpsertResult>> action)
    {
        logger.LogInformation("Inserting an new item of type {itemType} by user {userId} with id {itemId}", data.GetType().Name, userContextAccessor.UserId, data.Id);
        var result = await action();
        logger.LogInformation("Inserted an new item of type {itemType} by user {userId} with id {itemId}", data.GetType().Name, userContextAccessor.UserId, result.Id);
        return result;
    }

    public static async Task<UpsertResult> LogDataUpdate(this ILogger logger, IUserContextAccessor userContextAccessor, IDataModel data, Func<Task<UpsertResult>> action)
    {
        logger.LogInformation("Updating an existing item of type {itemType} by user {userId} with id {itemId}", data.GetType().Name, userContextAccessor.UserId, data.Id);
        var result = await action();
        logger.LogInformation("Updated an existing item of type {itemType} by user {userId} with id {itemId}", data.GetType().Name, userContextAccessor.UserId, result.Id);
        return result;
    }

    public static async Task LogPropertyManyUpdate<T>(this ILogger logger, string userId, string propertyName, string filterProperty,
        Func<Task<long>> action)
    {
        logger.LogInformation("Updating many property {propertyName} of type {itemType} by user {userId} with filter on property {filterProperty}", propertyName, typeof(T).Name,
            userId, filterProperty);
        var count = await action();
        logger.LogInformation("Updated many property {propertyName} of type {itemType} by user {userId} with filter on property {filterProperty} - {matches} updates done.",
            propertyName, typeof(T).Name, userId, filterProperty, count);
    }

    public static async Task<UpsertResult> LogPropertyUpdate<T>(this ILogger logger, IUserContextAccessor userContextAccessor, string propertyName, string id,
        Func<Task<UpsertResult>> action)
    {
        logger.LogInformation("Updating property {propertyName} of type {itemType} by user {userId} with id {itemId}", propertyName, typeof(T).Name, userContextAccessor.UserId, id);
        var result = await action();
        logger.LogInformation("Updated property {propertyName} of type {itemType} by user {userId} with id {itemId}", propertyName, typeof(T).Name, userContextAccessor.UserId, result.Id);
        return result;
    }
}