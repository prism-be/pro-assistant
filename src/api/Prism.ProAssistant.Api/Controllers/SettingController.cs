// -----------------------------------------------------------------------
//  <copyright file = "SettingController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Controllers;

public class SettingController : Controller
{
    private readonly ICrudService _crudService;

    public SettingController(ICrudService crudService)
    {
        _crudService = crudService;
    }

    [Route("api/settings")]
    [HttpGet]
    public async Task<ActionResult<List<Setting>>> FindMany()
    {
        var result = await _crudService.FindMany<Setting>();
        return result.ToActionResult();
    }

    [Route("api/settings")]
    [HttpPost]
    public async Task SaveSettings([FromBody] List<Setting> settings)
    {
        await _crudService.UpsertMany(settings);
    }
}