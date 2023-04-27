$(document).ready(function () {

    $('#fechaDesde, #fechaHasta').datetimepicker({
        format: 'DD/MM/YYYY',
        widgetPositioning: {
            horizontal: 'auto',
            vertical: 'auto'
        }
    });

    $("#fechaDesde, #fechaHasta").on("dp.change", function () {
        var desde = $("#fechaDesde").val();
        var hasta = $("#fechaHasta").val();
        $("#acumulado").val("");
        var dt = $("#tbl").DataTable();
        dt.ajax.url("/colegiotest/movimientos/ObtenerListadoIndex?desde=" + desde + "&hasta=" + hasta);
        dt.ajax.reload();
    });

    var oTable = $('#tbl').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/colegiotest/movimientos/ObtenerListadoIndex",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[0, "desc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '17%' },
            { "bSortable": true, "sWidth": '68%' },
            { "bSortable": true, "sWidth": '15%' }
        ],
        "fnRowCallback": function (a, b, c) {
            $("#acumulado").val(b[3].toFixed(2).replace(".", ","));
        }
    });
});