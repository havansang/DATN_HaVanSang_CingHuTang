$(document).ready(() => {
    try {
        ChartTopSale();
        GetDataForMessage();
        ChartHardestToSell();
        ChartBestSaleTopping()

        ChartPercentSuccessOrder();
        GetFiveTopSale();
        ChartPuchase();
        ChartTotalRevanue();
    } catch (exrr) {

    }
    setInterval(function () {
        GetDataForMessage();
    }, 60000);


    setInterval(function () {
        try {
            ChartTopSale();
            GetDataForMessage();
            ChartHardestToSell();
            ChartBestSaleTopping()

            ChartPercentSuccessOrder();
            GetFiveTopSale();
            ChartPuchase();
            ChartTotalRevanue();
        } catch (exrr) {

        }
    }, 75000);


    ShowSpinnerClient();

})

function getTopSale() {
    const topSale = $('#selected_top_sale').val();
    const dateStart = $('#dateStart_top_sale').val();
    const dateEnd = $('#dateEnd_top_sale').val();
    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetTopSale`,
            data: { topSale, dateStart, dateEnd },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}

async function ChartTopSale() {
    let resultTopSale = await getTopSale();
    let data = [];
    let categories = [];
    if (resultTopSale) {
        resultTopSale.forEach(res => {
            data.push(parseInt(res.totalSales));
            categories.push(res.productName);
        })
    }

    var options = {
        series: [{
            name: 'Lượt bán',
            data: data
        }],
        chart: {
            height: 250,
            type: 'bar',
        },
        plotOptions: {
            bar: {
                borderRadius: 0,
                dataLabels: {
                    position: 'center', // top, center, bottom
                },
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val) {
                return val;
            },
            offsetY: -20,
            style: {
                fontSize: '12px',
                colors: ["#304758"]
            }
        },

        xaxis: {
            categories: categories,
            position: 'bottom',
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            crosshairs: {
                fill: {
                    type: 'gradient',
                    gradient: {
                        colorFrom: '#D8E3F0',
                        colorTo: '#BED1E6',
                        stops: [0, 100],
                        opacityFrom: 0.4,
                        opacityTo: 0.5,
                    }
                }
            },
            tooltip: {
                enabled: true,
            }
        },
        yaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false,
            },
            labels: {
                show: false,
                formatter: function (val) {
                    return val + " sản phẩm";
                }
            }

        }
    };
    try {
        $("#chart_top_sale").html("");
        var chart = new ApexCharts(document.querySelector("#chart_top_sale"), options);
        chart.render();
    } catch (exrr) {

    }
}


function getHardestToSell() {
    const topSale = $('#selected_hardest_to_sell').val();
    const dateStart = $('#dateStart_hardest_to_sell').val();
    const dateEnd = $('#dateEnd_hardest_to_sell').val();
    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetHardestToSell`,
            data: { topSale, dateStart, dateEnd },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}

async function ChartHardestToSell() {
    let resultHardestToSell = await getHardestToSell();
    let data = [];
    let categories = [];

    if (resultHardestToSell) {
        resultHardestToSell.forEach(res => {
            data.push(parseInt(res.totalSales));
            categories.push(res.productName);
        })
    }

    var options = {
        series: [{
            name: 'Lượt bán',
            data: data
        }],
        chart: {
            height: 250,
            type: 'bar',
        },
        plotOptions: {
            bar: {
                borderRadius: 0,
                dataLabels: {
                    position: 'center', // top, center, bottom
                },
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val) {
                return val;
            },
            offsetY: -20,
            style: {
                fontSize: '12px',
                colors: ["#304758"]
            }
        },

        xaxis: {
            categories: categories,
            position: 'bottom',
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            crosshairs: {
                fill: {
                    type: 'gradient',
                    gradient: {
                        colorFrom: '#D8E3F0',
                        colorTo: '#BED1E6',
                        stops: [0, 100],
                        opacityFrom: 0.4,
                        opacityTo: 0.5,
                    }
                }
            },
            tooltip: {
                enabled: true,
            }
        },
        yaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false,
            },
            labels: {
                show: false,
                formatter: function (val) {
                    return val + " sản phẩm";
                }
            }

        }

    };

    try {
        $("#chart_hardest_to_sell").html("");
        var chart = new ApexCharts(document.querySelector("#chart_hardest_to_sell"), options);
        chart.render();
    } catch (exrr) {

    }
}

function getPuchase() {
    let dateStr = $('#inputMonth').val()
    if (!dateStr) return;
    let parts = dateStr.split('-');
    let year = parseInt(parts[0], 0);
    let month = parseInt(parts[1], 0);
    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetPuchase`,
            data: { month, year },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}

async function ChartPuchase() {
    let resultPuchase = await getPuchase();
    let data = [];
    let categories = [];

    if (resultPuchase) {
        resultPuchase.forEach(res => {
            data.push(parseInt(res.totalMoney));
            let parts = res.dayInMonth.split('-');
            categories.push(parseInt(parts[0], 0));
        })
    }
    var options = {
        series: [{
            name: "VNĐ",
            data: data
        }],
        chart: {
            height: '100%',
            type: 'bar',
        },
        plotOptions: {
            bar: {
                borderRadius: 0,
                dataLabels: {
                    position: 'center',
                },
            }
        },
        dataLabels: {
            enabled: true,
            formatter: function (val) {
                return val.toLocaleString('en-US');
            },
            offsetY: -20,
            style: {
                fontSize: '8px',
                colors: ["#304758"]
            }
        },

        xaxis: {
            categories: categories,
            position: 'bottom',
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false
            },
            crosshairs: {
                fill: {
                    type: 'gradient',
                    gradient: {
                        colorFrom: '#D8E3F0',
                        colorTo: '#BED1E6',
                        stops: [0, 100],
                        opacityFrom: 0.4,
                        opacityTo: 0.5,
                    }
                }
            },
            tooltip: {
                enabled: true,
            }
        },
        yaxis: {
            axisBorder: {
                show: false
            },
            axisTicks: {
                show: false,
            },
            labels: {
                show: false,
                formatter: function (val) {
                    return val.toLocaleString('en-US') + " VNĐ";
                }
            }

        }

    };
    $("#chart_puchase").html("");
    try {
        var chart = new ApexCharts(document.querySelector("#chart_puchase"), options);
        chart.render();
    } catch (exrr) {

    }
}


function onchangeSelectTop(id, idText) {
    let val = $(id).val();
    if (idText == '#title_top_sale') {
        ChartTopSale();
        $(idText).text(`Top ${val} sản phẩm bán chạy`);
    }
    else {
        ChartHardestToSell();
        $(idText).text(`Top ${val} sản phẩm bán ít nhất`);
    }

}

function reloadChartPuchase() {
    let dateStr = $("#inputMonth").val();
    let month = dateStr.split("-")[1].replace(/^0+/, '');;
    let year = dateStr.split("-")[0].replace(/^0+/, '');;
    let text = `Doanh thu tháng ${month}-${year}`;

    $("#title_puchase").text(text);
    ChartPuchase();
}


//===============================================================================================================================================================
let typeTable = 1
function getOrderPercent() {

    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetPercentOrderSuccess`,
            data: { typeTable },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}
function SetTypeTable(id) {
    typeTable = id;
    let headerText = id == 1 ? "Tuần" : (id == 2 ? "Tháng" : "Năm")
    $("#title_percent_order_success").text(`Tỉ lệ đơn hàng thành công theo ${headerText}`);
    ChartPercentSuccessOrder();
}
async function ChartPercentSuccessOrder() {
    let resultPercentSuccessOrder = await getOrderPercent();
    if (resultPercentSuccessOrder.status != 1) return;
    var options = {
        series: [resultPercentSuccessOrder.data.totalSuccess, resultPercentSuccessOrder.data.totalFalse],
        chart: {
            width: 380,
            type: 'pie',
        },
        colors: ['#008FFB', '#F44336'],
        labels: ['Thành công', 'Bị hủy']
    };
    try {
        $("#chart_percent_order_success").html("");
        var chart = new ApexCharts(document.querySelector("#chart_percent_order_success"), options);
        chart.render();
    } catch (exrr) {

    }
}


let typeTopping = 1
function getBestSaleTopping() {

    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetTopBestSaleTopping`,
            data: { typeTopping },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}
function SetTypeTopping(id) {
    typeTopping = id;
    let headerText = id == 1 ? "Tuần" : (id == 2 ? "Tháng" : "Năm")
    $("#title_topping_best_sale").text(`Top 5 Topping bán chạy theo ${headerText}`);
    ChartBestSaleTopping();
}
async function ChartBestSaleTopping() {
    let resultSaleTopping = await getBestSaleTopping();
    if (resultSaleTopping == null || resultSaleTopping.status != 1) return;
    let data = [];
    let catalog = [];

    resultSaleTopping.data.forEach(res => {
        data.push(parseInt(res.totalSale));
        catalog.push(res.toppingName);
    })

    var options = {
        series: data,
        chart: {
            width: 380,
            type: 'pie',
        },
        labels: catalog
    };

    try {
        $("#chart_topping_best_sale").html("");
        var chart = new ApexCharts(document.querySelector("#chart_topping_best_sale"), options);
        chart.render();
    } catch (exrr) {

    }
}



function GetDataForMessage() {
    $.ajax({
        url: "/Admin/Home/GetAllInformationOrder",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            let totalInCreaseSale = (data.totalCurrentWeek - (data.totalLastWeek == 1 ? 0 : data.totalLastWeek)) / data.totalLastWeek
            let htmlSale = `
                            <h6 >${data.totalCurrentWeek.toLocaleString('en-US')} lượt</h6>
                            <span class="${totalInCreaseSale > 0 ? 'text-success' : 'text-danger'} small pt-1 fw-bold">${(Math.abs(totalInCreaseSale)).toFixed(2)}%</span> <span class="text-muted small pt-2 ps-1">${totalInCreaseSale > 0 ? 'tăng' : 'giảm'}</span>`
            $("#total_sales").html(htmlSale);

            let totalInCreaseRevenue = (data.moneyCurrentWeek - (data.moneyLastWeek == 1 ? 0 : data.moneyLastWeek)) / data.moneyLastWeek
            let htmlRevenue = `
                            <h6 >${data.moneyCurrentWeek.toLocaleString('en-US')} VNĐ</h6>
                            <span class="${totalInCreaseRevenue >= 0.00 ? 'text-success' : 'text-danger'} small pt-1 fw-bold">${(Math.abs(totalInCreaseRevenue)).toFixed(2)}%</span> <span class="text-muted small pt-2 ps-1">${totalInCreaseRevenue >= 0.00 ? 'tăng' : 'giảm'}</span>`
            $("#total_revenue").html(htmlRevenue);

            $("#total_order_unprocess").text(`${data.totalUnProcess} ĐH`);
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}

function GetFiveTopSale() {
    let today = new Date();
    let sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(today.getDate() - 7);
    $.ajax({
        url: "/Admin/Home/GetTopSale",
        type: 'GET',
        data: {
            topSale: 5,
            dateStart: sevenDaysAgo.toISOString().split('T')[0],
            dateEnd: today.toISOString().split('T')[0]
        },
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            let html = ''
            $.each(data, function (index, item) {
                html += `<tr class="align-middle text-center">
                                        <th scope="row"><img src="${item.imageUrl}" alt="" style="width: 100%"></th>
                                        <td class="fw-bold">${item.productName}</td>
                                        <td class="">${item.price.toLocaleString('en-US')} VNĐ</td>
                                        <td class="fw-bold ">${item.totalSales.toLocaleString('en-US')}</td>
                                        <td class="">${item.productRevenue.toLocaleString('en-US')} VNĐ</td>
                                    </tr>`
            });
            $("#product_bestsale_week").html(html);


        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}



let typeFilter = 1
function getTotalRevanue() {

    let dateStart = $("#dateStart_total_revenuee").val();
    let dateEnd = $("#dateEnd_total_revenue").val();

    $("#title_chart_total_revenuee").text(`${moment(dateStart).format('DD/MM/YYYY')} - ${moment(dateEnd).format('DD/MM/YYYY')}`);

    return new Promise(resolve => {
        $.get({
            url: `/Admin/Home/GetToToalRevenue`,
            data: { dateStart, dateEnd },
            success: data => resolve(data),
            error: error => resolve(null)
        })
    })
}
function SetTypeFilter(id) {
    typeFilter = id;
    let headerText = id == 1 ? "Tuần" : (id == 2 ? "Tháng" : "Năm")
    $("#chart_total_revenue_filter_text").text(headerText);
    ChartTotalRevanue();
}
async function ChartTotalRevanue() {
    let resultTotalRevanue = await getTotalRevanue();
    if (resultTotalRevanue.status != 1) return;
    let totalRevenue = resultTotalRevanue.data.totalMoneyOrder - resultTotalRevanue.data.totalMoneyReceipt
    let htmlRevenue = ` <h6 class="${totalRevenue >= 0 ? 'text-success' : 'text-danger'}">${totalRevenue.toLocaleString('en-US')} VNĐ</h6>`
    $("#chart_total_revenue_html").html(htmlRevenue);

    var options = {
        series: [resultTotalRevanue.data.totalMoneyOrder, resultTotalRevanue.data.totalMoneyReceipt],
        chart: {
            width: 380,
            type: 'pie',
        },
        labels: ["Tổng thu", "Tổng chi"],
        legend: {
            position: 'bottom',
            show: true,
            formatter: function (seriesName, opts) {
                // Tạo custom label cho legend
                const value = opts.w.globals.series[opts.seriesIndex];
                return `${seriesName}: ${value.toLocaleString('en-US')} VNĐ`;
            }
        }
    };
    try {
        $("#chart_total_revenue").html("");
        var chart = new ApexCharts(document.querySelector("#chart_total_revenue"), options);
        chart.render();
    } catch (exrr) {

    }
}