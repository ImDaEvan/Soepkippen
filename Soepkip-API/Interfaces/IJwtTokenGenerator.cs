namespace SoepkipAPI.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateClientToken(string clientId);
}