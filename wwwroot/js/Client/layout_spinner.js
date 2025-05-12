$(document).ready(function (e) {
    // ShowSpinnerClient();
    LoadWallet();
});


function ShowSpinnerClient() {
    $("#model_spinner_layout_client").modal('show')
    setTimeout(() => {
        HideSpinnerClient();
    }, 300);
}

function HideSpinnerClient() {
    $("#model_spinner_layout_client").modal('hide')
}

function LoadWallet() {
    $.ajax({
        url: "/Account/GetMoneyInWallet",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            $("#total_money_in_wallet").html(`${data} VNĐ`)
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}
