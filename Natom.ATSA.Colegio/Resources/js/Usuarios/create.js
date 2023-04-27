$(document).ready(function () {
    $("form").validate({
        rules: {
            Nombre: {
                required: true,
                maxlength: 50
            },
            Apellido: {
                required: true,
                maxlength: 50
            },
            Email: {
                required: true,
                email: true,
                minlength: 5,
                maxlength: 100
            }
        },
        errorPlacement: function (error, element) {
            if (element.is("select")) {
                error.insertAfter("#" + element.attr("id") + "_chosen");
            } else {
                error.insertAfter(element);
            }
        }
    });
   
});

function Grabar() {
    if (!$("form").valid()) {
        return;
    }

    var obj = {
        UsuarioId: parseInt($("#UsuarioId").val()),
        Nombre: $("#Nombre").val(),
        Apellido: $("#Apellido").val(),
        PuedeCargarExcelFacturas: $("#PuedeCargarExcelFacturas").is(":checked"),
        PuedeEliminarExcelFacturas: $("#PuedeEliminarExcelFacturas").is(":checked"),
        PuedeCargarExcelAuditoria: $("#PuedeCargarExcelAuditoria").is(":checked"),
        PuedeEliminarExcelAuditoria: $("#PuedeEliminarExcelAuditoria").is(":checked"),
        PuedeAuditarFactura: $("#PuedeAuditarFactura").is(":checked"),
        PuedeAuditarConsiliacion: $("#PuedeAuditarConsiliacion").is(":checked"),
        PuedeVerAuditorias: $("#PuedeVerAuditorias").is(":checked"),
        PuedeDescargarExcelAuditoria: $("#PuedeDescargarExcelAuditoria").is(":checked"),
        Email: $("#Email").val()
    };
    
    $.ajax({
        type: "POST",
        url: '/colegiotest/usuarios/Grabar',
        data: JSON.stringify({
            usuario: obj
        }),
        dataType: "json",
        contentType: "application/json",
        beforeSend: function (xhr) {
            $.blockUI({ message: '<h4>Procesando...</h4>' });
        },
        success: function (data, status) {
            if (status == "success") {
                if (data.success) {
                    location.href = "/colegiotest/usuarios";
                }
                else {
                    Mensajes.MostrarError(data.error);
                    $.unblockUI();
                }
            }
            else {
                Mensajes.MostrarError("Se ha producido un error. Comuníquese con el administrador de ATSA.");
                $.unblockUI();
            }
        }
    });
    
}