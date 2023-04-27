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

    $("#tbl").on('click', '.btnEditar', function () {
        var id = $(this).attr('data-id');
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/cicloslectivos/AbrirModal?CicloLectivoId=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalCicloLectivo").modal('show');
                OcultarCargando();
            }
        });
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
            "url": "/colegiotest/cicloslectivos/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '40%' },
            { "bSortable": true, "sWidth": '15%' },
            { "bSortable": true, "sWidth": '15%' },
            { "bSortable": true, "sWidth": '15%' },
            {
                "bSortable": false, "sWidth": '15%',
                mRender: function (data, type, row) {
                    var html = "<a class='btn btn-primary btn-xs btnEditar' data-id='" + data + "'>Editar</a> <a class='btn btn-danger btn-xs btnEliminar' data-id='" + data + "'>Eliminar</a>";
                    return html;
                }
            }
        ],

    });
});
