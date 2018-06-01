namespace Bizy.Board.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using OuinneBiseSharp.Enums;
    using OuinneBiseSharp.Extensions;
    using OuinneBiseSharp.Services;

    public static class GetBoardData
    {
        [FunctionName("GetBoardData")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            var _service = new OuinneBiseSharpService(Environment.GetEnvironmentVariable("WINBIZ_API_COMPANY"), 
                Environment.GetEnvironmentVariable("WINBIZ_API_USERNAME"), 
                Environment.GetEnvironmentVariable("WINBIZ_API_PASSWORD").Encrypt(), 
                2, 
                2018, 
                Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");

            var list = new List<object>();

            for (var i = 0; i < 3; i++)
            {
                var date = new DateTime(DateTime.Now.AddYears(i - 3).Year, 1, 1);
                var result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddMonths(12).AddDays(30), date);
                var resultToDate = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddMonths(DateTime.Now.Month).AddDays(DateTime.Now.Day), date);
                var year = date.Year;

                list.Add(new { result.Value, D = resultToDate.Value, year });
            }

            var list2 = new List<object>();
            for (int i = 0; i < 6; i++)
            {
                var date = DateTime.Now.AddMonths(i - 6).AddDays(DateTime.Now.Day - 1);
                var result = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddDays(30), date);
                var label = date.ToString("MMM");
                list.Add(new { label, result.Value });
            }

            var salesThisYear = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now, new DateTime(DateTime.Now.Year, 1, 1));
            var salesPastYear = await _service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now.AddYears(-1), new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1));

            var res = await _service.PendingPayments(9999);


            var res2 = await _service.PaymentsCalendar(9999);

            var res3 = await _service.AddressesPendingPayments(9999);
            var values = res3.Value.GroupBy(r => r.AddressId)
                .OrderByDescending(r => r.Sum(o => o.LocalOpenAmount))
                .Select(r => new {r.FirstOrDefault()?.Address, Amount = r.Sum(o => o.LocalOpenAmount), Count = r.Sum(o => o.OpenDocuments)}).Take(10 == 0 ? 99999 : 10);

            return new OkObjectResult(new
            {
                list, list2, salesThisYear = salesThisYear.Value, salesPastYear = salesPastYear.Value, PendingPayments = res.Value, PaymentsCalendar = res2.Value, AddressesPendingPayments = values
            });
        }
    }
}
