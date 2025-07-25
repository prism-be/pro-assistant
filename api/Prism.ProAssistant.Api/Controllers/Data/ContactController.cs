using Microsoft.EntityFrameworkCore;
using Prism.Infrastructure.Providers.Azure;

namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.DayToDay.Contacts;
using Helpers;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class ContactController : Controller
{
    private readonly ProAssistantDbContext _dbContext;

    public ContactController(ProAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        request.Id = Identifier.GenerateString();
        _dbContext.Contacts.Add(request);
        await _dbContext.SaveChangesAsync();

        return new UpsertResult(request.Id);
    }

    [HttpGet]
    [Route("api/data/contacts")]
    public async Task<IEnumerable<Contact>> List()
    {
        return await _dbContext.Contacts
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ThenBy(c => c.BirthDate)
            .ToListAsync();
    }

    public record ContactSearch(string FirstName, string LastName, string BirthDate, string PhoneNumber);
    [HttpPost]
    [Route("api/data/contacts/search")]
    public async Task<IEnumerable<Contact>> Search([FromBody] ContactSearch request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        var query = _dbContext.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            query = query.Where(c => c.FirstName != null && c.FirstName.StartsWith(request.FirstName));
        }
        
        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            query = query.Where(c => c.LastName != null && c.LastName.StartsWith(request.LastName));
        }
        
        if (!string.IsNullOrWhiteSpace(request.BirthDate))
        {
            query = query.Where(c => c.BirthDate != null && c.BirthDate.Contains(request.BirthDate));
        }
        
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            query = query.Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(request.PhoneNumber));
        }
        
        query = query.OrderBy(c => c.LastName)
                     .ThenBy(c => c.FirstName)
                     .ThenBy(c => c.BirthDate);
        
        return await query.ToListAsync();
    }

    [HttpGet]
    [Route("api/data/contacts/{id}")]
    public async Task<Contact?> Single(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        _dbContext.Contacts.Update(request);
        await _dbContext.SaveChangesAsync();

        return new UpsertResult(request.Id);
    }
}