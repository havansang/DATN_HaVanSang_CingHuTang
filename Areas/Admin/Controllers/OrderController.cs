using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;
using MesWeb.Models.CommonConfig;
using System.Data;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        public OrderRepository _repo = new OrderRepository();
        public AccountRepository _accRepo = new AccountRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc == null || acc.Role < 2)
            {
                return Redirect("/Shop/Index");
            }
            DateTime date = DateTime.Now;
            ViewBag.FirstDay = new DateTime(date.Year, date.Month, 1);
            ViewBag.LastDay = ViewBag.FirstDay.AddMonths(1).AddDays(-1);
            return View();
        }
        public JsonResult GetAll( [FromBody] InputDto input)
        {
            input.dateStart = new DateTime(input.dateStart.Value.Year, input.dateStart.Value.Month, input.dateStart.Value.Day, 0, 0, 0);
            input.dateEnd =   new DateTime(input.dateEnd.Value.Year, input.dateEnd.Value.Month, input.dateEnd.Value.Day, 23, 59, 59);
            List<OrderDto> data = SQLHelper<OrderDto>.ProcedureToList("spGetAllOrder", new string[] { "@Request", "@PageNumber", "@Status", "@DateStart", "@DateEnd" },
                                                                    new object[] { input.request, input.pageNumber, input.status, input.dateStart, input.dateEnd });

            PaginationDto totalCount = SQLHelper<PaginationDto>.ProcedureToModel("spGetAllTotalOrder", new string[] { "@Request", "@Status", "@DateStart", "@DateEnd" },
                                                                    new object[] { input.request, input.status, input.dateStart, input.dateEnd });

            return Json(new { data, totalCount });
        }

        public JsonResult GetDetail(int OrderId)
        {
            List<OrderDetailsDto> lst = SQLHelper<OrderDetailsDto>.ProcedureToList("spGetOrderDetails",
                                                                                    new string[] { "@OrderId" },
                                                                                    new object[] { OrderId });
            Order data = _repo.GetByID(OrderId);
            foreach (var item in lst)
            {
                item.lstTopping = SQLHelper<OrderDetailsToppingDTO>.SqlToList($"SELECT odt.*, t.ToppingCode, t.ToppingName FROM dbo.OrderDetailsTopping AS odt LEFT JOIN dbo.Topping AS t ON odt.ToppingID = t.ID WHERE odt.OrderDetailsID = {item.OrderDetailID}");
            }
            return Json(new { lst, data });
        }
        public JsonResult ChangeStatusOrder(int orderId, int status, string reasonCancel = "")
        {
            Order model = _repo.GetByID(orderId) ?? new Order();
            string statusText = status == 1 ? "giao" : (status == 2 ? "xác nhận" : "hủy");
            if (model.Id <= 0) return Json(new { status = 0, message = "Không thể tìm thấy đơn hàng!" });

            if (model.Status == status) return Json(new { status = 0, message = $"Đơn hàng đã được {statusText}!" });
            model.Status = status;
            model.ReasonCancel = TextUtils.ToString(reasonCancel);
            _repo.Update(model);
            return Json(new { status = 1, message = $"Thành công!" });
        }


        public JsonResult GetAllOrderUnprocessed()
        {
            List<Order> lst = _repo.GetAll().Where(p => p.Status == 0 && p.IsDeleted == false).ToList();
            return Json(lst.Count);
        }

        public async Task<FileResult> ExportExcel(DateTime dateStart, DateTime dateEnd)
        {
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllOrder", new string[] { "@Request", "@PageNumber", "@Status", "@DateStart", "@DateEnd" }
                                                                          , new object[] { "", "1", -1,dateStart, dateEnd});
            var result = TextUtils.ConvertDataTable<OrderDto>(ds.Tables[1]);
            string[] colName = { "Mã đơn hàng", "Trạng thái", "Tổng tiền", "Khách hàng", "Số điện thoại", "Địa chỉ", "Lý do hủy" };
            string[] colValue = { "OrderCode", "StatusText", "TotalMoney", "CustomerName", "PhoneNumber", "Address", "ReasonCancel" };
            var (contentFile, contentType, fileName) = Excel.GenerateExcel("Order.xlsx", result.ToList(), colName, colValue);
            return File(contentFile,
                        contentType,
                        fileName);
        }
    }
}
