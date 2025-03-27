using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;

namespace CingHuTang.Controllers
{
    public class CartController : Controller
    {
        public CartRepository _repo = new CartRepository();
        public AccountRepository _accRepo = new AccountRepository();
        public CartToppingRepository _cartToppingRepo = new CartToppingRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Redirect("/Shop/Index");
            }
            ViewBag.Account = acc;
            return View();
        }
        public JsonResult GetCartByAccountId()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Json(new { status = 0, massage = "Đăng nhập để sử dụng tính năng!" });
            }
            List<CartDto> lst = SQLHelper<CartDto>.ProcedureToList("spGetCartByAccountId",
                                                                    new string[] { "@AccountId" },
                                                                    new object[] { acc.Id });
            foreach (CartDto item in lst)
            {
                item.lstToppings = SQLHelper<Topping>.SqlToList($"SELECT t.* FROM dbo.CartTopping AS pt LEFT JOIN dbo.Topping AS t ON pt.ToppingID = t.ID WHERE pt.CartID = {item.CartId}");
            }
            if (lst.Count == 0) return Json(new { status = 2, massage = "Bạn chưa có sản phẩm nào trong giỏ hàng!" });

            return Json(new { status = 1, massage = "", data = lst });

        }

        [HttpPost]
        public async Task<JsonResult> AddToCart([FromBody] AddToCartDTO data)
        {
            try
            {
                Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();

                if (data.AccountID <= 0 || acc.Id <= 0) return Json(new { status = 0, message = "Hãy đăng nhập để sử dụng tính năng này!" });
                if (data.ProductDetailID <= 0) return Json(new { status = 0, message = "Hãy chọn size sản phẩm!" });


                List<Cart> lst = SQLHelper<Cart>.SqlToList($"SELECT * FROM Cart WHERE AccountId = {data.AccountID}");

                // Tìm lại productDetail đã có trong giỏ hàng
                List<Cart> lstModel = lst.Where(x => x.ProductDetailId == data.ProductDetailID).ToList();
                List<CartDataDTO> lstData = new List<CartDataDTO>();
                foreach (Cart item in lstModel) 
                {
                    CartDataDTO dto = new CartDataDTO();
                    dto.Id = item.Id;
                    dto.AccountId = item.AccountId;
                    dto.ProductDetailId = item.ProductDetailId;
                    dto.Quantity = item.Quantity;
                    dto.CreatedDate = item.CreatedDate;
                    dto.CreatedBy = item.CreatedBy;
                    dto.UpdatedDate = item.UpdatedDate;
                    dto.UpdatedBy = item.UpdatedBy;
                    dto.lstTopping = SQLHelper<Topping>.SqlToList($"SELECT t.* FROM dbo.CartTopping AS pt LEFT JOIN dbo.Topping AS t ON pt.ToppingID = t.ID WHERE pt.CartID = {item.Id}");
                    lstData.Add(dto);
                }
                lstData = lstData.Where(p=> p.lstTopping.Count == data.ToppingIDs.Count).ToList();
                Cart model = new Cart();

                //Tìm cart productDetail có cx topping
                foreach (CartDataDTO item in lstData) 
                {
                    bool isValid = true;
                    for (int i = 0; i < data.ToppingIDs.Count;  i++)
                    {
                        isValid = item.lstTopping.Any(p=> p.Id == data.ToppingIDs[i]);
                        if (!isValid) break;
                    }
                    if (isValid) model = lstModel.FirstOrDefault(p=> p.Id == item.Id) ?? new Cart();
                }

                if (model.Id > 0)
                {
                    model.Quantity = model.Quantity + data.Quantity;
                    model.UpdatedBy = acc.FullName;
                    model.UpdatedDate = DateTime.Now;
                    _repo.Update(model);
                }
                else
                {
                    model.Id = 0;
                    model.ProductDetailId = data.ProductDetailID;
                    model.AccountId = data.AccountID;
                    model.Quantity = data.Quantity;
                    model.CreatedBy = acc.FullName;
                    model.CreatedDate = DateTime.Now;
                    await _repo.CreateAsync(model);
                }

                SQLHelper<CartTopping>.SqlToList($"DELETE FROM dbo.CartTopping WHERE CartID = {model.Id}");
                foreach (int toppingID in data.ToppingIDs)
                {
                    CartTopping newCartTopping = new CartTopping()
                    {
                        CartId = model.Id,
                        ToppingId = toppingID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatedBy = acc.FullName,
                        UpdatedBy = acc.FullName
                    };
                    _cartToppingRepo.Create(newCartTopping);
                }


                return Json(new { status = 1, message = "Thêm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });
            }

        }


        public async Task<JsonResult> RemoveToCart(int cartId)
        {
            try
            {
                Cart model = _repo.GetByID(cartId) ?? new Cart();
                if (model.Id > 0)
                {

                    _repo.Delete(model.Id);
                    SQLHelper<CartTopping>.SqlToList($"DELETE FROM dbo.CartTopping WHERE CartID = {model.Id}");
                }
                return Json(new { status = 1, message = "Cập nhật giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });
            }

        }
    }
}
