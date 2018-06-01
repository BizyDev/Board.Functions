namespace Bizy.Board.Functions
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;
    using OuinneBiseSharp.Extensions;
    using OuinneBiseSharp.Services;

    public class Utils
    {
        public static OuinneBiseSharpService GetService(HttpRequest req, TraceWriter log)
        {
            var requestBody = new StreamReader(req.Body).ReadToEnd();
            if (string.IsNullOrWhiteSpace(requestBody)) throw new ArgumentNullException(nameof(requestBody), "La requête est vide.");
            
            var creds = JsonConvert.DeserializeObject<Credentials>(requestBody);
            
            if (string.IsNullOrWhiteSpace(creds.Company)) throw new ArgumentNullException(nameof(creds.Company), $"{nameof(creds.Company)} est vide.");
            if (string.IsNullOrWhiteSpace(creds.Username)) throw new ArgumentNullException(nameof(creds.Username), $"{nameof(creds.Username)} est vide.");
            if (string.IsNullOrWhiteSpace(creds.Password)) throw new ArgumentNullException(nameof(creds.Password), $"{nameof(creds.Password)} est vide.");
            if (creds.Folder == null || creds.Folder.Value < 1) throw new ArgumentOutOfRangeException(nameof(creds.Folder), $"{nameof(creds.Folder)} n'est pas un nombre entier valide.");
            if (creds.Exercice == null || creds.Exercice.Value < 1) throw new ArgumentOutOfRangeException(nameof(creds.Exercice), $"{nameof(creds.Exercice)} n'est pas un nombre entier valide.");


            return new OuinneBiseSharpService(creds.Company, creds.Username, creds.Password.Encrypt(), creds.Folder.Value, creds.Exercice.Value, Environment.GetEnvironmentVariable("WINBIZ_API_KEY"), "BizyBoard");
        }
    }
}
