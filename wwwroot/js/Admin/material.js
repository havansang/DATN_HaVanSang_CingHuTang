$(document).ready(function (e) {

    GetAll();
    $(".select2").select2();
    $("#formUnit").select2({
        dropdownParent: $("#staticBackdrop")
    });
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
    $("#formUnit").val(0);
    modelID = 0;
    document.getElementById('form').reset();
    $('#staticBackdrop').modal('hide');
}

$('#add_new').click(function () {
    $('#staticBackdropLabel').text("Thêm mới Nguyên liệu");
    showModal();
})  
$('#btn_deleteModal').click(function () {
    DeleteById(modelID);
});

function GetAll() {
    ShowSpinnerClient();
    let _url = "/Admin/Material/GetAll?";
    var request = $('#request').val();
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
                let style = item.MinQuantity >= item.ToltalQuantity ? "bg-warning" : ""
                html += `<tr class="  align-middle ">
                            <td scope="col" class="${style} align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.Id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="DeleteById(${item.Id})"><i class="bi bi-trash3"></i> Xóa</button>
                            </td>
                            <td class="${style} text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetById(${item.Id})">${item.MaterialCode} </a></td>
                            <td class="${style} ">${item.MaterialName}</td>
                            <td class="${style} text-end">${item.ToltalQuantity}</td>
                            <td class="${style} text-center">${item.UnitName}</td>
                            <td class="${style} text-end">${item.MinQuantity}</td>
                            <td class="${style} text-end">${new Intl.NumberFormat('vi-VN').format(item.UnitPrice)} VNĐ</td>
                            <td class="${style} " style="white-space: pre;">${item.SupplierName}</td>
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
    $('#staticBackdropLabel').text("Cập nhật Nguyên liệu");
    modelID = id;
    let _url = "/Admin/Material/GetByID";
    $.ajax({
        url: _url,
        type: 'GET',
        dataType: 'json',
        data: {
            Id: id
        },
        contentType: 'application/json',
        success: function (data) {
            $('#formCode').val(data.materialCode);
            $('#formName').val(data.materialName);
            $("#formMinQuantity").val(data.minQuantity);
            $("#formDecription").val(data.decription);
            $("#formUnit").val(data.unitId).trigger("change");
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
        UnitId: parseInt($("#formUnit").val()),
        MaterialCode: $("#formCode").val(),
        MaterialName: $("#formName").val(),
        MinQuantity: parseInt($("#formMinQuantity").val()),
        Decription: $("#formDecription").val()
    };

    if (obj.MaterialCode == "") {
        alert("Vui lòng nhập Mã nguyên liệu!");
        isValid = false;
    }
    else if (obj.MaterialName == "") {
        alert("Vui lòng nhập Tên nguyên liệu!");
        isValid = false;
    }
    else if (obj.UnitId <= 0) {
        alert("Vui lòng chọn Đơn vị!");
        isValid = false;
    }
    else if (obj.MinQuantity == "") {
        alert("Vui lòng nhập Số lượng tối thiểu!");
        isValid = false;
    }

    if (isValid) {
        let _url = "/Admin/Material/CreateOrUpdate";
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
                    CloseModal();
                    GetAll();
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
        let _url = "/Admin/Supplier/Delete";
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

function DowLoadExcel() {
    const apiURL = "/Admin/Material/ExportExcel";
    //const params = {
    //    userId: 123,     
    //    fileType: "pdf"  
    //};
    //const queryString = new URLSearchParams(params).toString();
    //const fullURL = `${apiURL}?${queryString}`;

    fetch(apiURL, {
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
            a.download = "Material.xlsx"; 
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url); 
        })
        .catch(error => {
            console.error('Có lỗi xảy ra:', error);
        });
}