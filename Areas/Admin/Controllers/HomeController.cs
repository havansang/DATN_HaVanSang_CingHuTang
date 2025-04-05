using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        AccountRepository _accRepo = new AccountRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null || acc.Role < 2)
            {
                return Redirect("/Home/Index");
            }
            ViewBag.Account = acc;

            DateTime currentDate = DateTime.Now;
            DateTime dateStart = currentDate.AddDays(-30);
            ViewBag.DateStart = dateStart.ToString("yyyy-MM-dd");
            ViewBag.DateEnd = currentDate.ToString("yyyy-MM-dd");

            ViewBag.Month = currentDate.ToString("yyyy-MM");
            ViewBag.Year = currentDate.ToString("yyyy");
            ViewBag.Month1 = currentDate.ToString("MM");
            return View();
        }

        public JsonResult GetTopSale(int topSale, DateTime dateStart, DateTime dateEnd)
        {
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
            List<ProductDto> data = new List<ProductDto>();
            try
            {
                data = SQLHelper<ProductDto>.ProcedureToList("spGetTop4BestSale", new string[] { "@topSale", "@DateStart", "@DateEnd" }, new object[] { topSale, dateStart, dateEnd });
            }
            catch (Exception ex)
            {

            }
            return Json(data);
        }

        public JsonResult GetHardestToSell(int topSale, DateTime dateStart, DateTime dateEnd)
        {
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
            List<ProductDto> data = new List<ProductDto>();
            try
            {
                data = SQLHelper<ProductDto>.ProcedureToList("spGetHardestToSell", new string[] { "@topSale", "@DateStart", "@DateEnd" }, new object[] { topSale, dateStart, dateEnd });
            }
            catch (Exception ex)
            {

            }
            //List<ProductDto> data = SQLHelper<ProductDto>.ProcedureToList("spGetHardestToSell", new string[] { "@topSale", "@DateStart", "@DateEnd" }, new object[] { topSale, dateStart, dateEnd });
            return Json(data);
        }


        public JsonResult GetPuchase(int month, int year)
        {
            List<PuchaseDto> data = new List<PuchaseDto>();
            try
            {
                data = SQLHelper<PuchaseDto>.ProcedureToList("spGetTotalPuchase", new string[] { "@Month", "@Year" }, new object[] { month, year });
            }
            catch (Exception ex)
            {

            }
            //List<PuchaseDto> data = SQLHelper<PuchaseDto>.ProcedureToList("spGetTotalPuchase", new string[] { "@Month", "@Year" }, new object[] { month, year });
            return Json(data);
        }

        public JsonResult GetAllInformationOrder()
        {
            OrderHomeDTO data = SQLHelper<OrderHomeDTO>.ProcedureToModel("spGetTotalOrderForMessage", new string[] { }, new object[] { });
            return Json(data);
        }

        public JsonResult GetPercentOrderSuccess(int typeTable)
        {
            //typeTable 1: Tuần ; 2: Tháng; 3: Năm
            DateTime currentDate = DateTime.Now;
            DateTime dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            DateTime dateStart = currentDate;
            switch (typeTable)
            {
                case 1: //Tuần
                    dateStart = currentDate.AddDays(-7);
                    break;
                case 2: // Tháng
                    dateStart = currentDate.AddMonths(-1);
                    break;
                case 3: // Năm
                    dateStart = currentDate.AddYears(-1);
                    break;
                default:
                    return Json(new { status = 0, message = "Tham số không hợp lệ!" });
            }
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 00, 00, 00);


            ChartOrderSuccessDTO result = SQLHelper<ChartOrderSuccessDTO>.ProcedureToModel("spGetPercentOrderSuccess", new string[] { "@DateStart", "@DateEnd" }, new object[] { dateStart, dateEnd });
            return Json(new { status = 1, data = result });
        }

        public JsonResult GetTopBestSaleTopping(int typeTopping)
        {
            //typeTable 1: Tuần ; 2: Tháng; 3: Năm
            DateTime currentDate = DateTime.Now;
            DateTime dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            DateTime dateStart = currentDate;
            switch (typeTopping)
            {
                case 1: //Tuần
                    dateStart = currentDate.AddDays(-7);
                    break;
                case 2: // Tháng
                    dateStart = currentDate.AddMonths(-1);
                    break;
                case 3: // Năm
                    dateStart = currentDate.AddYears(-1);
                    break;
                default:
                    return Json(new { status = 0, message = "Tham số không hợp lệ!" });
            }
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 00, 00, 00);


            List<ToppingDTO> result = SQLHelper<ToppingDTO>.ProcedureToList("spGetBestSaleTopping", new string[] { "@DateStart", "@DateEnd" }, new object[] { dateStart, dateEnd });
            return Json(new { status = 1, data = result });
        }


        public JsonResult GetToToalRevenue(DateTime dateStart, DateTime dateEnd)
        {
            //typeTable 1: Tuần ; 2: Tháng; 3: Năm
            //DateTime currentDate = DateTime.Now;
            //DateTime dateEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 23, 59, 59);
            //DateTime dateStart = currentDate;
            //switch (typeFilter)
            //{
            //    case 1: //Tuần
            //        dateStart = currentDate.AddDays(-7);
            //        break;
            //    case 2: // Tháng
            //        dateStart = currentDate.AddMonths(-1);
            //        break;
            //    case 3: // Năm
            //        dateStart = currentDate.AddYears(-1);
            //        break;
            //    default:
            //        return Json(new { status = 0, message = "Tham số không hợp lệ!" });
            //}
            try
            {
                dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 00, 00, 00);
                dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);


                RevenueDTO result = SQLHelper<RevenueDTO>.ProcedureToModel("spGetSumarizeRevenue", new string[] { "@DateStart", "@DateEnd" }, new object[] { dateStart, dateEnd });
                return Json(new { status = 1, data = result });
            }
            catch (Exception exception)
            {

                return Json(new { status = 0, data = exception.Message });
            }

        }
    }

}
