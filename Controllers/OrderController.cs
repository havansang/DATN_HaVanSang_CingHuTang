using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;
using CingHuTang.Reposiory;

namespace CingHuTang.Controllers
{
    public class OrderController : Controller
    {
        public AccountRepository _accRepo = new AccountRepository();
        public OrderRepository _repo = new OrderRepository();
        public OrderDetailsRepository _detailRepo = new OrderDetailsRepository();
        public OrderDetailsToppingRepository _detailToppingRepo = new OrderDetailsToppingRepository();
        public CartRepository _cartRepo = new CartRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Redirect("/Shop/Index");
            }
            DateTime date = DateTime.Now;
            ViewBag.FirstDay = new DateTime(date.Year, date.Month, 1);
            ViewBag.LastDay = ViewBag.FirstDay.AddMonths(1).AddDays(-1);
            return View();
        }

        public IActionResult OrderDetails(int OrderId = 0)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Redirect("/Shop/Index");
            }
            ViewBag.Order = _repo.GetByID(OrderId) ?? new Order();
            List<OrderDetailsDto> lst = SQLHelper<OrderDetailsDto>.ProcedureToList("spGetOrderDetails",
                                                                                    new string[] { "@OrderId" },
                                                                                    new object[] { OrderId });
            foreach (OrderDetailsDto item in lst)
            {
                item.lstTopping = SQLHelper<OrderDetailsToppingDTO>.SqlToList($"SELECT odt.*, t.ToppingName, t.ToppingCode FROM dbo.OrderDetailsTopping AS odt LEFT JOIN dbo.Topping AS t ON odt.ToppingID = t.ID WHERE odt.OrderDetailsID = {item.OrderDetailID}");
            }
            ViewBag.Details = lst;
            decimal totalMoney = 0;

            foreach (var item in lst)
            {
                foreach (var topping in item.lstTopping)
                {
                    totalMoney += TextUtils.ToDecimal(topping.ToppingPrice);
                }
                totalMoney += item.TotalMoney;
            }
            ViewBag.Total = totalMoney;

            return View();
        }

        public JsonResult GetAll([FromBody] InputDto dto)
        {
           

            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Json(new { status = 0, message = "Hãy đăng nhập để sử dụng tính năng này!" });
            }
            DateTime dateStart = new DateTime(dto.dateStart.Value.Year, dto.dateStart.Value.Month, dto.dateStart.Value.Day, 0, 0, 0);;
            DateTime dateEnd = new DateTime(dto.dateEnd.Value.Year, dto.dateEnd.Value.Month, dto.dateEnd.Value.Day, 23, 59, 59); ;
            List<OrderDto> lst = SQLHelper<OrderDto>.ProcedureToList("spGetHistoryCheckOut",
                                                                      new string[] { "@AccountId", "@Request", "@DateStart", "@DateEnd" },
                                                                      new object[] { acc.Id, TextUtils.ToString(dto.request), dateStart, dateEnd });
            foreach (OrderDto item in lst)
            {
                item.DateFormat =  item.CreateDate.Value.ToString("dd/MM/yyyy HH:mm:ss");
            }
            return Json(new { status = 1, message = "", data = lst });
        }
        [HttpPost]
        public async Task<JsonResult> CreateOrder([FromBody] OrderDto data)
        {
            try
            {
                Account accout = _accRepo.GetByID(TextUtils.ToInt(data.AccountId)) ?? new Account();
                if (accout.Id <= 0) return Json(new { status = 0, message = "Đăng nhập để sử dụng tính năng này!" });
                if (data.Details == null || data.Details.Count <= 0) return Json(new { status = 0, message = "Hãy chọn ít nhất 1 sản phẩm để tạo đơn hàng!" });

                

                Order newOrder = new Order();

                newOrder.OrderCode = LoadCode();
                newOrder.CustomerName = data.CustomerName;
                newOrder.PhoneNumber = data.PhoneNumber;
                newOrder.Address = data.Address;
                newOrder.Status = 0;
                newOrder.CreateDate = newOrder.UpdatedDate = DateTime.Now;
                newOrder.CreateBy =  accout.FullName;
                newOrder.AccountId = data.AccountId;
                newOrder.IsDeleted = false;

                await _repo.CreateAsync(newOrder);

                foreach (var item in data.Details)
                {
                    OrderDetail newOderDetails = new OrderDetail();
                    newOderDetails.Quantity = item.Quantity;
                    newOderDetails.TotalMoney = item.TotalMoney;
                    newOderDetails.ProductDetailId = item.ProductDetailId;
                    newOderDetails.OrderId = newOrder.Id;
                    newOderDetails.CreatedDate = DateTime.Now;
                    newOderDetails.CreatedBy = accout.FullName;
                    newOderDetails.IsDelete = false;
                    await _detailRepo.CreateAsync(newOderDetails);

                    foreach (var topping in item.LstTopping)
                    {
                        OrderDetailsTopping newTopping = new OrderDetailsTopping()
                        {
                            OrderDetailsId = newOderDetails.Id,
                            ToppingId = topping.ToppingId,
                            ToppingPrice = topping.ToppingPrice,
                            CreatedDate = DateTime.Now,
                            CreatedBy = accout.FullName + "_" + accout.Id.ToString(),
                            UpdatedDate = DateTime.Now,
                            UpdatedBy = accout.FullName+ "_" + accout.Id.ToString(),
                        };
                        _detailToppingRepo.Create(newTopping);
                    }

                }


                List<Cart> lstCart = _cartRepo.GetAll().Where(p=> p.AccountId == accout.Id).ToList();
                if(lstCart.Count > 0)
                {
                    string stringCartIDs = string.Join(",", lstCart.Select(p => p.Id));
                    SQLHelper<CartTopping>.SqlToModel($"DELETE dbo.CartTopping WHERE CartID IN ({stringCartIDs})");
                }
                SQLHelper<Cart>.SqlToModel($"DELETE FROM Cart WHERE AccountID = {accout.Id}");

                return Json(new { status = 1, message = "Đặt hàng thành công!" });
            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });
            }

        }

        public string LoadCode()
        {
            int currentYear = DateTime.Now.Year;
            List<Order> lst = SQLHelper<Order>.SqlToList($"SELECT * FROM [Order] WHERE YEAR(CreateDate) = {currentYear}");
            string numberCode = (lst.Count + 1).ToString(); 
            while (numberCode.Length < 7)
            {
                numberCode = "0" + numberCode;
            }
            string code = $"CFS{currentYear}{numberCode}";
            return code;

        }
    }
}
