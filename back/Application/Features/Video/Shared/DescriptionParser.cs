using System.Text.RegularExpressions;

namespace Application.Features.Video.Shared;

internal class DescriptionParser : IDescriptionParser
{
    public ParsedDescription ParseDescription(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new ParsedDescription { CleanText = "", Tags = new List<string>() };

        var tags = Regex.Matches(input, @"#([\p{L}\p{N}_]+)")
            .Select(m => m.Groups[1].Value.ToLower())
            .Distinct()
            .ToList();

        var cleanText = Regex.Replace(input, @"#\w+", "").Trim();
        cleanText = Regex.Replace(cleanText, @"\s{2,}", " ");

        return new ParsedDescription { CleanText = cleanText, Tags = tags };
    }
}