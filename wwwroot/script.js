
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

var timerId = undefined;

var tokenId = undefined;

function showModal(token) {
    $("#acquiredToken").text(token);
    $("#loadSuccessModal").modal('show');
    tokenId = token;
}

$("#loadSuccessModal").on('shown.bs.modal', function () {
    startWaitingTimer();
});
$("#loadSuccessModal").on('hidden.bs.modal', function () {
    stopWaitingTimer();
});

function checkReadyModal() {
    if(tokenId === undefined){
        console.log("Something went wrong. Token id is not defined");
        return;
    }
    $.ajax("api/ReviewBuilder/IsReady/" + tokenId, {
        method: 'GET',
        dataType: 'json'
    }
    ).done(function (data) {
        console.log(JSON.stringify(data));
        if (data.isReady) {
            $("#modalDownloadLink").prop('href', "api/ReviewBuilder/GetFiles/" + tokenId);;
            $("#modalReadyAlert").collapse('show');
            stopWaitingTimer();
            return;
        }
        showAlert('processing');
    }
    ).fail(function () { showAlert('error'); stopWatchingTimer(); $("#loadSuccessMoadl").modal('hide'); }); 
}

function startWaitingTimer() {
    if (timerId === undefined) {
        timerId = setInterval(checkReadyModal, 1000);
        console.log("Starting timer " + timerId);
        return;
    }
    console.log("Timer already started");
}
function stopWaitingTimer() {
    if (timerId !== undefined) {
        clearInterval(timerId);
        timerId = undefined;
        console.log("Timer stopped ...");
    }
    console.log("No timer to stop...");
}

function requestStatusFromServer(val) {
    $.ajax("api/ReviewBuilder/IsReady/" + val, {
        method: 'GET',
        dataType: 'json'
    }
    ).done(function (data) {
        if (data.isReady) {
            $("#downloadLink").prop('href', "api/ReviewBuilder/GetFiles/" + val); showAlert('ready');
            return;
        }
        showAlert('processing');
    }
    ).fail(function () { showAlert('error'); });
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


});
$("#tokenSubmitForm").on('submit', function (ev) {
    status = requestStatusFromServer($("#inputToken").val());
    ev.preventDefault();
});
