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

    public static class GetBadPayersList
    {
        [FunctionName("GetBadPayersList")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                if (!int.TryParse(req.Query["nb"], out var nb)) throw new ArgumentOutOfRangeException($"{nameof(nb)} n'est pas un nombre entier valide.");

                var service = Utils.GetService(req, log);

                var res = await service.AddressesPendingPayments(9999);
                var values = res.Value.GroupBy(r => r.AddressId)
                    .OrderByDescending(r => r.Sum(o => o.LocalOpenAmount))
                    .Select(r => new {r.FirstOrDefault()?.Address, Amount = r.Sum(o => o.LocalOpenAmount), Count = r.Sum(o => o.OpenDocuments)}).Take(nb == 0 ? 99999 : nb);
                return new OkObjectResult(values);
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