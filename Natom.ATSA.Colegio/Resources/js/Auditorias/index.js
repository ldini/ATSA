$(document).ready(function () {
    var filtro = $("#FiltroEstado").val();
    if (filtro.length > 0) {
        filtro = "?filtro=" + filtro;
    }
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/Auditorias/ObtenerListadoIndex" + filtro,
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[6, "desc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '14%' },
            { "bSortable": true, "sWidth": '9%' },
            { "bSortable": true, "sWidth": '8%' },
            { "bSortable": true, "sWidth": '24%' },
            { "bSortable": true, "sWidth": '9%' },
            { "bSortable": true, "sWidth": '15%' },
            {
                "bSortable": false, "sWidth": '21%',
                mRender: function (data, type, row) {
                    var html = "";
                    var puede = row[7];
                    var esAuditoria = row[8];
                    var esConsiliacion = row[9];
                    var bajoExcelConsiliacion = row[10];
                    var cargoExcelConsiliacion = row[11];

                    if (esAuditoria) {
                        html += '<a class="btn btn-primary btn-xs btn-auditar" id="' + data + '">Auditar</a>';
                    }
                    if (esConsiliacion) {
                        if (!cargoExcelConsiliacion) {
                            html += '<a class="btn btn-success btn-xs btn-bajar-excel" id="' + data + '" title="Bajar Excel consiliación"><span class="glyphicon glyphicon-download"></span> excel</a>';
                            if (bajoExcelConsiliacion) {
                                html += '<a class="btn btn-warning btn-xs btn-subir-excel" id="' + data + '" title="Subir Excel consiliación"><span class="glyphicon glyphicon-upload"></span> excel</a>';
                            }
                        }
                        else {
                            html += '<a class="btn btn-warning btn-xs btn-consiliar" id="' + data + '">Consiliar</a>';
                        }
                    }
                    
                    return html;
                }
            }
        ],

    });

    $("#tbl").on("click", ".btn-eliminar", function (e) {
        var id = e.currentTarget.id;
        var puede = $(this).is("[puede]");
        if (puede) {
            Mensajes.MostrarSiNo("¿Desea eliminar la carga?", function () {
                alert("Eliminar");
                $("#tbl").DataTable().ajax.url("/colegiotest/cargas/ObtenerListadoIndex");
                $("#tbl").DataTable().ajax.reload();
            });
        }
        else {
            Mensajes.MostrarError("No puedes eliminar la carga porque ya se encuentra en auditoría.");
        }
    });

    $("#tbl").on("click", ".btn-auditar", function (e) {
        var id = e.currentTarget.id;
        location.href = "/Auditorias/VerAuditoria?cargaid=" + id;
    });

    $("#tbl").on("click", ".btn-consiliar", function (e) {
        var id = e.currentTarget.id;
        location.href = "/Auditorias/VerConsiliacion?cargaid=" + id;
    });
});