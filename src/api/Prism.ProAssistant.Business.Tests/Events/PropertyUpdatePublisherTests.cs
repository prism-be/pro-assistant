// -----------------------------------------------------------------------
//  <copyright file = "PropertyUpdatePublisherTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Security;

namespace Prism.ProAssistant.Business.Tests.Events;

public class PropertyUpdatePublisherTests
{

    [Fact]
    public void Publish_IgnoredProperty()
    {
        // Arrange
        var publisher = new Mock<IPublisher>();

        // Act
        var sut = new PropertyUpdatePublisher(publisher.Object);
        sut.Publish(new PropertyUpdated("Contact", Identifier.GenerateString(), "BirthDateIgnored", DateTime.Now));

        // Assert
        publisher.Verify(x => x.Publish("Property.Updated.Contact.BirthDate", It.IsAny<PropertyUpdated>()), Times.Never);
    }

    [Fact]
    public void Publish_Ok()
    {
        // Arrange
        var publisher = new Mock<IPublisher>();

        // Act
        var sut = new PropertyUpdatePublisher(publisher.Object);
        sut.Publish(new PropertyUpdated("Contact", Identifier.GenerateString(), "BirthDate", DateTime.Now));

        // Assert
        publisher.Verify(x => x.Publish("Property.Updated.Contact.BirthDate", It.IsAny<PropertyUpdated>()), Times.Once);
    }
}