namespace Bizy.Board.Functions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;

    public static class GetPaymentsCalendar
    {
        [FunctionName("GetPaymentsCalendar")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                var service = Utils.GetService(req, log);

                var res = await service.PaymentsCalendar(9999);

                return new OkObjectResult(new { Amount = res.Value.Sum(r => r.LocalOpenAmount), Docs = res.Value.Sum(r => r.OpenDocuments) });
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