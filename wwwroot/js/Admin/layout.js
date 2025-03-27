$(document).ready(function (e) {
    GetAllNotifyMaterialForLayoutAdmin();
    setInterval(function () {
        GetAllNotifyMaterialForLayoutAdmin();
    }, 300000);
});




function GetAllNotifyMaterialForLayoutAdmin() {
    $.ajax({
        url: "/Admin/Material/GetAll",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            let countMaterialOut = 0;
            let htmlNotify = `
                        <li class="dropdown-header">
                            Bạn có @TotalMaterialOut nguyên vật sắp hết
                            <a href="/Admin/Material"><span class="badge rounded-pill bg-primary p-2 ms-2">Chi tiết</span></a>
                        </li>
                        <li>
                            <hr class="dropdown-divider">
                        </li>
            `;
            $.each(data.dataTotal, function (index, item) {
                if (item.ToltalQuantity <= item.MinQuantity) {
                    countMaterialOut++;
                    htmlNotify += `
                        <li class="notification-item">
                            <i class="bi bi-exclamation-circle text-warning"></i>
                            <div>
                                <h4>${item.MaterialName}</h4>
                                <p>Còn lại ${item.ToltalQuantity}</p>
                            </div>
                        </li>
                        <li>
                            <hr class="dropdown-divider">
                        </li>
                    `;
                }
            })
            htmlNotify = htmlNotify.replace("@TotalMaterialOut", `${countMaterialOut}`);
            $("#layout_notify_material_out_total").text(countMaterialOut);
            $("#layout_notify_material_out_message").html(htmlNotify);
        },

        error: function (err) {
            alert(err.responseText);
        }
    });
}