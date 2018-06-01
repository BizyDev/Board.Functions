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

    public static class GetSalesThisAndPastYear
    {
        [FunctionName("GetSalesThisAndPastYear")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                var service = Utils.GetService(req, log);

                var salesThisYear = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now, new DateTime(DateTime.Now.Year, 1, 1));
                var salesPastYear = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, DateTime.Now.AddYears(-1), new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1));

                return new OkObjectResult(new { salesThisYear, salesPastYear });
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
