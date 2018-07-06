
function showAlert(name) {
    $("#readyAlert").collapse('hide');
    $("#errorAlert").collapse('hide');
    $("#processingAlert").collapse('hide');

    switch (name) {
        case 'ready':
            $("#readyAlert").collapse('show');
            break;
        case 'error':
            $("#errorAlert").collapse('show');
            break;
        case 'processing':
            $("#processingAlert").collapse('show');
            break;
    }
    return false;
}

function showModal(token) {
    $("#acquiredToken").text(token);
    $("#loadSuccessModal").modal('show');
}

function requestStatusFromServer(val) {
    return 0;
}

$("#fileSubmitForm").on('submit', function (ev) {
    ev.preventDefault();
    showModal("133asd");

    let formdata = new FormData($("#fileSubmitForm"));


    $.post(
        "api/ReviewBuilder/UploadFiles"
    );
    // $("#fileSumbitForm").ajaxSubmit({ url: "/api/ReviewBuilder/UploadFiles", type: "post" });
    // $("#modalReadyAlert").collapse('show');

});

$("#tokenSubmitForm").on('submit', function (ev) {
    status = requestStatusFromServer($("#inputToken").val());
    showAlert(status);
    ev.preventDefault();
});
