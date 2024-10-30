using GestaoDemandas.Controllers;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static NPOI.HSSF.Util.HSSFColor;

namespace GestaoDemandas.Models
{
    public class PipelineRun
    {
        public string RunNumber { get; set; }
        public int PipelineRunId { get; set; }

        private string _startedDateSK;
        [JsonProperty("StartedDateSK")]
        public string StartedDateSK
        {
            get => _startedDateSK;
            set
            {
                _startedDateSK = value;
                if (DateTime.TryParse(value, out DateTime dateValue))
                {
                    ParsedStartedDateSK = dateValue;
                }
                else
                {
                    ParsedStartedDateSK = null; // Handle invalid date
                }
            }
        }

        public DateTime? ParsedStartedDateSK { get; private set; } // Parsed date value

        public DateTime? CompletedDate { get; set; }
        public int SucceededCount { get; set; }
        public string QueueDurationSeconds { get; set; }
        public Pipeline pipeline { get; set; }
        public Project project { get; set; }
        public Branch BranchTo { get; set; }
        
        public class Pipeline
        {
            public int PipelineSK { get; set; }
            public string PipelineId { get; set; }
            public string PipelineName { get; set; }
        }

        public class Project
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
        }

        public class Branch
        {
            public string RepositoryId { get; set; }
            public string BranchName { get; set; }
            public DateTime? AnalyticsUpdatedDate { get; set; }
        }

    }
}