using AquiEstoy.Application.Interfaces;

namespace AquiEstoy.Infrastructure.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash con formato invalido (p. ej. datos cargados a mano en la BD):
            // se trata como credencial incorrecta en vez de reventar el login.
            return false;
        }
    }
}
