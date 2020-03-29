var TableDatatablesFixedHeader = function () {
    var initTable1 = function () {
        var table = $('#table');
        var fixedHeaderOffset = 0;
        if (App.getViewPort().width < App.getResponsiveBreakpoint('md')) {
            if ($('.page-header').hasClass('page-header-fixed-mobile')) {
                fixedHeaderOffset = $('.page-header').outerHeight(true);
                console.log("1");
            }
        } else if ($('.page-header').hasClass('navbar-fixed-top')) {
            fixedHeaderOffset = $('.page-header').outerHeight(true);
            console.log("2");
        } else if ($('body').hasClass('page-header-fixed')) {
            fixedHeaderOffset = 50; // admin 5 fixed height
            console.log("3");
        }
        var wh = $(window).height(),
              th = 55, // table header height
              scrollY = Math.floor((wh - th) / wh * 100);
        var oTable = table.dataTable({
            // Internationalisation. For more info refer to http://datatables.net/manual/i18n
            "language": {
                "aria": {
                    "sortAscending": ": activate to sort column ascending",
                    "sortDescending": ": activate to sort column descending"
                },
                "emptyTable": "No data available in table",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "infoEmpty": "No entries found",
                "infoFiltered": "(filtered1 from _MAX_ total entries)",
                "lengthMenu": "_MENU_ entries",
                "search": "Search:",
                "zeroRecords": "No matching records found"
            },
            // Or you can use remote translation file
            //"language": {
            //   url: '//cdn.datatables.net/plug-ins/3cfcc339e89/i18n/Portuguese.json'
            //},
            // setup rowreorder extension: http://datatables.net/extensions/fixedheader/
            //fixedHeader: {
            //    header: true,
            //    headerOffset: fixedHeaderOffset
            //},
            "scrollY": scrollY + "vh",
            "scrollX": true,
            "scrollCollapse": true,
            "fixedColumns": {
                leftColumns: 1,
            },
            "paging": false,
            "bFilter": false,
            "ordering": false,
            "buttons": false,
            "info": false
            // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
            // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.js). 
            // So when dropdowns used the scrollable div should be removed. 
            //"dom": "<'row' <'col-md-12'T>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r>t<'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>",
        });
    }
    return {
        //main function to initiate the module
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }
            initTable1();
        }
    };
}();
jQuery(document).ready(function () {

});