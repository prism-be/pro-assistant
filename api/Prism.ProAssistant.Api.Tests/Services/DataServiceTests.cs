using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using Prism.ProAssistant.Api.Exceptions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Tests.Services;

public class DataServiceTests
{
    [Fact]
    public async Task DeleteAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var id = Identifier.GenerateString();

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.DeleteAsync<Contact>(id);

        // Assert
        collection.Verify(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task DeleteFileAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var id = Identifier.GenerateString();

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.DeleteFileAsync(id);

        // Assert
        bucket.Verify(x => x.DeleteAsync(new ObjectId(id), CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task GetFileAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.Setup(x => x.DownloadAsBytesAsync(new ObjectId(id), null, CancellationToken.None)).ReturnsAsync(new byte[] { 1, 2, 3 });
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var data = new byte[] { 1, 2, 3 };

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.GetFileAsync(id);

        // Assert
        result.Should().BeEquivalentTo(data);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task GetFileNameAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var fileInfo = new GridFSFileInfo(new BsonDocument
        {
            { "filename", id }
        });

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.SetupBucket(fileInfo);
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.GetFileNameAsync(id);

        // Assert
        result.Should().BeEquivalentTo(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task ListAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
            {
                Id = Identifier.GenerateString()
            },
            new Contact
            {
                Id = Identifier.GenerateString()
            });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.ListAsync<Contact>();

        // Assert
        result.Count.Should().Be(2);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task ReplaceAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);
        collection.Setup(x => x.FindOneAndReplaceAsync(
                It.IsAny<FilterDefinition<Contact>>(),
                It.IsAny<Contact>(),
                It.IsAny<FindOneAndReplaceOptions<Contact>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.ReplaceAsync(contact);

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Contact>>(), contact, It.IsAny<FindOneAndReplaceOptions<Contact>>(), CancellationToken.None), Times.Once);

        result.Id.Should().Be(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task ReplaceManyAsync_Ok()
    {
        // Arrange
        var id1 = Identifier.GenerateString();
        var contact1 = new Contact
        {
            Id = id1
        };

        var id2 = Identifier.GenerateString();
        var contact2 = new Contact
        {
            Id = id2
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact1, contact2);
        collection.Setup(x => x.FindOneAndReplaceAsync(
                It.IsAny<FilterDefinition<Contact>>(),
                It.IsAny<Contact>(),
                It.IsAny<FindOneAndReplaceOptions<Contact>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact1);

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.ReplaceManyAsync(new List<Contact>
            { contact1, contact2 });

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Contact>>(), contact1, It.IsAny<FindOneAndReplaceOptions<Contact>>(), CancellationToken.None), Times.Once);
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Contact>>(), contact2, It.IsAny<FindOneAndReplaceOptions<Contact>>(), CancellationToken.None), Times.Once);

        result.Count.Should().Be(2);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeast(3));
    }

    [Fact]
    public async Task ReplaceManyAsync_ShouldInsert()
    {
        // Arrange
        var id1 = Identifier.GenerateString();
        var contact1 = new Contact
        {
            Id = id1
        };

        var id2 = Identifier.GenerateString();
        var contact2 = new Contact
        {
            Id = id2
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact1, contact2);

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.ReplaceManyAsync(new List<Contact>
            { contact1, contact2 });

        // Assert
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Contact>>(), contact1, It.IsAny<FindOneAndReplaceOptions<Contact>>(), CancellationToken.None), Times.Once);
        collection.Verify(x => x.FindOneAndReplaceAsync(It.IsAny<FilterDefinition<Contact>>(), contact2, It.IsAny<FindOneAndReplaceOptions<Contact>>(), CancellationToken.None), Times.Once);

        collection.Verify(x => x.InsertOneAsync(contact1, null, CancellationToken.None), Times.Once);
        collection.Verify(x => x.InsertOneAsync(contact2, null, CancellationToken.None), Times.Once);

        result.Count.Should().Be(2);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeast(3));
    }

    [Fact]
    public async Task SearchAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var filters = new List<SearchFilter>
        {
            new()
                { Field = "field1", Operator = "eq", Value = "value1" },
            new()
                { Field = "field2", Operator = "ne", Value = "value2" },
            new()
                { Field = "field3", Operator = "gt", Value = "value3" },
            new()
                { Field = "field4", Operator = "gte", Value = "value4" },
            new()
                { Field = "field5", Operator = "lt", Value = "value5" },
            new()
                { Field = "field6", Operator = "lte", Value = DateTime.Now },
            new()
                { Field = "field7", Operator = "regex", Value = "(.*)" }
        };

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.SearchAsync<Contact>(filters);

        // Assert
        collection.Verify(x => x.FindAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions<Contact, Contact>>(), CancellationToken.None), Times.Once);

        result.Count.Should().Be(1);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SearchAsync_Unsupported()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);
        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        var filters = new List<SearchFilter>
        {
            new()
                { Field = "field1", Operator = "yolo", Value = "value1" }
        };

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.Invoking(s => s.SearchAsync<Contact>(filters))
            .Should().ThrowAsync<NotSupportedException>();
    }

    [Fact]
    public async Task SingleAsync_NotFound()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection();

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);

        await service.Invoking(s => s.SingleAsync<Contact>(id))
            .Should().ThrowAsync<NotFoundException>();

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleAsync<Contact>(id);

        // Assert
        result.Id.Should().Be(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_Null()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection();

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().BeNull();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task SingleOrDefaultAsync_SpecialCase()
    {
        // Arrange
        var id = "000000000000000000000000";

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(new Contact
        {
            Id = id
        });

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.SingleOrDefaultAsync<Contact>(id);

        // Assert
        result.Should().BeNull();

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task UpdateAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();
        var contact = new Contact
        {
            Id = id
        };

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact);

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.UpdateAsync(contact, nameof(contact.Id));

        // Assert
        collection.Verify(x => x.UpdateOneAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<UpdateDefinition<Contact>>(), null, CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task UpdateManyAsync_Ok()
    {
        // Arrange
        var id1 = Identifier.GenerateString();
        var contact1 = new Contact
        {
            Id = id1
        };

        var id2 = Identifier.GenerateString();
        var contact2 = new Contact
        {
            Id = id2
        };

        var filter = Builders<Contact>.Filter.Eq(x => x.Id, id1);
        var update = Builders<Contact>.Update.Set(x => x.FirstName, "John");

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var collection = new Mock<IMongoCollection<Contact>>();
        collection.SetupCollection(contact1, contact2);

        userOrganizationService.Setup(x => x.GetUserCollection<Contact>()).ReturnsAsync(collection.Object);

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        await service.UpdateManyAsync(filter, update);

        // Assert
        collection.Verify(x => x.UpdateManyAsync(filter, update, null, CancellationToken.None), Times.Once);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }

    [Fact]
    public async Task UploadFromBytesAsync_Ok()
    {
        // Arrange
        var id = Identifier.GenerateString();

        var logger = new Mock<ILogger<DataService>>();
        var userOrganizationService = new Mock<IUserOrganizationService>();

        var bucket = new Mock<IGridFSBucket>();
        bucket.Setup(x => x.UploadFromBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>(), null, CancellationToken.None))
            .ReturnsAsync(new ObjectId(id));
        userOrganizationService.Setup(x => x.GetUserGridFsBucket()).ReturnsAsync(bucket.Object);

        var data = new byte[] { 1, 2, 3 };

        // Act
        var service = new DataService(userOrganizationService.Object, logger.Object);
        var result = await service.UploadFromBytesAsync(Identifier.GenerateString(), data);

        // Assert
        result.Should().BeEquivalentTo(id);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.AtLeastOnce());
    }
}