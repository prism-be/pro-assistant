// -----------------------------------------------------------------------
//  <copyright file = "AdminController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AdminController: Controller
{
    private readonly ICrudService _crudService;
    private readonly IPropertyUpdatePublisher _propertyUpdatePublisher;

    public AdminController(ICrudService crudService, IPropertyUpdatePublisher propertyUpdatePublisher)
    {
        _crudService = crudService;
        _propertyUpdatePublisher = propertyUpdatePublisher;
    }

    [Route("api/admin/rebuild")]
    [HttpPost]
    public async Task Rebuild()
    {
        foreach (var contact in await _crudService.FindMany<Contact>())
        {
            _propertyUpdatePublisher.Publish(new PropertyUpdated(nameof(Contact), contact.Id, nameof(Contact.BirthDate), contact.BirthDate));
            _propertyUpdatePublisher.Publish(new PropertyUpdated(nameof(Contact), contact.Id, nameof(Contact.PhoneNumber), contact.PhoneNumber));
        }
        
        foreach (var tariff in await _crudService.FindMany<Tariff>())
        {
            _propertyUpdatePublisher.Publish(new PropertyUpdated(nameof(Tariff), tariff.Id, nameof(Appointment.BackgroundColor), tariff.BackgroundColor));
        }
    }
}