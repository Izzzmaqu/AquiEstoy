namespace AquiEstoy.Application.Interfaces;

/// <summary>
/// Abstraccion del hashing de contrasenas. La implementacion concreta (BCrypt)
/// vive en Infrastructure para que Application no dependa del algoritmo.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string hash);
}
