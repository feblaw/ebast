var $common = function () {
    var setActiveMenu = function (elementId, childId) {
        $(".page-sidebar-menu").find(".nav-item.active").removeClass("active");
        $(elementId + ".nav-item").addClass("active open");
        if (childId != undefined) {
            $(childId).addClass("active open");
        }
    }

    return {
        setMenu: function (id, childId) {
            setActiveMenu(id, childId);
        }
    };
}();