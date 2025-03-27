$(document).ready(function (e) {
    GetAll();
    $(".select2").select2();
});

var pageNumber = 1;
var totalPage = 0;
var accountId = 0;
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
    if (accountId == 0) {
        $('#btn_deleteModal').hide();
    }
}

function CloseModal() {
    accountId = 0;
    document.getElementById('form').reset();
    $('#importedMaterial').val("");
    $('#staticBackdrop').modal('hide');
}

$('.add_new').click(function () {
    $('#staticBackdropLabel').text("Thêm mới loại sản phẩm");
    showModal();
})
$('#btn_deleteModal').click(function () {
    DeleteById(accountId);
});
function GetAll() {
    ShowSpinnerClient();
    let _url = "/Admin/Account/GetAll?";
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
            $.each(data.customer.data, function (index, item) {
                html += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                            </td>
                            <td scope="col" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetById(${item.id})">${item.email} </a></td>
                            <td scope="col">${item.fullName}</td>
                            <td scope="col">${item.genderText}</td>
                            <td scope="col">${item.roleText}</td>
                            <td scope="col">${item.phoneNumber}</td>
                            <td scope="col">${item.address}</td>
                            <td scope="col">${item.totalOrderSuccess}</td>
                            <td scope="col">${item.totalOrderCancel}</td>
                        </tr>`;
            })
            let total = Math.ceil(data.customer.totalCount[0].totalCount / 10);
            totalPage = total > 0 ? total : 1;
            $('#tbodyCus').html(html);
            $('#page_details').text(`Trang ${pageNumber} / ${totalPage}`);
            $('#pageNumber').val(pageNumber);
            Pagination();

            let htmlEmp = '';
            $.each(data.employee.data, function (index, item) {
                htmlEmp += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                            </td>
                            <td scope="col" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetById(${item.id})">${item.email} </a></td>
                            <td scope="col">${item.fullName}</td>
                            <td scope="col">${item.genderText}</td>
                            <td scope="col">${item.roleText}</td>
                            <td scope="col">${item.phoneNumber}</td>
                            <td scope="col">${item.address}</td>
                            
                        </tr>`;
            })
            $('#tbodyEmp').html(htmlEmp);

            let htmlManage = '';

            $.each(data.manager.data, function (index, item) {
                htmlManage += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                            </td>
                            <td scope="col" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetById(${item.id})">${item.email} </a></td>
                            <td scope="col">${item.fullName}</td>
                            <td scope="col">${item.genderText}</td>
                            <td scope="col">${item.roleText}</td>
                            <td scope="col">${item.phoneNumber}</td>
                            <td scope="col">${item.address}</td>
                            
                        </tr>`;
            })
            $('#tbodyManager').html(htmlManage);
        },

        error: function (err) {
            alert(err.responseText);
        }
    });
}


function GetById(id) {
    $('#btn_deleteModal').show();
    $('#staticBackdropLabel').text("Cập nhật Size");
    accountId = id;
    let _url = "/Admin/Account/GetById";
    $.ajax({
        url: _url,
        type: 'GET',
        dataType: 'json',
        data: {
            Id: id
        },
        contentType: 'application/json',
        success: function (data) {
            $("#formName").val(data.fullName);
            $("#formGender").val(data.gender);
            $("#formRole").val(data.role);
            $("#formIsActive").val(data.isActive);
            $("#formPhoneNumber").val(data.phoneNumber);
            $("#formEmail").val(data.email);
            $("#formAddress").val(data.address);

        },
        error: function (err) {
            alert(err.responseText);
        }
    });
    showModal();
}
function CreateOrUpdate() {

    let fn = $("#formName").val();
    let em = $("#formEmail").val();

    if (fn == null || fn.length <= 0) {
        alert("Hãy nhập họ tên!");
        return
    }
    if (em == null || em.length <= 0) {
        alert("Hãy nhập email!");
        return
    }


    ShowSpinnerClient();
    var obj = {
        Id: accountId,
        FullName: $("#formName").val(),
        Role: $("#formRole").val(),
        Gender: $("#formGender").val(),
        IsActive: parseInt($("#formIsActive").val()),
        PhoneNumber: $("#formPhoneNumber").val(),
        Email: $("#formEmail").val(),
        Address: $("#formAddress").val()
    };

    let _url = "/Admin/Account/CreateOrUpdate";
    $.ajax({
        type: 'POST',
        url: _url,
        contentType: 'application/json;charset=utf-8',
        data: JSON.stringify(obj),
        success: function (result) {
            if (result.status == 0) {
                alert(result.message)
            } else {
                CloseModal();
                GetAll();
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    });

}
