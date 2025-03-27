var _attachFiles = [];


$(document).ready(function (e) {
    $("#Account_Gender").val($("#Account_Gender_Data").val());
});


function reset() {
    $("#Account_Old_Password").val("");
    $("#Account_New_Password").val("");
    $("#Account_Confirm_Password").val("");
}
function UpdateLoadData() {
    try {
        let isValid = true;
        let fullName = $("#Account_FullName").val().trim();
        let address = $("#Account_Address").val().trim();
        let phoneNumber = $("#Account_PhoneNumber").val().trim();
        let gender = parseInt($("#Account_Gender").val());
        if (fullName == "") {
            isValid = false;
            alert("Vui lòng nhập Họ và tên!");
        }
        else if (phoneNumber == "") {
            isValid = false;
            alert("Vui lòng nhập Số điện thoại")
        }


        if (isValid) {
            ShowSpinnerClient();

            let obj = {
                Id: parseInt($("#Account_ID").val()),
                FullName: fullName,
                Gender: gender,
                PhoneNumber: phoneNumber,
                Address: address,
            };
            $.ajax({
                url: '/AccountDetails/UpdateAccount',
                type: 'POST',
                dataType: 'json',
                data: JSON.stringify(obj),
                contentType: 'application/json;charset=utf-8',
                success: function (result) {
                    if (parseInt(result.status) == 1) {
                        alert(result.message);
                        UploadFile(result.data.id);
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
function UploadFile(accountID) {
    try {
        var filedata = new FormData();
        if (_attachFiles.length > 0) {
            $.each(_attachFiles, function (key, item) {
                filedata.append(key, item);
            })

            $.ajax({
                url: '/AccountDetails/UploadFile?Id=' + accountID,
                type: 'POST',
                dataType: 'json',
                data: filedata,
                processData: false,
                contentType: false,
                success: function (result) {
                    if (parseInt(result.status) == 1) {
                        _attachFiles = [];
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

function ChangePassword() {
    let isValid = true;
    let oldPassword = $("#Account_Old_Password").val().trim();
    let newPassword = $("#Account_New_Password").val().trim();
    let confirmPassword = $("#Account_Confirm_Password").val().trim();

    if (oldPassword == "") {
        isValid = false;
        alert("Vui lòng nhập Mật khẩu cũ!");
    }
    else if (newPassword == "") {
        isValid = false;
        alert("Vui lòng nhập Mật khẩu mới!");
    }
    else if (confirmPassword == "") {
        isValid = false;
        alert("Vui lòng nhập Mật khẩu xác thực!");
    }
    else if (confirmPassword != newPassword) {
        isValid = false;
        alert("Mật khẩu xác thực không chính xác!");
    }


    if (isValid) {
        ShowSpinnerClient();

        let obj = {
            AccountID: parseInt($("#Account_ID").val()),
            OldPassword: oldPassword,
            NewPassword: newPassword,
            ConfirmPassword: confirmPassword,
        };
        $.ajax({
            url: '/AccountDetails/UpdatePassword',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(obj),
            contentType: 'application/json;charset=utf-8',
            success: function (result) {
                if (result.status == 1) {
                    reset();
                }
                alert(result.message);
            },
            error: function (err) {
                alert(err.responseText);
            }
        });
    }
}


function onSelectedFile(event) {
    _attachFiles.push(event.target.files[0]);

    var reader = new FileReader();
    reader.readAsDataURL(event.target.files[0]);
    reader.onloadend = (value) => {
        $("#main-user-image").attr("src", value.target.result);
    }

}