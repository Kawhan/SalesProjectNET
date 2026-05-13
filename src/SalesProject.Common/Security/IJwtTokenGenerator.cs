namespace SalesProject.Common.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(IUser user);
}
