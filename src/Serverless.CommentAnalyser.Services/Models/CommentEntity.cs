using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.Services.Models
{
    public class CommentEntity : TableEntity
    {
        public string Content { get; set; }

        public string ToneInformation { get; set; }   
        
    }
}
