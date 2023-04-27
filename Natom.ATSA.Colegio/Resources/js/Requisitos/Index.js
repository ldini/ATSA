$(document).ready(function () {
    $("#tbl").on('click', '.btnEliminar', function () {
        var id = $(this).attr('data-id');
        Mensajes.MostrarSiNo("¿Desea eliminar el Requisito?", function () {
            MostrarCargando();
            $.ajax({
                url: '/colegiotest/requisitos/EliminarRequisito?requisitoid=' + id,
                method: 'GET',
                dataType: 'html',
                success: function (data) {
                    location.reload();
                }
            });
        });
    });

    $("#tbl").on('click', '.btnEditar', function () {
        var id = $(this).attr('data-id');
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/requisitos/AbrirModal?requisitoid=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalRequisitos").modal('show');
                OcultarCargando();
            }
        });
    });

    $("#NuevoRequisito").on('click', function () {
        MostrarCargando();
        $.ajax({
            url: '/colegiotest/requisitos/AbrirModal?requisitoid=' + 0,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalRequisitos").modal('show');
                OcultarCargando();
            }
        });
    });
    
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": '/colegiotest/requisitos/ObtenerListadoIndex',
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "asc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '90%' },
            {
                "bSortable": false, "sWidth": '10%',
                mRender: function (data, type, row) {
                    var html = "<a class='btn btn-primary btn-xs btnEditar' data-id='" + row[1] + "'>Editar</a> <a class='btn btn-danger btn-xs btnEliminar' data-id='" + row[1] + "'>Eliminar</a>";
                    return html;
                }
            }
        ],

    });
});