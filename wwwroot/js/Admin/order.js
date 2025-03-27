$(document).ready(function (e) {
    $(".select2").select2();
    GetAll();
    setInterval(function () {
        GetAll();
    }, 10000);
});
var pageNumber = 1;
var totalPage = 0;
var productId = 0;
var htmlSize = "";
var _attachFiles = [];
$('#request').keydown(function (e) {
    if (e.keyCode == 13) {
        pageNumber = 1;
        GetAll();
    }
})
function previousPage() {
    if (pageNumber > 1) {
        pageNumber -= 1;
        GetAll();
    }
}
function nextPage() {
    if (pageNumber < totalPage) {
        pageNumber += 1;
        GetAll();
    }
}
$('#pageSize').change(function () {
    pageNumber = 1;
    GetAll();
});

$('#groupId').change(function () {
    pageNumber = 1;
    GetAll();
});
function Pagination() {
    var html = `<li class="page-item"> <a onclick="previousPage()" class="page-link cusor" aria - label="Previous" > <span aria-hidden="true">&laquo;</span> </a > 
                </li > `;
    if (totalPage == 1) {
        html += `<li class="page-item"><a class="page-link" style="background-color: aliceblue;">${pageNumber}</a></li>`
    }
    else if (pageNumber == 1) {
        html += `<li class="page-item"><a class="page-link" style="background-color: aliceblue;">${pageNumber}</a></li>
            <li class="page-item cusor"><a onclick="nextPage()" class="page-link">${pageNumber + 1}</a></li>`;
    }
    else if (pageNumber == totalPage) {
        html += `<li class="page-item cusor"><a onclick="previousPage()"  class="page-link">${pageNumber - 1}</a></li>
            <li class="page-item"><a style="background-color: aliceblue;" class="page-link">${pageNumber}</a></li>`;
    }
    else {
        html += `<li class="page-item cusor"><a onclick="previousPage()" class="page-link">${pageNumber - 1}</a></li>
            <li class="page-item"><a class="page-link" style="background-color: aliceblue;" >${pageNumber}</a></li>
            <li class="page-item cusor"><a onclick="nextPage()" class="page-link">${pageNumber + 1}</a></li>`;
    }
    html += `<li class="page-item cusor">
            <a onclick="nextPage()" class="page-link" aria-label="Next">
                <span aria-hidden="true">&raquo;</span>
            </a>
         </li>`;

    $('#pagination').html(html);
}
$('#btn_search').click(function () {
    pageNumber = 1;
    GetAll();
})
function GetAll() {
    var request = $('#request').val() ?? "";
    var status = $('#order_status option:selected').val();
    var dateStart = $('#date_start').val();
    var dateEnd =   $('#date_end').val();
    let obj = {
        request,
        pageNumber,
        status,
        dateStart,
        dateEnd
    };
    $.ajax({
        url: "/Admin/Order/GetAll",
        data: JSON.stringify(obj),
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var html = '';
            $.each(data.data, function (index, item) {
                html += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <btn class="btn btn-sm btn-danger" title="Hủy đơn hàng" ${item.status >= 2 ? "hidden" : ""} onclick="CancelOrder(${item.id})"><i class="bi bi-x-circle-fill"></i></btn>
                                <btn class="btn btn-sm btn-success" title="Giao hàng" ${item.status >= 1 ? "hidden" : ""} onclick="StatusApproved(${item.id}, 1, 'Giao đơn hàng')"><i class="bi bi-truck"></i></btn>
                                <btn class="btn btn-sm btn-primary" title="Thành công" ${item.status >= 2 ? "hidden" : ""}  onclick="StatusApproved(${item.id}, 2, 'Hoàn thành đơn hàng')"><i class="bi bi-check-lg"></i></btn>
                            </td>
                            <td scope="col" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetDetails(${item.id}, event)" created-date="${moment(item.createDate).format('DD/MM/YYYY HH:mm:ss')}">${item.orderCode}</a></td>
                            <td scope="col" class="text-end"> ${item.totalMoney.toLocaleString('en-US') } VNĐ</td>
                            <td scope="col" class="text-center">${moment(item.createDate).format('DD/MM/YYYY HH:mm:ss')}</td>
                            <td class="text-center" scope="col">${item.status === 0 ? "Chờ xác nhận" : (item.status === 1 ? "Đang giao" : (item.status === 2 ? "Hoàn thành" : "Đã hủy"))}</td>
                            <td scope="col">${item.customerName ?? ""}</td>
                            <td scope="col">${item.phoneNumber ?? ""}</td>
                            <td scope="col">${item.address ?? ""}</td>
                        </tr>`;
            })

            let total = Math.ceil(data.totalCount.totalCount / 10);
            totalPage = total > 0 ? total : 1;
            $('#tbody').html(html);
            $('#page_details').text(`Trang ${pageNumber} / ${totalPage}`);
            $('#pageNumber').val(pageNumber);
            Pagination();
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}
function GetDetails(id, event) {
    let el = $(event.target);


    $.ajax({
        url: '/Admin/Order/GetDetail',
        type: 'GET',
        dataType: 'json',
        data: {
            OrderId: id
        },
        contentType: 'application/json',
        success: function (result) {
            let htmlReasonCancel = `<div class="col-sm-12 align-middle p-1"><h6><span class="fw-bold">Lý do hủy:</span> ${result.data.reasonCancel}</h6></div>`;
            let htmlBody = `<div class="row ps-5 pe-5">
                                <div class="col-sm-6 align-middle p-1"><h6><span class="fw-bold">Người đặt:</span> ${result.data.customerName}</h6></div>
                                <div class="col-sm-6 align-middle p-1"><h6><span class="fw-bold">Số điện thoại:</span> ${result.data.phoneNumber}</h6></div>
                                <div class="col-sm-12 align-middle p-1"><h6><span class="fw-bold">Địa chỉ:</span> ${result.data.address}</h6></div>
                                ${result.data.status == 3 ? htmlReasonCancel : ''}
                            </div>`
            let totalMoney = 0;
            //Hiển thị danh sách chi tiết
            $.each(result.lst, function (index, item) {
                var styleText = item.sizeName;
                let htmlTopping = '`<h5 class="text-truncate font-size-18">Topping</h5>`';
                let toppingPrice = 0;
                $.each(item.lstTopping, function (toppingIndex, toppingItem) {
                    htmlTopping += `<p class="mb-0 mt-1 topping_price_${index}" style="font-size: 14px !important; opacity: .8;" toppingPrice="${toppingItem.toppingPrice}" toppingId="${toppingItem.id}">${toppingItem.toppingName} (${toppingItem.toppingPrice.toLocaleString('en-US')} VNĐ)</p>`;
                    toppingPrice += toppingItem.toppingPrice;
                });
                totalMoney += (item.price * item.quantity) + toppingPrice;

                htmlBody += `<div class="card border shadow-none mb-0" id="cart_product_${index}" style="scale: .9;">
             <div class="card-body">
                 <div class="d-flex align-items-start border-bottom pb-3 pt-1">
                     <div class="me-4">
                         <img src="${item.imageUrl}" alt="" class="avatar-lg rounded" style="width: 150px !important;">
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
                         ${item.lstTopping.length > 0 ? htmlTopping : ``}
                        
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
                                 <input type="number" class="form-control form-control-sm me-2 ms-2 p-0 text-center quantity product-number" style="width: 56px !important" readonly value="${item.quantity}" " />
                             </div>
                         </div>
                         <div class="col-md-3">
                             <div class="mt-3">
                                 <p class="text-muted mb-2">Tổng tiền</p>
                                 <h5 id="totalMoneyText_${index}">${(item.price * item.quantity + toppingPrice).toLocaleString('en-US')} VNĐ</h5>
                             </div>
                         </div>
                     </div>
                 </div>

             </div>
         </div>`;
            });

            htmlBody += `<div class="row">
                            <div class="col-sm-2 text-center fw-bold align-middle p-1"> Tổng tiền </div>
                            <div class="col-sm-10 text-center"> <input type="text" class="form-control text-center" id="totalMoney" value="${totalMoney.toLocaleString('en-US')} VNĐ" disabled readonly> </div>
                         </div>`

            
            $("#modal-content").html(htmlBody);
            let modalHeader = `${$(el).html()} - ${$(el).attr('created-date')}`
            $("#modal-mahoadon").html(modalHeader);
            $("#exampleModal").modal('show');
        },

        error: function (err) {
            MessageError(err.responseText);
        }
    });
}
function StatusApproved(orderId, status, text) {
    if (confirm(`Ban có chắc muốn xác nhận ${text} không?`)) {
        $.ajax({
            type: 'GET',
            url: "/Admin/Order/ChangeStatusOrder",
            contentType: 'application/json;charset=utf-8',
            data: { orderId, status },
            success: function (result) {
                if (result.status != 1) {
                    alert(result.message)
                }
                GetAll();
            },
            error: function (err) {
                MessageError(err.responseText);
            }
        });
    }
}

function CancelOrder(orderid) {
    $("#reason_cancel_order_id").val(orderid)
    $("#modal_reason_cancel").modal('show');
}
function CallAPICancelOrder() {

    let orderId = parseInt($("#reason_cancel_order_id").val());
    let status = 3;
    let reasonCancel = $("#reason_cancel_order").val();
    $.ajax({
        type: 'GET',
        url: "/Admin/Order/ChangeStatusOrder",
        contentType: 'application/json;charset=utf-8',
        data: { orderId, status, reasonCancel },
        success: function (result) {
            if (result.status != 1) {
                alert(result.message)
            }
            $("#modal_reason_cancel").modal('hide');
            GetAll();
        },
        error: function (err) {
            MessageError(err.responseText);
        }
    });
}

function DowLoadExcel() {
    const apiURL = "/Admin/Order/ExportExcel";
    const params = {
        dateStart:$('#date_start').val(),
        dateEnd: $('#date_end').val()
    };
    const queryString = new URLSearchParams(params).toString();
    const fullURL = `${apiURL}?${queryString}`;

    fetch(fullURL, {
        method: 'GET',
    })
        .then(response => {
            if (!response.ok) {
                alert('Lỗi khi tải file');
            }
            return response.blob();
        })
        .then(blob => {
            // Tạo link tạm thời để tải file
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = "Order.xlsx";
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Có lỗi xảy ra:', error);
        });
}