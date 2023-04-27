$(document).ready(function () {
    $("#FiltroCarreraCurso,#FiltroCicloLectivo").chosen();
    $("#FiltroCarreraCurso,#FiltroCicloLectivo").on('change', function () {
        var idcarreracurso = parseInt($("#FiltroCarreraCurso").val());
        var idciclolectivo = parseInt($("#FiltroCicloLectivo").val());
        var url = ObtenerUrl(idcarreracurso, idciclolectivo);
        $("#tbl").DataTable().ajax.url(url);
        $("#tbl").DataTable().ajax.reload();
    });

    $("#tbl").on('click', '.btnDarBaja', function () {
        var id = $(this).attr('data-id');
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/inscripciones/ModalDarBaja?inscripcionid=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModalBaja").html(data);
                $("#divModalBaja").find("#modalDarBaja").modal('show');
                OcultarCargando();
            }
        });
    });

    $("#tbl").on('click', '.btnEditar', function () {
        var id = $(this).attr('data-id');
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/inscripciones/AbrirModal?inscripcionid=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModalInscripciones").html(data);
                $("#divModalInscripciones").find("#modalInscripcion").modal('show');
                OcultarCargando();
            }
        });
    });

    $("#tbl").on('click', '.btnReinscribir', function () {
        var id = $(this).attr('data-id');
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/inscripciones/AbrirModal?inscripcionid=' + id + "&reinscripcion=True",
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModalInscripciones").html(data);
                $("#divModalInscripciones").find("#modalInscripcion").modal('show');
                OcultarCargando();
            }
        });
    });

    $("#tbl").on('click', '.btnDeBaja', function () {
        var id = $(this).attr('data-id');
        var fecha = $(this).attr('data-fecha');
        var motivo = $(this).attr('data-motivo');
        Mensajes.MostrarOK("<b>Fecha de baja:</b>&nbsp;" + fecha + "<br/><b>Motivo:</b>&nbsp;" + motivo);
    });

    $("#NuevaInscripcion").on('click', function () {
        AbrirModalInscripciones();
    });
    
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/colegiotest/inscripciones/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[1, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '8%' },
            { "bSortable": true, "sWidth": '25%' },
            { "bSortable": true, "sWidth": '8%' },
            { "bSortable": true, "sWidth": '25%' },
            { "bSortable": false, "sWidth": '17%' },
            {
                "bSortable": false, "sWidth": '18%',
                mRender: function (data, type, row) {
                    var html;
                    var id = data;
                    var fechaBaja = row[6];
                    var motivoBaja = row[7];
                    var debeDocs = row[8];
                    var ciclo = row[4];

                    html = "";

                    if (debeDocs) {
                        html += "&nbsp;<a class='btn btn-warning btn-xs' onclick='MostrarDocsPendientes(" + data + ")'>&nbsp;!&nbsp;</a>";
                    }

                    //html += "&nbsp;<a class='btn btn-success btn-xs btnReinscribir' data-id='" + data + "'>Reinscribir</a>";

                    if (ciclo != "COMPLETADO") {
                        if (fechaBaja === null) {
                            html += "&nbsp;<a class='btn btn-primary btn-xs btnEditar' data-id='" + data + "'>Editar</a>";
                            html += "&nbsp;<a class='btn btn-danger btn-xs btnDarBaja' data-id='" + data + "'>Baja</a>";
                        }
                        else {
                            html = "&nbsp;<a class='btn btn-danger btn-xs btnDeBaja' data-id='" + data + "' data-fecha='" + fechaBaja + "' data-motivo='" + motivoBaja + "'>Anulado</a>";
                        }
                    }
                    return html;
                }
            }
        ],

    });
});

function ObtenerUrl(filtrocarreracurso, filtrociclolectivo) {
    return "/colegiotest/inscripciones/ObtenerListadoIndex?filtrocarreracurso=" + filtrocarreracurso + "&filtrociclolectivo=" + filtrociclolectivo;
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