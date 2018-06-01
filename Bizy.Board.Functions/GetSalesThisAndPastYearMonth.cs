namespace Bizy.Board.Functions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using OuinneBiseSharp.Enums;

    public static class GetSalesThisAndPastYearMonth
    {
        [FunctionName("GetSalesThisAndPastYearMonth")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                var service = Utils.GetService(req, log);

                var salesThisMonth = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                var salesPastYearMonth = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire,
                    new DateTime(DateTime.Now.AddYears(-1).Year, DateTime.Now.Month, 31),
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddYears(-1));

                return new OkObjectResult(new { salesThisMonth, salesPastYearMonth });
            }
            catch (ArgumentNullException ex)
            {
                log.Error("ArgumentNullException", ex);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                log.Error("ArgumentOutOfRangeException", ex);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("Erreur", ex);
                return new BadRequestObjectResult("Erreur inattendue");
            }
        }
    }
}