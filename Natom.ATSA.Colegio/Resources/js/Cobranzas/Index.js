$(document).ready(function () {
    $("#tbl").on('click', '.btnEliminar', function () {
        MostrarCargando();
        var id = $(this).attr('data-id');
        $.ajax({
            url: '/colegiotest/cicloslectivos/EliminarCicloLectivo?CicloLectivoId=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                location.reload();
            }
        });
    });

    $("#tbl").on('click', '.btnCobrar', function () {
        var id = $(this).attr('inscripcion-id');
        var mes = $(this).attr('mes');
        var anio = $(this).attr('anio');
        AbrirModalCobranza(id, mes, anio);
    });

    $("#NuevoCicloLectivo").on('click', function () {
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/cicloslectivos/AbrirModal?CicloLectivoId=' + 0,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalCicloLectivo").modal('show');
                OcultarCargando();
            }
        });
    });

    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/colegiotest/cobranzas/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[5, "desc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '7%' },
            { "bSortable": true, "sWidth": '23%' },
            { "bSortable": true, "sWidth": '7%' },
            { "bSortable": true, "sWidth": '18%' },
            { "bSortable": false, "sWidth": '11%' },
            { "bSortable": true, "sWidth": '7%' },
            { "bSortable": true, "sWidth": '7%' },
            { "bSortable": true, "sWidth": '8%' },
            {
                "bSortable": false, "sWidth": '11%',
                mRender: function (data, type, row) {
                    var inscripcionId = data;
                    var carreraCursoId = row[9];
                    var cobranzaId = row[10];
                    var mes = row[11];
                    var anio = row[12];
                    var debeDocs = row[13];
                    var html = "";
                    if (debeDocs) {
                        html += "&nbsp;<a class='btn btn-warning btn-xs' onclick='MostrarDocsPendientes(" + data + ")'>&nbsp;!&nbsp;</a>";
                    }
                    if (cobranzaId === null) {
                        html += "&nbsp;<a class='btn btn-primary btn-xs btnCobrar' inscripcion-id='" + data + "' mes='" + mes + "' anio='" + anio + "'>Cobrar</a>";
                    }
                    else {
                        html += "&nbsp;<a class='btn btn-primary btn-xs' href='javascript:Imprimir(" + cobranzaId + ")'>Imprimir</a>";
                        html += "&nbsp;<a class='btn btn-danger btn-xs btnAnular' href='javascript:Anular(" + data + ", " + mes + ", " + anio + ")'>Anular</a>";
                    }
                    return html;
                }
            }
        ],

    });
});

function Anular(inscripcionId, mes, anio) {
    var _id = inscripcionId;
    var _mes = mes;
    var _anio = anio;
    Mensajes.MostrarSiNo("¿Desea eliminar la Cobranza?", function () {
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/cobranzas/Anular',
            method: 'POST',
            data: { inscripcionId: _id, mes: _mes, anio: _anio },
            dataType: 'json',
            success: function (data) {
                if (data.success == true) {
                    location.href = '/colegiotest/cobranzas/Index';
                }
                else {
                    Mensajes.MostrarError(data.error);
                    OcultarCargando();
                }
            }
        });
    });
}

function Imprimir(id) {
    window.open('/colegiotest/cobranzas/ImprimirReciboDePago?id=' + id, "_blank");
}

function MostrarDocsPendientes(inscripcionId) {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/inscripciones/ObtenerDocumentacionFaltantePorInscripcion',
        method: 'POST',
        data: { inscripcionId: inscripcionId },
        dataType: 'json',
        success: function (data) {
            OcultarCargando();
            if (data.success == true) {
                if (data.pendientes.length > 0) {
                    var html = "<h4>Documentación pendiente de entregar:</h4>";
                    html += "<ul style='text-align: left;'>";
                    $.each(data.pendientes, function (i, p) {
                        html += "<li>" + p.value + "</li>";
                    });
                    html += "</ul>";
                }
                Mensajes.MostrarOK(html);
            }
            else {
                Mensajes.MostrarError(data.error);
            }
        }
    });
}