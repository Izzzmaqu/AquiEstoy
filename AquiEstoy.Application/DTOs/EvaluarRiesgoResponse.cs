using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquiEstoy.Application.DTOs;

public class EvaluarRiesgoResponse
{
    public Guid AlertaId { get; set; }

    public string NivelRiesgo { get; set; } = string.Empty;

    public bool MostrarAlerta { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public List<LineaAyudaDto> LineasAyuda { get; set; } = [];
}