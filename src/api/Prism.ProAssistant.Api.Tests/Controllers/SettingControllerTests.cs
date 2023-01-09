// -----------------------------------------------------------------------
//  <copyright file = "SettingControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Prism.ProAssistant.Api.Controllers;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Security;
using Xunit;

namespace Prism.ProAssistant.Api.Tests.Controllers
{
    public class SettingControllerTests
    {
        [Fact]
        public async Task FindMany()
        {
            await CrudTests.FindMany<SettingController, Setting>(c => c.FindMany());
        }

        [Fact]
        public async Task UpsertMany()
        {
            await CrudTests.UpsertMany<SettingController, Setting>(c => c.UpsertMany(new [] {new Setting
            {
                Id = Identifier.GenerateString()
            }}));
        }
    }
}