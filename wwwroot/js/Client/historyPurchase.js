
$(document).ready(function (e) {
    GetAll();
});

function GetAll() {
    ShowSpinnerClient();
    var request = $('#request').val() ?? "";
    var dateStart = $('#date_start').val();
    var dateEnd = $('#date_end').val();
    let obj = {
        request: request,
        dateStart: dateStart,
        dateEnd: dateEnd,
        pageNumber: 1,
        status: 1
    };

    $.ajax({
        url: "/Order/GetAll",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(obj),
        success: function (data) {
            //var html = '';
            let htmlCard = '';
            $.each(data.data, function (index, item) {
                let btnAprroved = `<button type="button" class = "btn btn-sm btn-danger fw-bold" onclick="StatusApproved(${item.id},3)">Hủy đơn hàng</button>`;

                //html += `<tr class="align-middle">
                //            <td scope="col" class="text-center"><a style="color: blue;  cursor: pointer; text-decoration: none;"  href="/Order/OrderDetails?OrderId=${item.id}">${item.orderCode}</a></td>
                //            <td scope="col" class="text-center">${item.dateFormat}</td>
                //            <td scope="col" class="">${item.statusText}</td>
                //            <td scope="col" class="text-center">${item.totalMoney.toLocaleString('en-US')} VNĐ</td>
                //            <td scope="col">${item.customerName}</td>
                //            <td scope="col">${item.phoneNumber}</td>
                //            <td scope="col">${item.address}</td>
                //            <td scope="col" class="text-center">
                //                ${item.status > 0 ? "" : btnAprroved}
                //            </td>
                //        </tr>`;

                let htmlReasonCancel = `<li class="text-danger">
                                            <i class="bi bi-x-circle"></i>  <span class="fw-bold">Lý do hủy:</span> ${item.reasonCancel}
                                        </li>`
                let styleButton = (item.status == 0 || item.status == 1) ? "warning" : ((item.status == 2) ? "success" : "danger")
                htmlCard += `<div class="card p-0 mb-3">
                                <div class="card-body p-1">
                                    <div class="container">
                                        <div class="container">

                                            <div class="row">
                                                <div class="col-xl-7">
                                                    <ul class="list-unstyled">
                                                        <li class="fw-bold"><i class="bi bi-receipt"></i>  Mã hóa đơn: <span><a class="fw-bold" style="color: blue;  cursor: pointer; text-decoration: none;"  href="/Order/OrderDetails?OrderId=${item.id}">${item.orderCode}</a></span></li>
                                                         <li class="">
                                                            <i class="bi bi-calendar"></i>  <span class="fw-bold">Thời gian đặt:</span> ${moment(item.createDate).format('DD/MM/YYYY HH:mm:ss')}
                                                        </li>
                                                        <li class="fw-bold"><i class="bi bi-bell"></i>  Trạng thái: <span class="btn btn-sm btn-${styleButton} fw-bold" onclick="ShowReasonCancel(${item.status},'${item.reasonCancel}')">${item.statusText}</span></li>
                                                    </ul>
                                                </div>
                                                <div class="col-xl-5">
                                                    <ul class="list-unstyled">
                                                       

                                                        <li class="">
                                                           <i class="bi bi-geo-alt"></i>  <span class="fw-bold">Địa chỉ:</span> ${item.address}
                                                        </li>

                                                        <li class="">
                                                           <i class="bi bi-telephone"></i>  <span class="fw-bold">Điện thoại:</span> ${item.phoneNumber}
                                                        </li>
                                                        
                                                    </ul>
                                                </div>
                                            </div>
                                            <div class="row justify-content-center">
                                                <div class="col-md-2 mb-4 mb-md-0">
                                                    <div class="bg-image ripple rounded-5 mb-4 overflow-hidden d-block" data-ripple-color="light">
                                                        <img src="${item.imageUrl}" class="w-100" height="100px" alt="Elegant shoes and shirt" />
                                                        <a href="#!">
                                                            <div class="hover-overlay">
                                                                <div class="mask" style="background-color: hsla(0, 0%, 98.4%, 0.2)"></div>
                                                            </div>
                                                        </a>
                                                    </div>
                                                </div>
                                                <div class="col-md-5 mb-4 mb-md-0">
                                                    <p class="fw-bold mb-0">${item.productName}</p>
                                                    <p class="fw-bold mb-0">
                                                        <span class=" me-2">Size:</span><span>${item.sizeName}</span>
                                                    </p>
                                                    <p class="fw-bold mb-0">
                                                        <span class="text-danger">${item.unitPrice.toLocaleString('en-US')} VNĐ</span>
                                                    </p>
                                                </div>
                                                <div class="col-md-5 mb-4 mb-md-0">
                                                    <h5 class="mb-2">
                                                       <span class="align-middle">Tổng tiền:</span>
                                                    </h5>
                                                    <h5 class="mb-2">
                                                       <span class="align-middle text-danger fw-bold">${item.totalMoney.toLocaleString('en-US')} VNĐ</span>
                                                    </h5>
                                                    
                                                    <div class="d-flex justify-content-end">
                                                        ${item.status > 0 ? "" : btnAprroved}
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                                `



            })
            //$('#tbody').html(html);
            $('#content').html(htmlCard);
        },

        error: function (err) {
            alert(err.responseText);
        }
    });
}
$('#request').keydown(function (e) {
    if (e.keyCode == 13) {
        pageNumber = 1;
        GetAll();
    }
})

function StatusApproved(orderId, status) {
    if (confirm(`Ban có chắc muốn xác nhận hủy đơn hàng này không?`)) {
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
                alert(err.responseText);
            }
        });
    }
}

function ShowReasonCancel(status, reasonCancel) {
    if (status != 3) return;



    $("#reason_cancel_order").val(reasonCancel);
    $("#modal_reason_cancel").modal('show');
}