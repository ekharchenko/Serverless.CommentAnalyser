using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.AzureFunctions.Models
{
    public class ValidationErrorMessage
    {
        public List<string> Errors { get; set; }

        public ValidationErrorMessage()
        {
            Errors = new List<string>();
        }
    }
}
