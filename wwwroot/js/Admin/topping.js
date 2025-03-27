$(document).ready(function (e) {
    GetAll();
});

var pageNumber = 1;
var totalPage = 0;
var modelID = 0;
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



function showModal() {
    $('#staticBackdrop').modal('show');
    if (modelID == 0) {
        $('#btn_deleteModal').hide();
    }
}

function CloseModal() {
    modelID = 0;
    document.getElementById('form').reset();
    $('#importedMaterial').val("");
    $('#staticBackdrop').modal('hide');
}

$('#add_new').click(function () {
    $('#staticBackdropLabel').text("Thêm mới topping");
    showModal();
})
$('#btn_deleteModal').click(function () {
    DeleteById(modelID);
});
function GetAll() {
    ShowSpinnerClient();

    let _url = "/Admin/Topping/GetAll?";
    var request = $('#request').val();
    var groupId = $('#groupId option:selected').val();
    if (request.length > 0) {
        _url += "request=" + request;
    }
    _url += "&pageNumber=" + pageNumber;

    $.ajax({
        url: _url,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var html = '';
            $.each(data.data, function (index, item) {
                html += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.Id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="DeleteById(${item.Id})"><i class="bi bi-trash3"></i> Xóa</button>
                            </td>
                            <td scope="col" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetById(${item.Id})">${item.ToppingCode} </a></td>
                            <td scope="col">${item.ToppingName}</td>
                            <td scope="col">${formatter(item.ToppingPrice)} VNĐ</td>
                            
                        </tr>`;
            })
            let total = Math.ceil(data.totalCount[0].TotalCount / 10);

            totalPage = total > 0 ? total : 1;
            $('#tbody').html(html);
            $('#page_details').text(`Trang ${pageNumber} / ${totalPage}`);
            $('#pageNumber').val(pageNumber);
            Pagination();
        },

        error: function (err) {
            alert(err.responseText);
        }
    });
}


function GetById(id) {
    $('#btn_deleteModal').show();
    $('#staticBackdropLabel').text("Cập nhật topping");
    modelID = id;
    let _url = "/Admin/Topping/GetById";
    $.ajax({
        url: _url,
        type: 'GET',
        dataType: 'json',
        data: {
            Id: id
        },
        contentType: 'application/json',
        success: function (data) {
            $('#formCode').val(data.toppingCode);
            $('#formName').val(data.toppingName);
            $("#formPrice").val(formatter(data.toppingPrice));

        },
        error: function (err) {
            MessageError(err.responseText);
        }
    });
    showModal();
}
function CreateOrUpdate() {
    let isValid = true;
    var obj = {
        Id: parseInt(modelID),
        ToppingCode: $("#formCode").val(),
        ToppingName: $("#formName").val(),
        ToppingPrice: parseFloat($("#formPrice").val().replaceAll(",", ""))
    };

    if (obj.ToppingCode == "") {
        alert("Vui lòng nhập Mã Topping!");
        isValid = false;
    }
    else if (obj.ToppingName == "") {
        alert("Vui lòng nhập Tên Topping!");
        isValid = false;
    }
    else if (obj.ToppingPrice <= 0) {
        alert("Vui lòng nhập Giá Topping!");
        isValid = false;
    }


    if (isValid) {
        let _url = "/Admin/Topping/CreateOrUpdate";
        $.ajax({
            type: 'POST',
            url: _url,
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(obj),
            success: function (result) {
                if (result.status == 0) {
                    alert(result.statusText)
                }
                else {
                    GetAll();
                    CloseModal();
                }
            },
            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}
function DeleteById(id) {
    if (confirm("Bạn có chắc chắn muốn thực hiện thao tác này?") == true) {
        let _url = "/Admin/Topping/Delete";
        $.ajax({
            type: 'GET',
            url: _url,
            contentType: 'application/json;charset=utf-8',
            data: {
                Id: id,
            },
            success: function (result) {
                if (result.status == 1) {
                    CloseModal();
                    pageNumber = 1;
                    GetAll();
                }
                else alert(result.message);

            },
            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}

function formatMoney(event) {
    let value = $(event.target).val();
    value = value.replace(/[^0-9]/g, '');
    let lastValue = value.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    $(event.target).val(lastValue);
}

const formatter = (number) => {
    return new Intl.NumberFormat('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0, }).format(number);
};