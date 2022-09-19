// -----------------------------------------------------------------------
//  <copyright file = "DummyValidator.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using Prism.ProAssistant.Business;

namespace Prism.Picshare.Tests;

public class DummyRequestValidator : AbstractValidator<DummyRequest>
{
    public DummyRequestValidator()
    {
        RuleFor(x => x.Organisation).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Login).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Password).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public record DummyResponse(string? Token);