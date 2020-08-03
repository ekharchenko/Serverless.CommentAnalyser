using Microsoft.Azure.Cosmos.Table;
using Serverless.CommentAnalyser.Services.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serverless.CommentAnalyser.Services.Models
{
    public class JobEntity : TableEntity
    {
        public string Content { get; set; }

        public int TargetId { get; set; }

        public int JobStatusCode { get; set; }

        // I know it looks odd, but azure tables ignore enum types when you save the data. I think there should be some attributes to include an enum field, but i just use a simple work around. :)
        public JobStatus JobStatus { get { return (JobStatus)JobStatusCode; } set { JobStatusCode = (int) value; } }
    }
}
