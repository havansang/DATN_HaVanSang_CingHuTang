$(document).ready(function (e) {
    GetAll();
    GetAllMaterial();
    GetAllUnit();
    $(".select2").select2();
    $("#formSupplier").select2({
        dropdownParent: $("#staticBackdrop")
    });
});

var pageNumber = 1;
var totalPage = 0;
var modelID = 0;
var htmlMaterial = "";
var htmlUnit = "";
var _attachFiles = [];
var lstFileDeleted = [];
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
    $("#btninformation").click();
    $('#staticBackdrop').modal('show');
    if (modelID == 0) {
        $('#btn_deleteModal').hide();
    }
}

function CloseModal() {
    $("#formUnit").val(0);
    modelID = 0;
    document.getElementById('form').reset();
    $("#formSupplier").val(0).trigger("change");
    $('#staticBackdrop').modal('hide');
    $('#tbodySteps').html('');
    $('#AttachFiles').html('');
    lstFileDeleted = [];

}

$('#add_new').click(function () {
    $('#staticBackdropLabel').text("Thêm Phiếu nhập");
    LoadGoodsIssueCode();
    showModal();
})
$('#btn_deleteModal').click(function () {
    DeleteById(modelID);
});

function GetAll() {
    ShowSpinnerClient();

    let dataRequest = {
        Request: $('#request').val(),
        PageNumber: parseInt(pageNumber),
        AccountID: parseInt($('#accountID').val()),
        DateStart: $('#dateStart').val(),
        DateEnd: $('#dateEnd').val(),
    }
    $.ajax({
        url: "/Admin/GoodsIssue/GetAll",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(dataRequest),
        success: function (data) {
            var html = '';
            $.each(data.data, function (index, item) {
                html += `<tr class="align-middle">
                            <td scope="col" class="align-center text-center" style="white-space: nowrap">
                                <button class="btn btn-sm btn-primary" onclick="GetById(${item.Id})" ><i class="bi bi-pencil-square"></i> Sửa</button>
                                <button class="btn btn-sm btn-danger" onclick="DeleteById(${item.Id})"><i class="bi bi-trash3"></i> Xóa</button>
                            </td>
                            <td class="" class="text-center"> <a style="color: blue;  cursor: pointer;" onclick="GetDetails(${item.Id}, event)">${item.GoodIssueCode} </a></td>
                            <td class="">${item.FullName}</td>
                            <td class="text-center">${moment(item.IssueDate).format("DD/MM/YYYY")}</td>
                            <td class="text-center">${item.Decription}</td>
                            <td class="">${item.CreatedBy}</td>
                            <td class="">${moment(item.CreatedDate).format("DD/MM/YYYY")}</td>
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
function GetDetails(id, event) {
        ShowSpinnerClient();
    var htmlDetails = `<tr class="goodsIssue-details" id="goodsIssue_details_${id}" style="border-left: 2px solid red; border-right: 2px solid red; border-bottom: 2px solid red;">
                        <td colspan="8" class="p-1">
                            <div class="card-body p-0">
                                <ul class="nav nav-tabs" id="myTab2" role="tablist">
                                    <li class="nav-item flex-fill" role="presentation">
                                        <button class="nav-link w-100 active" id="btninformation1" data-bs-toggle="tab" data-bs-target="#bordered-justified-home1" type="button" role="tab" aria-controls="btninformation1" aria-selected="true">Chi tiết</button>
                                    </li>

                                    <li class="nav-item flex-fill" role="presentation">
                                        <button class="nav-link w-100" id="btnsteps1" data-bs-toggle="tab" data-bs-target="#bordered-justified-profile1" type="button" role="tab" aria-controls="btnsteps1" aria-selected="false">File đính kèm</button>
                                    </li>
                                </ul>
                                <div class="tab-content tab-bordered">

                                     <div class="tab-pane fade show active" id="bordered-justified-home1" role="tabpanel" aria-labelledby="btninformation1">
                                        <div class="table-responsive" style="height:200px !important;">
                                            <table class="table table-sm m-0 table-bordered border-primary">
                                                <thead>
                                                    <tr class="text-center align-middle">
                                                        <th class="table-color" style="width: 30%;">Nguyên vật liệu</th>
                                                        <th class="table-color" style="width: 10%;">Số lượng</th>
                                                        <th class="table-color" style="width: 15%;">Đơn vị</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    @tbody
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>

                                   <div class="tab-pane fade" id="bordered-justified-profile1" role="tabpanel" aria-labelledby="btnsteps1">
                                        <ul class="list-group list-group-flush">
                                            @files
                                        </ul>
                                    </div>

                                </div>
                            </div>
                        </td>
                    </tr>`;

    var isShowDetail = $(`#goodsIssue_details_${id}`).length;
    var el = $(event.target).parent();
    if (isShowDetail > 0) {
        $(`#goodsIssue_details_${id}`).remove();
    } else {
        $('.goodsIssue-details').remove();
        $.ajax({
            url: '/Admin/GoodsIssue/GetById',
            type: 'GET',
            dataType: 'json',
            data: {
                Id: id
            },
            contentType: 'application/json',
            success: function (result) {
                var htmlBody = '';
                let htmlFile = '';
                let totalMoney = 0;
                //Hiển thị danh sách chi tiết
                $.each(result.details, function (index, item) {
                    totalMoney += (item.price * item.quantity)
                    htmlBody += `<tr class="sortable goodsReceipt_details_item">
                                    <td scope="col">${item.MaterialName}</td>
                                    <td scope="col" class="text-center">${item.Quantity}</td>
                                    <td scope="col" class="text-center">${item.UnitName}</td>
                                </tr>`;
                });

                $.each(result.files, function (key, item) {
                    htmlFile += `<li class="list-group-item p-0">
                                    <p class="m-0 font-weight-bold">${item.FileName}</p>
                                    <div>
                                        <a href="${item.FileUrl}">Tải về</a>
                                    </div>
                                </li>`;
                });


                htmlDetails = htmlDetails.replace('@tbody', htmlBody);
                htmlDetails = htmlDetails.replace('@files', htmlFile);

                $(htmlDetails).insertAfter($(el).parent());

            },

            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}

function GetById(id) {
    $('#btn_deleteModal').show();
    $('#staticBackdropLabel').text("Cập nhật Phiếu nhập");
    modelID = id;
    let _url = "/Admin/GoodsIssue/GetById";
    $.ajax({
        url: _url,
        type: 'GET',
        dataType: 'json',
        data: {
            Id: id
        },
        contentType: 'application/json',
        success: function (data) {
            $('#formCode').val(data.data[0].GoodIssueCode);
            $('#formIssueDate').val(moment(data.data[0].IssueDate).format("YYYY-MM-DD"));
            $('#formDecription').val(data.data[0].Description);
            let html = "";
            $.each(data.details, function (index, item) {
                html += `<tr class="sortable goodsReceipt_details_item">
                            <th scope="col">
                                   <select class="form-select material-Id select2-material" onchange="onSelectedMaterial(event)">
                                               ${htmlMaterial}    
                                   </select>
                            </th>
                            <th scope="col">
                                    <input type="number" class="form-control form-control-sm material-quantity" min="0" value="0">
                            </th>
                             <th scope="col">
                                    <select class="form-select select2-material material-unit" readonly disabled>
                                               ${htmlUnit}    
                                   </select>
                            </th>

                            <th scope="col" style="text-align:center;vertical-align: middle;"><button  onclick="deleteRow(event)" class="btn btn-danger"><i class="bi bi-trash"></i></th>
                        </tr>`;
            });
            $('#tbodySteps').append(html);

            $(".select2-material").select2({
                dropdownParent: $("#staticBackdrop")
            });
            $(".material-Id").each(function (index, el) {
                $(el).val(data.details[index].MaterialId).trigger("change");
            });
            
            $(".material-quantity").each(function (index, el) {
                $(el).val(data.details[index].Quantity);
            });

            var htmlFile = '';
            _attachFiles = data.files.map(x => ({ id: x.id, name: x.FileName }));
            $.each(_attachFiles, function (key, item) {
                htmlFile += `<p class="m-0 px-1 text-nowrap text-dark a-product-details">${item.name}<span class="text-danger icon-x-span" onclick="return onRemoveFile(${key},${item.id})"><i class="bi bi-x"></i></span></p>`;
            });
            $('#AttachFiles').html(htmlFile);

        },
        error: function (err) {
            alert(err.responseText);
        }
    });
    showModal();
}
function Validate() {
    let isValid = true;
    let dateReceipt = $("#formIssueDate").val();

    if (dateReceipt == null || dateReceipt == "") {
        alert("Vui lòng chọn Ngày xuất kho!");
        isValid = false;
        return false;
    }
    let isMaterial = 0;
    if ($(".goodsReceipt_details_item").length <= 0) {
        alert("Vui lòng nhập Chi tiết phiếu xuất!");
        isValid = false;
        return false;
    }
    $(".goodsReceipt_details_item").each(function (index, el) {
        let price = ($(el).find(".material-price").val());
        let materialID = parseInt($(el).find(".material-Id").val());
        let materialQuantity = parseInt($(el).find(".material-quantity").val());
        if (!materialID || materialID <= 0) {
            alert("Trường Nguyên vật liệu không được bỏ trống!");
            isValid = false;
            return false;
        }
       
        if (materialQuantity <= 0) {
            alert("Vui lòng nhập đủ số lượng xuất cho Nguyên vật liệu!");
            isValid = false;
            return false;
        }

        if (isMaterial == materialID) {
            alert("Không được tồn tại nguyên vật liệu giống nhau!");
            isValid = false;
            return false;
        }
    });
    return isValid;
}
function CreateOrUpdate() {
    if (Validate()) {
        ShowSpinnerClient();
        let arrDetails = [];
        $(".goodsReceipt_details_item").each(function (index, el) {
            let materialID = parseInt($(el).find(".material-Id").val());
            let quantity = parseInt($(el).find(".material-quantity").val());

            let objDetails = {
                Id: 0,
                MaterialId: materialID,
                Quantity: quantity,
            };
            arrDetails.push(objDetails);
        });

        var obj = {
            Id: modelID,
            GoodIssueCode: $("#formCode").val(),
            IssueDate: $("#formIssueDate").val(),
            Decription: $("#formDecription").val(),
            LstDetails: arrDetails
        };
        let _url = "/Admin/GoodsIssue/CreateOrUpdate";
        $.ajax({
            type: 'POST',
            url: _url,
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(obj),
            success: function (result) {
                if (result.status == 0) {
                    alert(result.statusText)
                } else {
                    UploadFile(result.result.id);
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
        let _url = "/Admin/GoodsIssue/Delete";
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

function GetAllMaterial() {
    $.ajax({
        type: 'GET',
        url: "/Admin/Material/GetAllForView",
        contentType: 'application/json;charset=utf-8',
        data: {},
        success: function (result) {
            htmlMaterial = `<option value="0" disabled selected hidden>Chọn nguyên vật liệu</option>`;
            result.forEach(e => {
                htmlMaterial += `<option value="${e.Id}" unit-data="${e.UnitId}">${e.MaterialName} (${e.MaterialCode})</option>`;
            })
        }, error: function (err) {
            alert(err.responseText);
        }
    });
}

function GetAllUnit() {
    $.ajax({
        type: 'GET',
        url: "/Admin/Unit/GetAllForView",
        contentType: 'application/json;charset=utf-8',
        data: {},
        success: function (result) {
            htmlUnit = `<option value="0">--Chọn đơn vị--</option>`;
            result.forEach(e => {
                htmlUnit += `<option value="${e.Id}">${e.UnitName}</option>`;
            })
        }, error: function (err) {
            alert(err.responseText);
        }
    });
}


function addRow() {
    let html = `<tr class="sortable goodsReceipt_details_item">
                <th scope="col">
                       <select class="form-select material-Id select2-material" onchange="onSelectedMaterial(event)">
                                   ${htmlMaterial}    
                       </select>
                </th>
                <th scope="col">
                        <input type="number" class="form-control form-control-sm material-quantity" min="0" value="0">
                </th>
                 <th scope="col">
                        <select class="form-select select2-material material-unit" readonly disabled>
                                   ${htmlUnit}    
                       </select>
                </th>

                <th scope="col" style="text-align:center;vertical-align: middle;"><button  onclick="deleteRow(event)" class="btn btn-danger"><i class="bi bi-trash"></i></th>
            </tr>`;
    $('#tbodySteps').append(html);


    $(".select2-material").select2({
        dropdownParent: $("#staticBackdrop")
    });
}
function onSelectedMaterial(event) {
    let targetElementTR = $(event.target).closest("tr");
    let elSelected = $(targetElementTR).find(".material-Id").find('option:selected');
    $(targetElementTR).find(".material-unit").val($(elSelected).attr("unit-data")).trigger("change");
}

function deleteRow(event) {
    if (confirm("Bạn có chắc muốn xóa dòng này không?")) {
        let targetElement = $(event.target);
        targetElement.closest("tr").remove();
    }
}

function UploadFile(modelID) {
    try {
        var filedata = new FormData();
        if (_attachFiles.length > 0) {
            $.each(_attachFiles, function (key, item) {
                filedata.append(key, item);
            })

            $.ajax({
                url: '/Admin/GoodsIssue/UploadFile?Id=' + modelID,
                type: 'POST',
                dataType: 'json',
                data: filedata,
                processData: false,
                contentType: false,
                success: function (result) {
                    if (parseInt(result.status) == 1) {
                        _attachFiles = [];
                        $("#AttachFiles").html('');
                    } else {
                        alert(result.message);
                    }
                },

                error: function (err) {
                    alert(err.responseText);
                }
            });
        }

    } catch (e) {
        alert(e);
    }
}

async function onSelectedFile(event) {

    var html = '';
    var fileSelecteds = $('input[name="AttachFiles"]').get(0).files;
    $.each(fileSelecteds, function (i, file) {
        _attachFiles.push(file);
    })

    $.each(_attachFiles, function (key, item) {
        html += `<p class="m-0 px-1 text-nowrap text-dark a-product-details">${item.name}<span class="text-danger icon-x-span" onclick="return onRemoveFile(${key},0)"><i class="bi bi-x"></i></span></p>`;

    })
    $('#AttachFiles').html(html);
}
function onRemoveFile(index, fileID) {
    if (confirm("Bạn có chắc muốn xóa file này không?")) {
        _attachFiles.splice(index, 1);
        var html = '';
        $.each(_attachFiles, function (key, item) {
            html += `<p class="m-0 px-1 text-nowrap text-dark a-product-details">${item.name}<span class="text-danger icon-x-span" onclick="return onRemoveFile(${key},${item.id})"><i class="bi bi-x"></i></span></p>`;
        });
        $('#AttachFiles').html(html);
    }
    let idFile = parseInt(fileID);
    if (idFile > 0) lstFileDeleted.push(idFile);

}

function LoadGoodsIssueCode() {
    $.ajax({
        type: 'GET',
        url: "/Admin/GoodsIssue/LoadCode",
        contentType: 'application/json;charset=utf-8',
        data: {},
        success: function (result) {
            $("#formCode").val(result.code);
        }, error: function (err) {
            alert(err.responseText);
        }
    });
}