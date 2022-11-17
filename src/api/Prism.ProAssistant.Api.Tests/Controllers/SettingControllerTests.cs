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
        public async Task FindOne()
        {
            await CrudTests.FindOne<SettingController, Setting>(c => c.FindOne(Identifier.GenerateString()));
        }

        [Fact]
        public async Task UpsertOne()
        {
            await CrudTests.UpsertOne<SettingController, Setting>(c => c.UpsertOne(new Setting
            {
                Id = Identifier.GenerateString()
            }));
        }
    }
}