$(document).ready(function () {
    var cargaid = $("#CargaId").val();
    var btnIdAutoIncrement = 0;
    var oTable = $('#tblPendientes').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/Auditorias/ObtenerListadoFacturas?cargaid=" + cargaid + "&auditada=False",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[4, "desc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '15%' },
            { "bSortable": true, "sWidth": '15%' },
            {
                "bSortable": false, "sWidth": '10%',
                mRender: function (data, type, row) {
                    var html = '<a class="btn btn-primary btn-xs btn-auditar btnAuditarFactura" onclick="AbrirModal(' + row[5] + ')" data-facturaid="' + row[5] + '" data-btnid="' + btnIdAutoIncrement + '">Auditar</a>';

                    btnIdAutoIncrement++;

                    return html;
                }
            }
        ]
    });

    var aTable = $('#tblAuditados').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "ajax": {
            "url": "/Auditorias/ObtenerListadoFacturas?cargaid=" + cargaid + "&auditada=True",
            "type": "POST"
        },
        "bLengthChange": true,
        "bFilter": true,
        "bSort": true,
        "bInfo": true,
        "order": [[4, "desc"]],
        "aoColumns": [
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' },
            { "bSortable": true, "sWidth": '20%' }
        ]
    });

});

function AbrirModal(e) {
    var facturaid = e;
    var btnid = $(this).attr('data-btnid');
    $.ajax({
        url: 'Auditorias/ModalCrearAuditoria',
        method: 'GET',
        data: { facturaid: facturaid },
        dataType: 'json',
        success: function (data) {
            $('#ModalDiv').html(data);
        }
    })
});