namespace AquiEstoy.Application.DTOs.Casos
{
    public class CerrarCasoDto
    {
        /// <summary>Actor que cierra el caso (sin auth todavía). Se valida que exista pero no se persiste: Caso no tiene columna CerradoPorId.</summary>
        public int UsuarioActorId { get; set; }

        public string? Notas { get; set; }
    }
}
