$(document).ready(function (e) {
   // ShowSpinnerClient();
});


function ShowSpinnerClient() {
    $("#model_spinner_layout_client").modal('show')
    setTimeout(() => {
        HideSpinnerClient();
    }, 500);
}

function HideSpinnerClient() {
    $("#model_spinner_layout_client").modal('hide')
}
