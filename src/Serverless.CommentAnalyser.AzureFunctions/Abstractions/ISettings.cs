using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.AzureFunctions.Abstractions
{
    public interface ISettings
    {
        string TextAnalyserUrl { get; }

        string TextAnalyserContentType { get; }

        string TextAnalyserAuthToken { get; }

        string AzureStorageConenectionString { get; }
    }
}
