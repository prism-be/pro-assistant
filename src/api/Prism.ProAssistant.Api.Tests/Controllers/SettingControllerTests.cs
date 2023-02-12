// -----------------------------------------------------------------------
//  <copyright file = "SettingControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers;

public class SettingControllerTests
{
    [Fact]
    public async Task FindMany()
    {
        await CrudTests.FindMany<SettingController, Setting>(c => c.FindMany());
    }

    [Fact]
    public async Task SaveSettings()
    {
        await CrudTests.UpsertMany<SettingController, Setting>(c => c.SaveSettings(new List<Setting>
        {
            new()
            {
                Id = Identifier.GenerateString(),
                Value = Identifier.GenerateString()
            }
        }));
    }
}