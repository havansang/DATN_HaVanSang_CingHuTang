$(document).ready(function (e) {
    GetAll();
});


function GetAll() {
    $.ajax({
        url: "/Cart/GetCartByAccountId",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var html = '';
            if (data.status == 2) {
                html = `<h1 class="text-warning">${data.massage}</h1>`;
            }
            else if (data.status == 1) {
                $.each(data.data, function (index, item) {
                    let htmlTopping = '';
                    let toppingPrice = 0;
                    $.each(item.lstToppings, function (toppingIndex, toppingItem) {
                        htmlTopping += `<p class="mb-0 mt-1 topping_price_${index}" style="font-size: 14px !important; opacity: .8;" toppingPrice="${toppingItem.toppingPrice}" toppingId="${toppingItem.id}">${toppingItem.toppingName} (${toppingItem.toppingPrice.toLocaleString('en-US')} VNĐ)</p>`;
                        toppingPrice += toppingItem.toppingPrice;
                    });
                    html += `<div class="card border shadow-none" id="cart_product_${index}">
                    <div class="card-body">
                        <input type="number" class="productDetailId" value="${item.productDetailsId}"  hidden />
                        <div class="d-flex align-items-start border-bottom pb-3">
                            <div class="me-4">
                                <img src="${item.imageUrl}" alt="" class="avatar-lg rounded">
                            </div>
                            <div class="flex-grow-1 overflow-hidden">
                                <div>
                                    <h5 class="text-truncate font-size-18"><a href="#" class="text-dark">${item.productName}</a></h5>
                                    <p class="text-muted mb-0">
                                        <i class="text-warning bi bi-star-fill"></i>
                                        <i class="text-warning bi bi-star-fill"></i>
                                        <i class="text-warning bi bi-star-fill"></i>
                                        <i class="text-warning bi bi-star-fill"></i>
                                        <i class="text-warning bi bi-star-fill"></i>

                                    </p>
                                    <p class="mb-0 mt-1">Size: <span class="fw-medium">${item.sizeName}</span></p>
                                </div>
                            </div>

                            <div class="flex-grow-1 overflow-hidden">
                                ${item.lstToppings.length > 0 ? `<h5 class="text - truncate font - size - 18">Topping</h5>` : ``}
                                ${htmlTopping}
                            </div>


                            <div class="flex-shrink-0 ms-2">
                                <ul class="list-inline mb-0 font-size-16">
                                    <li class="list-inline-item">
                                        <a href="#" class="text-muted px-1" onclick="DeleteProductCart(${item.cartId}, ${index})">
                                            <i class="bi bi-trash"></i>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>

                        <div>
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="mt-3">
                                        <p class="text-muted mb-2">Giá</p>
                                        <h5 class="mb-0 mt-2" >${item.price.toLocaleString('en-US')} VNĐ</h5>
                                        <input type="number" value="${item.price}" id="price_${index}" hidden />
                                    </div>
                                </div>
                                <div class="col-md-5">
                                    <div class="mt-3">
                                        <p class="text-muted mb-2">Số lượng</p>
                                        <div class="d-inline-flex">
                                            <button class="btn btn-sm btn-success" onclick="decreaseValue(event,${index})"><i class="bi bi-dash-lg"></i></button>
                                            <input type="number" class="form-control form-control-sm me-2 ms-2 p-0 text-center quantity product-number" style="width: 56px !important" readonly value="${item.quantity}" " />
                                            <button class="btn btn-sm btn-success" onclick="increaseValue(event,${index})"><i class="bi bi-plus-lg"></i></button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="mt-3">
                                        <p class="text-muted mb-2">Tổng tiền</p>
                                        <h5 id="totalMoneyText_${index}">${(item.price * item.quantity + toppingPrice).toLocaleString('en-US')} VNĐ</h5>
                                        <input class="totalPrice" type="number" value="${item.price * item.quantity + toppingPrice}" id="totalPrice_${index}" hidden />

                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>`;
                })
            } else {
                alert(data.massage);
                return;
            }

            $("#cart_item").html(html);
            TotalMoney();
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}

function chageQuantity(el, index) {
    let totalPriceTopping = 0;
    $.each($(`.topping_price_${index}`), function (index, item) {
        totalPriceTopping += parseInt($(item).attr("toppingPrice"));
    });
    let quantity = $(el).val();
    let price = $(`#price_${index}`).val();
    $(`#totalMoneyText_${index}`).html(`${(price * quantity + totalPriceTopping).toLocaleString('en-US')} VNĐ`);
    $(`#totalPrice_${index}`).val(price * quantity + totalPriceTopping);
    TotalMoney();

}
function TotalMoney() {
    let totalMoney = 0;
    $(".totalPrice").each((index, el) => {
        totalMoney += parseFloat($(el).val()) || 0;
    });
    $("#sub_total").html(`${totalMoney.toLocaleString('en-US')} VNĐ`)
    $("#total_money").html(`${(totalMoney + 25000).toLocaleString('en-US')} VNĐ`)
}

function DeleteProductCart(Id, index) {
    if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này ra khỏi danh sách không?")) {
        $(`#cart_product_${index}`).remove();
        $.ajax({
            url: "/Cart/RemoveToCart",
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            data: {
                cartId: Id
            },
            success: function (data) {
                GetAll();
                TotalMoney();

            },

            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}
function reset() {
    $("#customer_name").val("");
    $("#customer_address").val("");
    $("#customer_phone").val("");
}
function CreateOrder() {

    let cusName = $("#customer_name").val();
    let cusAddress = $("#customer_address").val();
    let cusPhone = $("#customer_phone").val();
    let accountId = $("#accountId").val();
    if (cusName.length <= 0) {
        alert("Hãy nhập tên người mua!");
        return;
    }
    if (cusPhone.length <= 0) {
        alert("Hãy nhập số điện thoại!");
        return;
    }
    if (cusAddress.length <= 0) {
        alert("Hãy nhập địa chỉ!");
        return;
    }
    if (accountId <= 0) {
        alert("Hãy đăng nhập để sử dụng tính năng!");
        return;
    }

    let elQuantity = $(".quantity");
    let elProductDetailId = $(".productDetailId");
    let elTotalMoney = $(".totalPrice");
    let arrDetail = [];
    for (let i = 0; i < elQuantity.length; i++) {
        let arrTopping = [];
        $.each($(`.topping_price_${i}`), function (index, item) {
            let objTopping = {
                ToppingId: parseInt($(item).attr("toppingId")),
                ToppingPrice: parseInt($(item).attr("toppingPrice")),
            }
            arrTopping.push(objTopping);
        });



        let objDetail = {
            Quantity: parseInt($(elQuantity[i]).val()),
            TotalMoney: parseFloat($(elTotalMoney[i]).val()),
            ProductDetailId: parseInt($(elProductDetailId[i]).val()),
            LstTopping: arrTopping
        }
        arrDetail.push(objDetail);
    }
    let obj = {
        Details: arrDetail,
        CustomerName: cusName,
        PhoneNumber: cusPhone,
        Address: cusAddress,
        AccountId: accountId
    }
    if (confirm("Bạn có muốn xác nhận đơn hàng này không?")) {
        $.ajax({
            type: 'POST',
            url: "/Order/CreateOrder",
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(obj),
            success: function (result) {
                if (result.status == 1) {
                    reset();
                }
                alert(result.message)
                GetAll();
                TotalMoney();
            },
            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}

function decreaseValue(event, index) {
    var el = $(event.target);
    let elInput = $(el).closest("div").find(".product-number");
    let valueNumber = parseInt($(elInput[0]).val());
    if (valueNumber > 1) {
        $(elInput[0]).val(valueNumber - 1)
        chageQuantity(elInput[0], index);
    }
}

function increaseValue(event, index) {
    var el = $(event.target);
    let elInput = $(el).closest("div").find(".product-number");
    let valueNumber = parseInt($(elInput[0]).val());
    $(elInput[0]).val(valueNumber + 1)
    chageQuantity(elInput[0], index);
}