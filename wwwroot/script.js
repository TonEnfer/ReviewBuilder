
function showAlert(name) {
    $("#readyAlert").collapse('hide');
    $("#errorAlert").collapse('hide');
    $("#processingAlert").collapse('hide');
    $("#incorrectFileAlert").collapse('hide');

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
        case 'incorrectFile':
            $("#incorrectFileAlert").collapse('show');
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

    var data = new FormData();
    data.append("files", $("#files")[0].files[0]);

    $.ajax("api/ReviewBuilder/UploadFiles",
        {
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            dataType: 'json'
        }
    ).done(function (data) { showAlert('processing'); showModal(data.id); }
    ).fail(function () { showAlert('incorrectFile'); });

    // $("#modalReadyAlert").collapse('show');

});

$("#tokenSubmitForm").on('submit', function (ev) {
    status = requestStatusFromServer($("#inputToken").val());
    showAlert(status);
    ev.preventDefault();
});
