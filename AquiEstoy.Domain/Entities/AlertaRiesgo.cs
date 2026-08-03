using AquiEstoy.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquiEstoy.Domain.Entities;

public class AlertaRiesgo
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UsuarioId { get; set; }

    public NivelRiesgo Nivel { get; set; }

    public string Motivo { get; set; } = string.Empty;

    public string MensajeMostrado { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool Atendida { get; set; } = false;
}