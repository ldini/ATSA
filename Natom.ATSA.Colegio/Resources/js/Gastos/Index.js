$(document).ready(function () {
    $("#tbl").on('click', '.btnEliminar', function () {
        var id = $(this).attr('data-id');
        Mensajes.MostrarSiNo("¿Desea eliminar?", function () {
            MostrarCargando();
            $.ajax({
                url: '/colegiotest/gastos/EliminarGasto?gastoid=' + id,
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
        $.ajax({
            url: '/colegiotest/gastos/AbrirModal?gastoid=' + id,
            method: 'GET',
            dataType: 'html',
            success: function (data) {
                $("#divModal").html(data);
                $("#divModal").find("#modalGastos").modal('show');
                setInputFilter($("#divModal").find("#Monto"), function (value) {
                    return /^\d*\,?\d{0,2}$/.test(value);
                });
            }
        });
    });

    $("#NuevoGasto").on('click', function () {
        AbrirModalGastos();
    });
    
    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/colegiotest/gastos/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[2, "desc"]],
        "aoColumns": [
            {
                "bSortable": true, "sWidth": '68%',
                mRender: function (data, type, row) {
                    var tipo = row[4];
                    var html = "";
                    if (tipo == 1)
                        html = "<b style='color:#3c8d3c;'>" + data + "</b>";
                    else
                        html = "<b style='color:red;'>" + data + "</b>";
                    return html;
                }
            },
            {
                "bSortable": true, "sWidth": '10%',
                mRender: function (data, type, row) {
                    var tipo = row[4];
                    var html = "";
                    if (tipo == 1)
                        html = "<b style='color:#3c8d3c; float:right; margin-right:15px;'>" + data + "</b>";
                    else
                        html = "<b style='color:red; float:right; margin-right:15px;'>- " + data + "</b>";
                    return html;
                }
            },
            { "bSortable": true, "sWidth": '15%' },
            {
                "bSortable": false, "sWidth": '7%',
                mRender: function (data, type, row) {
                    var html = "";//"<a class='btn btn-primary btn-xs btnEditar' data-id='" + row[3] + "'>Editar</a>";
                    html += "<a class='btn btn-danger btn-xs btnEliminar' data-id='" + row[3] + "'>Eliminar</a>";
                    return html;
                }
            }
        ],

    });
});