// -----------------------------------------------------------------------
//  <copyright file = "Localizator.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;

namespace Prism.ProAssistant.Documents.Locales;

public interface ILocalizator
{
    string Locale { get; }
    string GetTranslation(string ns, string key);
}

public class Localizator : ILocalizator
{

    private readonly Dictionary<string, JsonNode> _translations = new();
    public string Locale => "fr";

    public string GetTranslation(string ns, string key)
    {
        EnsureJson(ns);
        return _translations[ns][key]?.ToString() ?? String.Empty;
    }

    private void EnsureJson(string ns)
    {
        if (_translations.ContainsKey(ns))
        {
            return;
        }

        var filePath = Path.Combine(Path.GetDirectoryName(typeof(Localizator).Assembly.Location)!, "Locales", Locale, $"{ns}.json");
        var json = File.ReadAllText(filePath);
        _translations.Add(ns, JsonNode.Parse(json) ?? new JsonObject());
    }
}