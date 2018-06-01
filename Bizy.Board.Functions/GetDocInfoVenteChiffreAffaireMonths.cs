namespace Bizy.Board.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using OuinneBiseSharp.Enums;

    public static class GetDocInfoVenteChiffreAffaireMonths
    {
        [FunctionName("GetDocInfoVenteChiffreAffaireMonths")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                if (!int.TryParse(req.Query["nbMonths"], out var nbMonths)) throw new ArgumentOutOfRangeException($"{nameof(nbMonths)} n'est pas un nombre entier valide.");

                var service = Utils.GetService(req, log);

                var list = new List<object>();
                for (var i = 0; i < nbMonths; i++)
                {
                    var date = DateTime.Now.AddMonths(i - nbMonths).AddDays(DateTime.Now.Day - 1);
                    var value = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddDays(30), date);
                    var label = date.ToString("MMM");
                    list.Add(new {label, value});
                }
                return new OkObjectResult(list);
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
