using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquiEstoy.Application.DTOs;
public class LineaAyudaDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;

    public bool EsEmergencia { get; set; }

    public string EnlaceTelefono => $"tel:{Telefono.Replace("-", "")}";
}
