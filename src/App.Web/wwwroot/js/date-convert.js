var monthNames = ["January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December"];
var my_date = {
    
    date_to_db : function(val){
        var arr = val.split(" ");
        var month = monthNames.indexOf(arr[1]);
        month = parseInt(month) + 1;
        if (parseInt(month) < 10) {
            month = "0" + month;
        }
        return arr[2] + "-" + month + "-" + arr[0];
    },

    date_to_view: function (val) {
        var arr = val.split("-");
        return arr[0] + " " + monthNames[arr[1]] + " " + arr[2];
    }

};