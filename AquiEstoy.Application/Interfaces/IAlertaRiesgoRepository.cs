using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AquiEstoy.Domain.Entities;

namespace AquiEstoy.Application.Interfaces;

public interface IAlertaRiesgoRepository
{
    Task GuardarAsync(
        AlertaRiesgo alerta,
        CancellationToken cancellationToken = default);

    Task<List<AlertaRiesgo>> ObtenerPorUsuarioAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);
}