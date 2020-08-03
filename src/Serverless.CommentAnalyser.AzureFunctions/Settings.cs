using Serverless.CommentAnalyser.AzureFunctions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.AzureFunctions
{
    public class Settings : ISettings
    {
        public string TextAnalyserUrl => Environment.GetEnvironmentVariable("WatsonTextAnalyserUrl");

        public string TextAnalyserContentType => Environment.GetEnvironmentVariable("WatsonTextAnalyserContentType");

        public string TextAnalyserAuthToken => Environment.GetEnvironmentVariable("WatsonTextAnalyserAuthToken");

        public string AzureStorageConenectionString => Environment.GetEnvironmentVariable("AzureWebJobsStorage");

    }
}
