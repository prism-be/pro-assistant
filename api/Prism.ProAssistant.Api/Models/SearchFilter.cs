﻿namespace Prism.ProAssistant.Api.Models;

public class SearchFilter
{
    required public string Type { get; set; }
    required public object Value { get; set; }
    required public string Field { get; set; }
}