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

    public static class GetDocInfoVenteChiffreAffaireYears
    {
        [FunctionName("GetDocInfoVenteChiffreAffaireYears")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                if (!int.TryParse(req.Query["nbYears"], out var nbYears)) throw new ArgumentOutOfRangeException($"{nameof(nbYears)} n'est pas un nombre entier valide.");

                var service = Utils.GetService(req, log);

                var list = new List<object>();
                for (var i = 0; i < nbYears; i++)
                {
                    var date = new DateTime(DateTime.Now.AddYears(i - nbYears).Year, 1, 1);
                    var result = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddMonths(12).AddDays(30), date);
                    var resultToDate = await service.DocInfo(DocInfoMethodsEnum.VenteChiffreAffaire, date.AddMonths(DateTime.Now.Month).AddDays(DateTime.Now.Day), date);
                    var year = date.Year;

                    list.Add(new { YearToDate = resultToDate.Value, Label = year, Year = result.Value });
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