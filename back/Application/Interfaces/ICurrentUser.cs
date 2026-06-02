namespace Application.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get;}
    bool IsAuthenticated { get;}
}