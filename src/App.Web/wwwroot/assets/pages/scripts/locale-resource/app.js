var LocaleResources = function () {
    var ajaxCall = function (form, text) {
        var $this = $(form);

        var validationContainer = $this.find('#validation-summary');
        var data = {};

        $.each($this.serializeArray(), function (index, item) {
            data[item.name] = item.value;
        });

        $.ajax({
            url: $this.attr("action"),
            method: $this.attr("method"),
            data: data,
            success: function (resp) {
                var tblData = $("#table-language").DataTable();
                validationContainer.hide();
                tblData.ajax.reload();
                $this.closest(".bootbox.modal").find(".bootbox-close-button").trigger("click");
                $alert.success("Resource " + data.ResourceName + " has been " + text);
            },
            error: function (resp) {
                validationContainer.html("");
                validationContainer.addClass("alert alert-danger");
                if (resp.responseJSON) {
                    var obj = resp.responseJSON;
                    var html = "";

                    $.each(obj, function (i, item) {
                        $.each(item, function (index, text) {
                            html += text + "<br/>";
                        });
                    });

                    validationContainer.html(html);
                } else {
                    validationContainer.html(resp.responseText);
                }
            }
        });
    }

    var editAction = function (e, elementId, editUrl, current) {
        e.preventDefault();
        var table = $(elementId).DataTable();
        var data = table.row(current.parents('tr')).data();
        var uri = editUrl + "?name=" + data.resourceName;
        var modal = bootbox.dialog({
            title: 'Edit Resource ' + data.resourceName,
            message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
            backdrop: true,
            onEscape: true
        })
        .on('shown.bs.modal', function () {
            var options = {
                success: function (form) {
                    ajaxCall(form, "updated");
                }
            }

            FormValidation.init("#ResourceEdit", options);
        });

        modal.init(function () {
            $.ajax({
                url: uri,
                success: function (resp) {
                    modal.find('.bootbox-body').html(resp);
                }
            })
        });
    }

    var init = function (detailUrl, editUrl, createUrl) {
        $common.setMenu("#menu-globalization", "#menu-locale-resource");

        var language = $("#Languages");
        var selectedLanguage;
        selectedLanguage = language.val();

        var datatableOption = {
            ajaxMethod: "POST",
            listUrl: "/admin/api/localeresources/postdatatables/" + selectedLanguage,
            detailUrl: detailUrl,
            editUrl: editUrl,
            deleteUrl: "/admin/api/localeresources/",
            deleteAlertSuccess: "Language has been removed",
            columnDefs: [
                     {
                         "targets": -1,
                         "data": null,
                         "defaultContent": "<button class='btn btn-primary edit' title='Edit Language'><i class='fa fa-edit'></i></button>" +
                             "<button class='btn btn-danger remove' title='Remove Language'><i class='fa fa-trash'></i></button>",
                         "orderable": false
                     },
                     {
                         "targets": 0,
                         "data": "resourceName",
                         name: "ResourceName"
                     },
                     {
                         "targets": 1,
                         "data": "resourceValue",
                         name: "ResourceValue"
                     }
            ],
            removeSuccess: function () {
                var tblData = $("#table-language").DataTable();
                tblData.ajax.url("/admin/api/localeresources/postdatatables/" + selectedLanguage).load();
            },
            editAction: editAction
        }

        $datatables.init("#table-language", datatableOption);

        language.on("change", function () {
            selectedLanguage = language.val();
            var table = $("#table-language").DataTable();
            table.ajax.url("/admin/api/localeresources/postdatatables/" + selectedLanguage);
            table.ajax.reload();
        });

        $('.btn-add').on('click', function (e) {
            e.preventDefault();

            var uri = createUrl;

            var modal = bootbox.dialog({
                title: 'Add New Resource',
                message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>'
            })
            .on('shown.bs.modal', function () {
                var options = {
                    success: function (form) {
                        ajaxCall(form, "added");
                    }
                }

                FormValidation.init("#ResourceCreate", options);
            });

            modal.init(function () {
                $.ajax({
                    url: uri,
                    success: function (resp) {
                        modal.find('.bootbox-body').html(resp);
                    }
                })
            });
        });
    }

    return {
        //main function to initiate the module
        init: function (detailUrl, editUrl, createUrl) {
            init(detailUrl, editUrl, createUrl);
        }
    };
}();