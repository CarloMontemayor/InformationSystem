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

    $.ajax({
        url: "/Home/GetPendingComplaint",
        type: "POST",
        success: function (result) {
            if (result != 0) {
                $('#complaintCount').append('<span class="badge badge-pill badge-danger">!</span>');
            }
        }
    });

    $.ajax({
        url: "/Home/GetPendingReports",
        type: "POST",
        success: function (result) {
            if (result != 0) {
                $('#reportCount').append('<span class="badge badge-pill badge-danger">!</span>');
            }
        }
    });

    $.ajax({
        url: "/Home/GetPendingFeedBack",
        type: "POST",
        success: function (result) {
            if (result != 0) {
                $('#feedbackCount').append('<span class="badge badge-pill badge-danger">!</span>');
            }
        }
    });

    $.ajax({
        url: "/Home/GetPendingAccident",
        type: "POST",
        success: function (result) {
            if (result != 0) {
                $('#accidentCount').append('<span class="badge badge-pill badge-danger">!</span>');
            }
        }
    });

});