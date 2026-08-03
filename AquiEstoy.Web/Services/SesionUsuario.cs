using System.Security.Claims;

namespace AquiEstoy.Web.Services
{
    /// <summary>
    /// Acceso tipado a la identidad guardada en la cookie. Existe para que ningun
    /// controller ni vista tenga que hardcodear un UsuarioId.
    /// </summary>
    public static class SesionUsuario
    {
        public const string ClaimUsuarioId = "AquiEstoy:UsuarioId";
        public const string ClaimProfesionalId = "AquiEstoy:ProfesionalId";

        /// <summary>Id del usuario en sesion, o null si no hay sesion valida.</summary>
        public static int? UsuarioId(this ClaimsPrincipal usuario)
        {
            var valor = usuario.FindFirst(ClaimUsuarioId)?.Value;
            return int.TryParse(valor, out var id) ? id : null;
        }

        /// <summary>Id del registro Profesional, o null si el usuario no lo es.</summary>
        public static int? ProfesionalId(this ClaimsPrincipal usuario)
        {
            var valor = usuario.FindFirst(ClaimProfesionalId)?.Value;
            return int.TryParse(valor, out var id) ? id : null;
        }

        public static string NombreCompleto(this ClaimsPrincipal usuario) =>
            usuario.FindFirst(ClaimTypes.GivenName)?.Value
            ?? usuario.FindFirst(ClaimTypes.Name)?.Value
            ?? string.Empty;
    }
}
