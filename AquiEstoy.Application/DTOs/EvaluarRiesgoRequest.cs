using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquiEstoy.Application.DTOs;
public class EvaluarRiesgoRequest
{
    public Guid UsuarioId { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    // Este valor puede ser enviado por la IA.
    public double PuntajeRiesgo { get; set; }
}