document.addEventListener("DOMContentLoaded", function () {
    const datos = window.datosPanelEstadistico;

    if (!datos) {
        console.error("No se encontraron los datos del panel estadístico.");
        return;
    }

    const obtenerPropiedad = (elemento, nombreMinuscula, nombreMayuscula) => {
        return elemento[nombreMinuscula] ??
            elemento[nombreMayuscula];
    };

    const obtenerEtiquetas = (lista) => {
        return lista.map(item =>
            obtenerPropiedad(item, "etiqueta", "Etiqueta"));
    };

    const obtenerCantidades = (lista) => {
        return lista.map(item =>
            obtenerPropiedad(item, "cantidad", "Cantidad"));
    };

    const coloresGenerales = [
        "#5fa88c",
        "#7db9a3",
        "#a4d4c2",
        "#f0bd6b",
        "#e98d83",
        "#8ca9d3",
        "#aa97ca"
    ];

    Chart.defaults.font.family =
        "'Plus Jakarta Sans', Arial, sans-serif";

    Chart.defaults.color = "#718096";

    crearGraficoMeses(
        datos.casosPorMes,
        obtenerEtiquetas,
        obtenerCantidades
    );

    crearGraficoSeveridad(
        datos.casosPorSeveridad,
        obtenerEtiquetas,
        obtenerCantidades,
        obtenerPropiedad,
        coloresGenerales
    );

    crearGraficoProvincias(
        datos.casosPorProvincia,
        obtenerEtiquetas,
        obtenerCantidades
    );

    crearGraficoFactores(
        datos.factoresRiesgo,
        obtenerEtiquetas,
        obtenerCantidades
    );

    crearGraficoEstados(
        datos.casosPorEstado,
        obtenerEtiquetas,
        obtenerCantidades,
        coloresGenerales
    );
});

function crearGraficoMeses(
    lista,
    obtenerEtiquetas,
    obtenerCantidades
) {
    const elemento = document.getElementById("graficoMeses");

    if (!elemento) {
        return;
    }

    new Chart(elemento, {
        type: "line",
        data: {
            labels: obtenerEtiquetas(lista),
            datasets: [
                {
                    label: "Casos registrados",
                    data: obtenerCantidades(lista),
                    borderColor: "#4c9b7d",
                    backgroundColor: "rgba(76, 155, 125, 0.14)",
                    borderWidth: 3,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBackgroundColor: "#4c9b7d",
                    tension: 0.35,
                    fill: true
                }
            ]
        },
        options: opcionesBase("Casos")
    });
}

function crearGraficoSeveridad(
    lista,
    obtenerEtiquetas,
    obtenerCantidades,
    obtenerPropiedad,
    coloresGenerales
) {
    const elemento = document.getElementById("graficoSeveridad");

    if (!elemento) {
        return;
    }

    const colores = lista.map((item, indice) => {
        return obtenerPropiedad(item, "color", "Color") ||
            coloresGenerales[indice % coloresGenerales.length];
    });

    new Chart(elemento, {
        type: "doughnut",
        data: {
            labels: obtenerEtiquetas(lista),
            datasets: [
                {
                    data: obtenerCantidades(lista),
                    backgroundColor: colores,
                    borderColor: "#ffffff",
                    borderWidth: 3,
                    hoverOffset: 6
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: "66%",
            plugins: {
                legend: {
                    position: "bottom",
                    labels: {
                        usePointStyle: true,
                        padding: 18
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.raw} casos`;
                        }
                    }
                }
            }
        }
    });
}

function crearGraficoProvincias(
    lista,
    obtenerEtiquetas,
    obtenerCantidades
) {
    const elemento = document.getElementById("graficoProvincias");

    if (!elemento) {
        return;
    }

    new Chart(elemento, {
        type: "bar",
        data: {
            labels: obtenerEtiquetas(lista),
            datasets: [
                {
                    label: "Casos",
                    data: obtenerCantidades(lista),
                    backgroundColor: "#7db9a3",
                    borderRadius: 7,
                    borderSkipped: false
                }
            ]
        },
        options: {
            ...opcionesBase("Casos"),
            indexAxis: "y"
        }
    });
}

function crearGraficoFactores(
    lista,
    obtenerEtiquetas,
    obtenerCantidades
) {
    const elemento = document.getElementById("graficoFactores");

    if (!elemento) {
        return;
    }

    new Chart(elemento, {
        type: "bar",
        data: {
            labels: obtenerEtiquetas(lista),
            datasets: [
                {
                    label: "Registros",
                    data: obtenerCantidades(lista),
                    backgroundColor: "#f0bd6b",
                    borderRadius: 7,
                    borderSkipped: false
                }
            ]
        },
        options: {
            ...opcionesBase("Registros"),
            indexAxis: "y"
        }
    });
}

function crearGraficoEstados(
    lista,
    obtenerEtiquetas,
    obtenerCantidades,
    coloresGenerales
) {
    const elemento = document.getElementById("graficoEstados");

    if (!elemento) {
        return;
    }

    new Chart(elemento, {
        type: "pie",
        data: {
            labels: obtenerEtiquetas(lista),
            datasets: [
                {
                    data: obtenerCantidades(lista),
                    backgroundColor: coloresGenerales,
                    borderColor: "#ffffff",
                    borderWidth: 3,
                    hoverOffset: 6
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: "bottom",
                    labels: {
                        usePointStyle: true,
                        padding: 18
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `${context.label}: ${context.raw} casos`;
                        }
                    }
                }
            }
        }
    });
}

function opcionesBase(nombreUnidad) {
    return {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
            intersect: false,
            mode: "index"
        },
        plugins: {
            legend: {
                display: false
            },
            tooltip: {
                callbacks: {
                    label: function (context) {
                        return `${nombreUnidad}: ${context.raw}`;
                    }
                }
            }
        },
        scales: {
            x: {
                beginAtZero: true,
                grid: {
                    display: false
                },
                ticks: {
                    precision: 0
                }
            },
            y: {
                beginAtZero: true,
                border: {
                    display: false
                },
                grid: {
                    color: "rgba(203, 213, 224, 0.35)"
                },
                ticks: {
                    precision: 0
                }
            }
        }
    };
}
