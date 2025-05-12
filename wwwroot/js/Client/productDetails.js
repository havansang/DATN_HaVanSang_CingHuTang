var ProductDetailsId = 0;
$(document).ready(function (e) {



    function setStars(value) {
        $('.rating label').each(function () {
            const label = $(this);
            const forAttr = label.attr('for');
            const input = $('#' + forAttr);

            const icon = label.find('i');
            if (parseInt(input.val()) <= parseInt(value)) {
                icon.removeClass('bi-star').addClass('bi-star-fill');
                label.addClass('filled');
            } else {
                icon.removeClass('bi-star-fill').addClass('bi-star');
                label.removeClass('filled');
            }
        });
    }

    // Hover effect
    $('.rating label').hover(function () {
        const forAttr = $(this).attr('for');
        const value = $('#' + forAttr).val();
        setStars(value);
    }, function () {
        // Khi rời chuột ra ngoài, reset theo input đã chọn
        const checkedInput = $('.rating input[type="radio"]:checked');
        if (checkedInput.length) {
            setStars(checkedInput.val());
        } else {
            setStars(0);
        }
    });

    // Click chọn
    $('.rating input[type="radio"]').change(function () {
        setStars($(this).val());
    });







    $("#lst-product-size li:first a").click();
    loadReviews();
});

function ChangeSize(event) {
    let el = $(event.target);
    let price = $(el).attr("price");
    ProductDetailsId = $(el).attr("ProductDetailsId");
    price = price.replace(/[^0-9]/g, '');
    price = price.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    $("#Price").html(`${price} VNĐ`)
}

function showToast(msg, success = true) {
    $('#toast-message')
        .css('background', success ? '#28a745' : '#dc3545')
        .text(msg)
        .fadeIn(200)
        .delay(1500)
        .fadeOut(400);
}


function AddCart(event) {
    let accountId = $("#account_id").val();
    if (accountId <= 0) {
        alert("Hãy đăng nhập để sử dụng được chức năng!");
        return;
    }
    if (ProductDetailsId <= 0) {
        alert("Hãy chọn size sản phẩm!");
        return;
    }
    let quantity = $("#product-quanity").val();
    let toppingIDs = [];
    $(".input-topping").each(function (index, el) {
        let isChecked = $(el).is(':checked');
        if (isChecked) {
            let toppingID = parseInt($(el).attr("topping-id"));
            toppingIDs.push(toppingID);
        }
    });


    let object = {
        ProductDetailID: ProductDetailsId,
        AccountID: accountId,
        Quantity: quantity,
        ToppingIDs: toppingIDs
    };
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(object),
        success: function (result) {
            if (parseInt(result.status) == 1) {
                showToast(result.message, parseInt(result.status) == 1);
            } else {
                alert("Lỗi hệ thống!");
            }
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}

function changeImage(event) {
    let el = $(event.target);
    let imgSrc = $(el).attr("src");
    $("#product-image").attr("src", imgSrc);
}

// Xử lý form đánh giá
document.getElementById('review-form').addEventListener('submit', function (e) {
    e.preventDefault();

    const rating = document.querySelector('input[name="rating"]:checked').value;
    const comment = document.getElementById('review-content').value;
    const productId = document.getElementById('product_id').value;
    const accountId = document.getElementById('account_id').value;
    if (accountId <= 0) {
        alert("Hãy đăng nhập để sử dụng tính năng!");
        return;
    }

    fetch('/Review/AddReview', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            ProductId: productId,
            AccountId: accountId,
            Rating: rating,
            Comment: comment,
            ReviewDate: new Date().toISOString()
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showToast('Đánh giá của bạn đã được gửi!');
                loadReviews();

                document.getElementById('review-form').reset();
            } else {
                showToast('Có lỗi xảy ra khi gửi đánh giá!', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('Có lỗi xảy ra khi gửi đánh giá!', 'error');
        });
});

// Hàm tải danh sách đánh giá
function loadReviews() {
    const productId = document.getElementById('product_id').value;
    const reviewsContainer = $('#reviews-list');
    // Hiển thị trạng thái loading
    reviewsContainer.html(`
        <div class="text-center py-3">
            <div class="spinner-border text-success" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p>Đang tải đánh giá...</p>
        </div>
    `);

    $.ajax({
        url: "/Review/GetProductReviews",
        type: 'GET',
        dataType: 'json',
        data: { productId: productId },
        success: function (data) {
            // Xóa nội dung cũ
            reviewsContainer.html('<h5 class="mb-3">Đánh giá từ khách hàng</h5>');
            // Kiểm tra trạng thái
            if (data.status !== 1) {
                reviewsContainer.append(`
                    <div class="alert alert-warning">
                        ${data.message || 'Không thể tải đánh giá'}
                    </div>
                `);
                return;
            }

            // Kiểm tra dữ liệu rỗng
            if (!data.data || data.data.length === 0) {
                reviewsContainer.append(`
                    <div class="alert alert-info">
                        Chưa có đánh giá nào cho sản phẩm này.
                    </div>
                `);
                return;
            }

            // Duyệt qua từng đánh giá
         
            let htmlReviews = '';
            $.each(data.data, function (index, item) {
                    // Tạo sao đánh giá
                    let stars = '';
                    for (let i = 1; i <= 5; i++) {
                        stars += i <= item.Rating
                            ? '<i class="bi bi-star-fill text-warning"></i>'
                            : '<i class="bi bi-star text-warning"></i>';
                    }

                    // Format ngày tháng
                //const reviewDate = ${moment(item.CreatedDate).format("DD/MM/YYYY")};
                const reviewDate = moment(item.ReviewDate).format("DD/MM/YYYY");

                    htmlReviews += `
                    <div class="card mb-3">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-center">
                                <h6 class="card-subtitle mb-2 text-muted">${item.FullName}</h6>
                                <div class="text-warning">
                                    ${stars}
                                </div>
                            </div>
                            <p class="card-text mt-2">${item.Comment}</p>
                            <small class="text-muted">Đăng ngày ${reviewDate}</small>
                        </div>
                    </div>
                    `;
            });

            reviewsContainer.append(htmlReviews);
            
        },
        error: function (xhr, status, error) {
            reviewsContainer.html(`
                <h5 class="mb-3">Đánh giá từ khách hàng</h5>
                <div class="alert alert-danger">
                    Lỗi khi tải đánh giá: ${xhr.responseJSON?.message || error}
                </div>
            `);
        }
    });
}

//$('.rating > label').each(function () {
//    const icon = $(this).find('i');
//    if (icon.length) {
//        if (icon.hasClass('bi-star')) {
//            icon.removeClass('bi-star').addClass('bi-star-fill');
//        } else {
//            icon.removeClass('bi-star-fill').addClass('bi-star');
//        }
//    }
//});
// Tải đánh giá khi trang được load
//document.addEventListener('DOMContentLoaded', function () {
//    loadReviews();
//});