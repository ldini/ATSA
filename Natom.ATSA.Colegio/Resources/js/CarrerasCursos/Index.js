$(document).ready(function () {
    $("#FiltroTipo,#FiltroCursada").chosen();
    $("#FiltroTipo,#FiltroCursada").on('change', function () {
        var idtipo = parseInt($("#FiltroTipo").val());
        var idcursada = parseInt($("#FiltroCursada").val());
        var url = ObtenerUrl(idtipo, idcursada);
        $("#tbl").DataTable().ajax.url(url);
        $("#tbl").DataTable().ajax.reload();
    });

    $("#tbl").on('click', '.btnEliminar', function () {
        var id = $(this).attr('data-id');
        Mensajes.MostrarSiNo("¿Desea eliminar la Carrera / Curso?", function () {
            MostrarCargando();
            $.ajax({
                url: '/colegiotest/carrerascursos/EliminarCarreraCurso?carreracursoid=' + id,
                method: 'GET',
                dataType: 'html',
                success: function (data) {
                    location.reload();
                }
            });
        });
    });

    $("#tbl").on('click', '.btnEditar', function () {
        MostrarCargando();
        var id = $(this).attr('data-id');
        $.ajax({
            url: '/colegiotest/carrerascursos/AbrirModal?carreracursoid=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalCarreraCursos").modal('show');
                OcultarCargando();
            }
        });
    });

    $("#NuevaCarreraCurso").on('click', function () {
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/carrerascursos/AbrirModal?carreracursoid=' + 0,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalCarreraCursos").modal('show');
                OcultarCargando();
            }
        });
    });

    var filtro = $("#FiltroEstado").val();
    if (filtro.length > 0) {
        filtro = "?filtro=" + filtro;
    }
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/colegiotest/carrerascursos/ObtenerListadoIndex" + filtro,
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '45%' },
            { "bSortable": true, "sWidth": '25%' },
            { "bSortable": true, "sWidth": '20%' },
            {
                "bSortable": false, "sWidth": '10%',
                mRender: function (data, type, row) {
                    var html = "<a class='btn btn-primary btn-xs btnEditar' data-id='" + row[3] + "'>Editar</a> <a class='btn btn-danger btn-xs btnEliminar' data-id='" + row[3] + "'>Eliminar</a>";
                    return html;
                }
            }
        ],

    });
});

function ObtenerUrl(filtrotipo, filtrocursada) {
    return "/colegiotest/carrerascursos/ObtenerListadoIndex?filtrotipo=" + filtrotipo + "&filtrocursada=" + filtrocursada;
}