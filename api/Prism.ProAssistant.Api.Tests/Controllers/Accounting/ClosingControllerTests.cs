namespace Prism.ProAssistant.Api.Tests.Controllers.Accounting;

using Api.Controllers.Data.Accounting;
using Domain.DayToDay.Appointments;
using Infrastructure.Providers;
using Moq;
using Storage;

public class ClosingControllerTests
{
    [Fact]
    public async Task ListUnclosed_Ok()
    {
        // Arrange
        var queryService = new Mock<IQueryService>();
        
        // Act
        var controller = new ClosingController(queryService.Object);
        var result = await controller.ListUnclosed();

        // Assert
        Assert.NotNull(result);
        queryService.Verify(x => x.SearchAsync<Appointment>(It.Is<Filter[]>(f => f.Length == 2)));
    }
}