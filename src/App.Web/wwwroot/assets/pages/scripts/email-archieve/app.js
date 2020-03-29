var EmailArchieves = function () {
    var detailAction = function (e, elementId, detailUrl, current) {
        e.preventDefault();
        var table = $(elementId).DataTable();
        var data = table.row(current.parents('tr')).data();
        var modal = bootbox.dialog({
            title: 'Detail Email Archieve',
            message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
            backdrop: true,
            onEscape: true,
            size: "large"
        });

        modal.init(function () {
            $.ajax({
                url: detailUrl + "/" + data.id,
                success: function (resp) {
                    modal.find('.bootbox-body').html(resp);
                }
            })
        });
    }

    var init = function (detailUrl, dateformat) {
        $common.setMenu("#menu-email");

        var datatableOption = {
            ajaxMethod: "POST",
            listUrl: "/admin/api/emails/postdatatables",
            detailUrl: detailUrl,
            columnDefs: [
                         {
                             "targets": -1,
                             "data": null,
                             "orderable": false,
                             render: function (data, type, dataObj) {
                                 var html = "<button class='btn btn-success detail' title='Detail Setting'><i class='fa fa-info'></i></button>";
                                 if (!dataObj.isSent && dataObj.trySentCount == 0)
                                     html += "<button class='btn btn-primary resend' title='Send email'><i class='fa fa-send'></i> Send</button>";
                                 else if (!dataObj.isSent)
                                     html += "<button class='btn btn-info resend' title='Resend email'><i class='fa fa-send-o'></i> Resend</button>";

                                 return html;
                             }
                         },
                         {
                             "targets": 0,
                             "data": "from",
                             name: "From"
                         },
                         {
                             "targets": 1,
                             "data": "tos",
                             name: "Tos"
                         },
                         {
                             "targets": 2,
                             "data": "subject",
                             name: "Subject"
                         },
                         {
                             "targets": 3,
                             "data": "isSent",
                             name: "IsSent",
                             render: function (data, type, dataObj) {
                                 if (data)
                                     return "<i class='fa fa-check font-green-jungle'></i>"

                                 return "<i class='fa fa-times font-red-intense'></i>";
                             }
                         },
                         {
                             "targets": 4,
                             "data": "sentDate",
                             name: "SentDate",
                             render: function (data, type, dataObj) {
                                 if (data) {
                                     var date = moment(data);
                                     return date.format(dateformat);
                                 }

                                 return "";
                             }
                         }
            ],
            detailAction: detailAction
        }

        $datatables.init("#table-email", datatableOption);

        $("#table-email").on("click", "td > .resend", function (e) {
            e.preventDefault();
            var table = $("#table-email").DataTable();
            var data = table.row($(this).parents('tr')).data();
            var id = data.id;
            var uri = "/admin/api/emails/sendemail/" + id;

            bootbox.confirm("Are you sure want to send this email?", function (result) {
                if (result) {
                    $.ajax({
                        method: "POST",
                        url: uri,
                        success: function (resp) {
                            if (resp.isSent) {
                                $alert.success("Email has been sent successfully.");
                            } else {
                                $alert.error("Failed to sent email.")
                            }
                            table.ajax.reload();
                        },
                        beforeSend: function () {
                            App.blockUI({
                                boxed: true
                            });
                        },
                        complete: function () {
                            App.unblockUI();
                        }
                    });
                }
            });
           
        });
    }

    return {
        //main function to initiate the module
        init: function (detailUrl, dateformat) {
            init(detailUrl, dateformat);
        }
    };
}();