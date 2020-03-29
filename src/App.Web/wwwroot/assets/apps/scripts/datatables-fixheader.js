var $datatables = function () {
    var defaultOptions = {
        removeSuccess: function (url, table) {
            table.ajax.url(url).load();
        },
        editAction: function (e, elementId, editUrl, current) {
            e.preventDefault();
            var table = $(elementId).DataTable();
            var data = table.row(current.parents('tr')).data();
            var id = data.id;
            window.location.href = editUrl + id;
        },
        detailAction: function (e, elementId, detailUrl, current) {
            e.preventDefault();
            var table = $(elementId).DataTable();
            var data = table.row(current.parents('tr')).data();
            var id = data.id;
            window.location.href = detailUrl + id;
        }
    };

    var init = function (elementId, option) {
        option = $.extend(defaultOptions, option);

        var fixedHeaderOffset = 0;
        if (App.getViewPort().width < App.getResponsiveBreakpoint('md')) {
            if ($('.page-header').hasClass('page-header-fixed-mobile')) {
                fixedHeaderOffset = $('.page-header').outerHeight(true);
            }
        } else if ($('.page-header').hasClass('navbar-fixed-top')) {
            fixedHeaderOffset = $('.page-header').outerHeight(true);
        } else if ($('body').hasClass('page-header-fixed')) {
            fixedHeaderOffset = 64; // admin 5 fixed height
        }

        $(elementId)
           .DataTable({
               "processing": false,
               "serverSide": true,
               "info": true,
               "infoCallback": function (settings, start, end, max, total, pre) {
                   return "Showing " + start + " to " + end + " of " + total + " entries";
                   //return start + " - " + end + " of " + total;
               },
               "ajax": {
                   "url": option.listUrl,
                   "type": option.ajaxMethod || "GET"
               },
               "columnDefs": option.columnDefs,
               "stateSave": true,
               "language": {
                   processing: '<i class="fa fa-refresh fa-spin fa-5x fa-fw margin-bottom" style="color:#2b3643; z-index:10;"></i><span class="sr-only">Loading..n.</span>'
               },
               "orderCellsTop": option.orderCellsTop,
               "initComplete": option.initComplete,
               "fnPreDrawCallback": function () {
                   $.LoadingOverlay("show");
                   return true;
               },
               "fnDrawCallback": function () {
                   $.LoadingOverlay("hide", true);
               },
               scrollY: "600px",
               scrollX: true,
               scrollCollapse: true,
               fixedColumns: {
                   leftColumns: option.fixedColumns,
               },
               //fixedHeader: {
               //    header: true,
               //    footer: true,
               //    headerOffset: fixedHeaderOffset
               //},
           });

        $(elementId).on('click', 'td > .remove', function (e) {
            var table = $(elementId).DataTable();
            var data = table.row($(this).parents('tr')).data();
            var id = data.id;
            e.preventDefault();// used to stop its default behaviour
            bootbox.confirm("Are you sure remove this item?", function (result) {
                if (result === true) {
                    $.ajax({
                        url: option.deleteUrl + id,
                        type: "DELETE",
                        timeout: 60000,
                        dataType: "json",
                        contentType: "application/json",
                        success: function (response) {
                            $alert.success(option.deleteAlertSuccess);
                            option.removeSuccess(option.listUrl, table);
                        },
                        error: function (response) {
                            $alert.warning("Attention, the record cannot deleted !!");
                        },
                        beforeSend: function () {
                            $("#body-overlay").removeClass("hide");
                        },
                        complete: function () {
                            $("#body-overlay").addClass("hide");
                        }
                    });
                };
            });
        });


        $(elementId).on('click', 'td > .detail', function (e) {
            option.detailAction(e, elementId, option.detailUrl, $(this));
            //e.preventDefault();
            //var table = $(elementId).DataTable();
            //var data = table.row($(this).parents('tr')).data();
            //var id = data.id;
            //window.location.href = option.detailUrl + id;
        });

        $(elementId).on('click', 'td > .edit', function (e) {
            option.editAction(e, elementId, option.editUrl, $(this));
            //e.preventDefault();
            //var table = $(elementId).DataTable();
            //var data = table.row($(this).parents('tr')).data();
            //var id = data.id;
            //window.location.href = option.editUrl + id;
        });

        return $(elementId).DataTable();
    }

    return {
        init: function (id, option) {
            init(id, option);
        }
    };
}();