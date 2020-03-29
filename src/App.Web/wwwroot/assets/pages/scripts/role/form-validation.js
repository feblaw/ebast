var FormValidation = function () {
    // advance validation
    var handleValidation = function (id) {
        // for more info visit the official plugin documentation: 
        // http://docs.jquery.com/Plugins/Validation

        var form = $(id);
        var error = $('.alert-danger', form);
        var success = $('.alert-success', form);

        //IMPORTANT: update CKEDITOR textarea with actual content before submit
        form.on('submit', function () {
            console.log(CKEDITOR.instances);
            for (var instanceName in CKEDITOR.instances) {
                CKEDITOR.instances[instanceName].updateElement();
            }
        })

        form.validate({
            errorElement: 'span', //default input error message container
            errorClass: 'help-block help-block-error', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "", // validate all fields including form hidden input
            rules: {
                Description: {
                    required: true
                }
            },
            errorPlacement: function (error, element) { // render error placement for each input type
                if (element.parents('.mt-radio-list') || element.parents('.mt-checkbox-list')) {
                    if (element.parents('.mt-radio-list')[0]) {
                        error.appendTo(element.parents('.mt-radio-list')[0]);
                    }
                    if (element.parents('.mt-checkbox-list')[0]) {
                        error.appendTo(element.parents('.mt-checkbox-list')[0]);
                    }
                } else if (element.parents('.mt-radio-inline') || element.parents('.mt-checkbox-inline')) {
                    if (element.parents('.mt-radio-inline')[0]) {
                        error.appendTo(element.parents('.mt-radio-inline')[0]);
                    }
                    if (element.parents('.mt-checkbox-inline')[0]) {
                        error.appendTo(element.parents('.mt-checkbox-inline')[0]);
                    }
                } else if (element.parent(".input-group").size() > 0) {
                    error.insertAfter(element.parent(".input-group"));
                } else if (element.attr("data-error-container")) {
                    error.appendTo(element.attr("data-error-container"));
                } else {
                    error.insertAfter(element); // for other inputs, just perform default behavior
                }
            },
            invalidHandler: function (event, validator) { //display error alert on form submit   
                success.hide();
                error.show();
                App.scrollTo(error, -200);
            },
            highlight: function (element) { // hightlight error inputs
                $(element)
                     .closest('.form-group').addClass('has-error'); // set error class to the control group
            },
            unhighlight: function (element) { // revert the change done by hightlight
                $(element)
                    .closest('.form-group').removeClass('has-error'); // set error class to the control group
            },
            success: function (label) {
                label
                    .closest('.form-group').removeClass('has-error'); // set success class to the control group
            },
            submitHandler: function (form) {
                success.show();
                error.hide();
                form[0].submit(); // submit the form
            }
        });
    }

    return {
        //main function to initiate the module
        init: function (id) {
            handleValidation(id);
        }
    };
}();