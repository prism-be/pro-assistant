// -----------------------------------------------------------------------
//  <copyright file = "DummyValidator.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using Prism.Picshare.Tests;

namespace Prism.ProAssistant.Business.Tests.Behaviors;

public class DummyRequestValidator : AbstractValidator<DummyRequest>
{
    public DummyRequestValidator()
    {
        RuleFor(x => x.Organization).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Login).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Password).NotNull().NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public record DummyResponse(string? Token);