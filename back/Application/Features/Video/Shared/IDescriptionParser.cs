namespace Application.Features.Video.Shared;

public interface IDescriptionParser
{
    public ParsedDescription ParseDescription(string input);
}