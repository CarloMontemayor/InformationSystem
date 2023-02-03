$(document).ready(function () {
    $.ajax({
        url: "/Home/GetPendingUsers",
        type: "POST",
        success: function (result) {
            if (result != 0) {
                $('#pendingCount').append('<span class="badge badge-pill badge-danger">!</span>');
                $('#pendingCountNumber').append('<span class="badge badge-pill badge-danger">' + result + '</span>');
            }
        }
    });
});