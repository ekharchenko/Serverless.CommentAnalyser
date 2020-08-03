using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.AzureFunctions.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public JObject Tone { get; set; }
    }
}
