
using CingHuTang.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
//using CingHuTang.Constant;
//using CingHuTang.Data;
using CingHuTang.Models;
//using CingHuTang.Models.Authentications;
//using CingHuTang.Models.ModelView;
using CingHuTang.Payments;
using static CingHuTang.Config.TextUtils;
using CingHuTang.Models.Vnpay;
using DocumentFormat.OpenXml.Drawing.Charts;
using CingHuTang.Areas.Admin.Controllers;
using CingHuTang.Services.Vnpay;

namespace CingHuTang.Controllers
{
    public class PaymentController : Controller

    {
        private readonly IConfiguration _configuration;
        private readonly IVnPayService _vnPayService;


        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
       
        public IActionResult CreatePaymentUrlVnpay1(string cusName, string nd, double tong, string ma )
        {
            PaymentInformationModel model = new PaymentInformationModel();
            model.Name = cusName;
            model.OrderDescription = nd;
            model.Amount = tong;
            model.OrderType = ma;
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }


        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return View(response);
        }



        public IActionResult Index(IQueryCollection collection)
        {
            VnPay vnpay = new VnPay();
            OrderInfo payment = new OrderInfo();
            if (HttpContext.Request.Query.Count > 0)
            {

                string vnpHashSecret = _configuration["Vnpay:vnp_HashSecret"];

                var queryStringParams = HttpContext.Request.Query;

                var vnpayData = Request.QueryString;
                foreach (var param in queryStringParams)
                {
                    if (param.Key.Length > 0 && param.Key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(param.Key, param.Value);
                    }
                }

                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long maMerChantVNPay = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnp_TransactionNo = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                string vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
                string TerminalID = vnpay.GetResponseData("vnp_TmnCode");
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                string bankCode = vnpay.GetResponseData("vnp_BankCode");

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnpHashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        int orderIdpaymant = Int32.Parse(vnpay.GetResponseData("vnp_OrderInfo").Split(":")[1]);
                        payment.OrderDesc = vnpay.GetResponseData("vnp_OrderInfo");
                        payment.PayStatus = vnp_TransactionStatus;
                        payment.Status = vnp_TransactionStatus;
                        payment.BankCode = bankCode;
                        payment.OrderId = orderIdpaymant;
                        payment.Amount = vnp_Amount;
                        payment.PaymentTranId = Int32.Parse(HttpContext.Session.GetString("idUser"));
                       
                        DateTime date;
                        DateTime.TryParseExact(vnpay.GetResponseData("vnp_PayDate"), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out date);
                        payment.CreatedDate = date;
                        ViewBag.StatusPayment = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        Console.WriteLine("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", maMerChantVNPay, orderIdpaymant);

                       
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.StatusPayment = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        Console.WriteLine("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", maMerChantVNPay, vnp_Amount, vnp_ResponseCode);
                    }
                }
                else
                {
                    ViewBag.StatusPayment = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View(payment);
        }
        public IActionResult CreatePaymentUrlVnpay2(PaymentInformationModel model)
        {
            VnPay vnpay = new VnPay();
            var tick = DateTime.Now.Ticks.ToString();
            string vnpUrl = _configuration["Vnpay:vnp_Url"];
            string vnpApi = _configuration["Vnpay:vnp_Api"];
            string vnpTmnCode = _configuration["Vnpay:vnp_TmnCode"];
            string vnpHashSecret = _configuration["Vnpay:vnp_HashSecret"];
            string vnpReturnUrl = _configuration["Vnpay:vnp_Returnurl"];
            vnpay.AddRequestData("vnp_Version", VnPay.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnpTmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            Console.WriteLine(_configuration["vnp_TmnCode"]);
            

            //Add Params of 2.1.0 Version
            //Billing
            string paymentUrl = vnpay.CreateRequestUrl(vnpUrl, vnpHashSecret);
            return Redirect(paymentUrl);

        }

    }
}
