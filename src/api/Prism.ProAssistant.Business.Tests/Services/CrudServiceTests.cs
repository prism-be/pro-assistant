// -----------------------------------------------------------------------
//  <copyright file = "CrudServiceTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Business.Tests.Services;

public class CrudServiceTests
{
    [Fact]
    public async Task FindMany()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        await crudService.FindMany<Contact>();

        // Assert
        findManyService.Verify(x => x.Find<Contact>(), Times.Once);
    }

    [Fact]
    public async Task FindOne()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var id = Identifier.GenerateString();
        await crudService.FindOne<Contact>(id);

        // Assert
        findOneService.Verify(x => x.Find<Contact>(id), Times.Once);
    }

    [Fact]
    public async Task RemoveOne()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var id = Identifier.GenerateString();
        await crudService.RemoveOne<Contact>(id);

        // Assert
        removeOneService.Verify(x => x.Remove<Contact>(id), Times.Once);
    }

    [Fact]
    public async Task UpdateManyProperty()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var id = Identifier.GenerateString();
        await crudService.UpdateManyProperty<Contact>("Test", id, "Name", "Test");

        // Assert
        updateManyPropertyService.Verify(x => x.Update<Contact>("Test", id, "Name", "Test"), Times.Once);
    }

    [Fact]
    public async Task UpdateProperty()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var id = Identifier.GenerateString();
        await crudService.UpdateProperty<Contact>(id, "Name", "Test");

        // Assert
        updatePropertyService.Verify(x => x.Update<Contact>(id, "Name", "Test"), Times.Once);
    }

    [Fact]
    public async Task UpsertMany()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var items = new List<Contact>();
        await crudService.UpsertMany(items);

        // Assert
        upsertManyService.Verify(x => x.Upsert(items), Times.Once);
    }

    [Fact]
    public async Task UpsertOne()
    {
        // Arrange
        var findManyService = new Mock<IFindManyService>();
        var findOneService = new Mock<IFindOneService>();
        var removeOneService = new Mock<IRemoveOneService>();
        var updateManyPropertyService = new Mock<IUpdateManyPropertyService>();
        var updatePropertyService = new Mock<IUpdatePropertyService>();
        var upsertManyService = new Mock<IUpsertManyService>();
        var upsertOneService = new Mock<IUpsertOneService>();

        // Act
        var crudService = new CrudService(findManyService.Object, findOneService.Object, removeOneService.Object, updateManyPropertyService.Object, updatePropertyService.Object,
            upsertManyService.Object, upsertOneService.Object);
        var item = new Contact
        {
            Id = Identifier.GenerateString()
        };
        await crudService.UpsertOne(item);

        // Assert
        upsertOneService.Verify(x => x.Upsert(item), Times.Once);
    }
}