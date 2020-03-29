var $alert = function () {
    var $toast;

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "1000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    var createToastr = function (type, text, title) {
        toastr[type](text, title);
    }

    return {
        success: function (text, title) {
            createToastr("success", text, title);
        },
        error: function (text, title) {
            createToastr("error", text, title);
        },
        info: function (text, title) {
            createToastr("info", text, title);
        },
        warning: function (text, title) {
            createToastr("warning", text, title);
        }
    };
}();