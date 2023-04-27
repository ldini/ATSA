var Mensajes = {
    MostrarError: function (mensaje) {
        noty({
            layout: 'center',
            text: mensaje,
            closeWith: [],
            modal: true,
            buttons: [
                {
                    addClass: 'btn btn-danger', text: 'OK', onClick: function ($noty) {
                        $noty.close();
                    }
                }
            ]
        });
    },
    MostrarOK: function (mensaje) {
        noty({
            layout: 'center',
            text: mensaje,
            closeWith: [],
            modal: true,
            buttons: [
                {
                    addClass: 'btn btn-warning', text: 'OK', onClick: function ($noty) {
                        $noty.close();
                    }
                }
            ]
        });
    },
    MostrarSiNo: function (mensaje, callbackSI, callbackNO) {
        noty({
            layout: 'center',
            text: mensaje,
            closeWith: [],
            modal: true,
            buttons: [
                {
                    addClass: 'btn btn-success', text: 'SI', onClick: function ($noty) {
                        if (callbackSI != undefined) {
                            callbackSI();
                        }
                        $noty.close();
                    }
                },
                {
                    addClass: 'btn btn-danger', text: 'NO', onClick: function ($noty) {
                        if (callbackNO != undefined) {
                            callbackNO();
                        }
                        $noty.close();
                    }
                }
            ]
        });
    },
    MostrarNotificacionIzquierda: function (mensaje, type) {
        noty({
            layout: 'topLeft',
            text: mensaje,
            closeWith: ['click'],
            modal: false,
            type: type
        });
    },
    MostrarNotificacionDerecha: function (mensaje, type) {
        noty({
            layout: 'topRight',
            text: mensaje,
            closeWith: ['click'],
            modal: false,
            type: type
        });
    }
};

$(document).ready(function () {
    jQuery.validator.setDefaults({
        ignore: ":hidden:not(.chosen-select)",
        highlight: function (element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).addClass(errorClass).removeClass(validClass);
            } else {
                $(element).closest('.form-group').removeClass('has-success has-feedback').addClass('has-error has-feedback');
                $(element).closest('.form-group').find('i.fa').remove();
                $(element).closest('.form-group').append('<i class="fa fa-exclamation fa-lg form-control-feedback"></i>');
            }
        },
        unhighlight: function (element, errorClass, validClass) {
            if (element.type === "radio") {
                this.findByName(element.name).removeClass(errorClass).addClass(validClass);
            } else {
                $(element).closest('.form-group').removeClass('has-error has-feedback').addClass('has-success has-feedback');
                $(element).closest('.form-group').find('i.fa').remove();
                $(element).closest('.form-group').append('<i class="fa fa-check fa-lg form-control-feedback"></i>');
            }
        }
    });

    setTimeout(function () {
        $('.decimal').keypress(function (event) {
            var charCode = (event.which) ? event.which : event.keyCode

            if (
                (charCode != 45 || $(this).val().indexOf('-') != -1) && // “-” CHECK MINUS, AND ONLY ONE.
                (charCode != 46 || $(this).val().indexOf('.') != -1) && // “.” CHECK DOT, AND ONLY ONE.
                (charCode < 48 || charCode > 57))
                return false;

            return true;

        }).focusout(function () {
            if ($(this).val() == "") {
                $(this).val("0.00");
            }
        });
    }, 300);
});

function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
        textbox.on(event, function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    });
}

jQuery.fn.ForceNumericOnly =
    function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                // allow backspace, tab, delete, arrows, numbers and keypad numbers ONLY
                return (
                    key == 8 ||
                    key == 9 ||
                    key == 46 ||
                    (key >= 37 && key <= 40) ||
                    (key >= 48 && key <= 57) ||
                    (key >= 96 && key <= 105));
            })
        })
    };

var TypeAhead = {
    constructMap: function (data, map) {
        var objects = [];
        $.each(data, function (i, object) {
            map[object.label] = object;
            objects.push(object.label);
        });
        return objects;
    }
};

function AbrirModalCarrerasCursos() {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/carrerascursos/AbrirModal?carreracursoid=' + 0,
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            $("#divModalCarrerasCursos").html(data);
            $("#divModalCarrerasCursos").find("#modalCarreraCursos").modal('show');
            OcultarCargando();
        }
    });
}

function AbrirModalInscripciones() {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/inscripciones/AbrirModal?inscripcionid=' + 0,
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            $("#divModalInscripciones").html(data);
            $("#divModalInscripciones").find("#modalInscripcion").modal('show');
            OcultarCargando();
        }
    });
}

function AbrirModalGastos() {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/gastos/AbrirModal?gastoid=' + 0,
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            $("#divModalGastos").html(data);
            $("#divModalGastos").find("#modalGastos").modal('show');
            setInputFilter($("#divModalGastos").find("#Monto"), function (value) {
                return /^\d*\,?\d{0,2}$/.test(value);
            });
            OcultarCargando();
        }
    });
}

function AbrirModalCobranza(id, mes, anio) {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/cobranzas/AbrirModal?InscripcionId=' + id + "&Mes=" + mes + "&Anio=" + anio,
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            $("#divModal").html(data);
            $("#divModal").find("#modalCobranza").modal('show');
            OcultarCargando();
        }
    });
}

function AbrirModalCobranzaPagoAdelantado() {
    MostrarCargando();
    $.ajax({
        url: '/colegiotest/cobranzas/AbrirModalPagoAdelantado',
        method: 'GET',
        dataType: 'html',
        success: function (data) {
            $("#divModal").html(data);
            $("#divModal").find("#modalCobranza").modal('show');
            OcultarCargando();
        }
    });
}

function MostrarCargando() {
    $(".cargando").fadeIn();
}

function OcultarCargando() {
    $(".cargando").fadeOut();
}