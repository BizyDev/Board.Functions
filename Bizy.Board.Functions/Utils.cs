namespace Bizy.Board.Functions
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs.Host;
    using OuinneBiseSharp.Services;

    public class Utils
    {
        public static OuinneBiseSharpService GetService(HttpRequest req, TraceWriter log)
        {
            string company = req.Query["company"];
            if (string.IsNullOrWhiteSpace(company)) throw new ArgumentNullException(nameof(company), $"{nameof(company)} est vide.");
            string username = req.Query["username"];
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username), $"{nameof(username)} est vide.");
            string password = req.Query["password"];
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password), $"{nameof(password)} est vide.");
            if (!int.TryParse(req.Query["folder"], out var folder)) throw new ArgumentOutOfRangeException(nameof(folder), $"{nameof(folder)} n'est pas un nombre entier valide.");
            if (!int.TryParse(req.Query["exercice"], out var exercice)) throw new ArgumentOutOfRangeException(nameof(exercice), $"{nameof(exercice)} n'est pas un nombre entier valide.");

            return new OuinneBiseSharpService(company, username, password, folder, exercice, Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");
        }
    }
}
