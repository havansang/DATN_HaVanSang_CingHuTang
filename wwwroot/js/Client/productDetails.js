var ProductDetailsId = 0;
$(document).ready(function (e) {
    $("#lst-product-size li:first a").click();
});

function ChangeSize(event) {
    let el = $(event.target);
    let price = $(el).attr("price");
    ProductDetailsId = $(el).attr("ProductDetailsId");
    price = price.replace(/[^0-9]/g, '');
    price = price.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    $("#Price").html(`${price} VNĐ`)
}

function showToast(msg, success = true) {
    $('#toast-message')
        .css('background', success ? '#28a745' : '#dc3545')
        .text(msg)
        .fadeIn(200)
        .delay(1500)
        .fadeOut(400);
}


function AddCart(event) {
    let accountId = $("#account_id").val();
    if (accountId <= 0) {
        alert("Hãy đăng nhập để sử dụng được chức năng!");
        return;
    }
    if (ProductDetailsId <= 0) {
        alert("Hãy chọn size sản phẩm!");
        return;
    }
    let quantity = $("#product-quanity").val();
    let toppingIDs = [];
    $(".input-topping").each(function (index, el) {
        let isChecked = $(el).is(':checked');
        if (isChecked) {
            let toppingID = parseInt($(el).attr("topping-id"));
            toppingIDs.push(toppingID);
        }
    });


    let object = {
        ProductDetailID: ProductDetailsId,
        AccountID: accountId,
        Quantity: quantity,
        ToppingIDs: toppingIDs
    };
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(object),
        success: function (result) {
            if (parseInt(result.status) == 1) {
                showToast(result.message, parseInt(result.status) == 1);
            } else {
                alert("Lỗi hệ thống!");
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}

function changeImage(event) {
    let el = $(event.target);
    let imgSrc = $(el).attr("src");
    $("#product-image").attr("src", imgSrc);
}