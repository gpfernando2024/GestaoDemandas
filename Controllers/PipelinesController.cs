using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Util;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using GestaoDemandas.Models;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace GestaoDemandas.Controllers
{
    public class PipelinesController : Controller
    {
        private static readonly HttpClient httpClient;

        static PipelinesController()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://analytics.dev.azure.com/")
            };

            // Add Personal Access Token (PAT) for authentication
            var pat = "wucpcjzts6okeheohrgfdntvtf5pfyism6rbrlmtdxlbeldzb7za"; // Replace with your PAT
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{pat}")));
        }

        public async Task<ActionResult> Index(string SearchipelineSK, string SearchRunNumber, int page = 1, int pageSize = 160, bool clear = false)
        {
            var url = "https://analytics.dev.azure.com/devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v4.0-preview/PipelineRuns?%20&$select=PipelineRunId,StartedDateSK,CompletedDate,RunNumber,RunReason,QueuedDate,SucceededCount,QueueDurationSeconds%20&$expand=Pipeline($select=PipelineSK,PipelineId,PipelineName),Project($select=ProjectId,ProjectName),Branch($select=RepositoryId,BranchName,AnalyticsUpdatedDate)&$orderby=PipelineRunId%20desc";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(response.StatusCode, response.ReasonPhrase);
            }
            var responseData = await response.Content.ReadAsStringAsync();
            try
            {
                //return new HttpStatusCodeResult(response.StatusCode, response.ReasonPhrase);

                // Create an anonymous object for deserialization
                var responseObj = new { value = new List<PipelineRun>() };
                responseObj = JsonConvert.DeserializeAnonymousType(responseData, responseObj);
                
                // Deserialize the JSON to a list of PipelineRun objects
                var pipelineRuns = responseObj.value;

                if (clear)
                {
                    // Clear search parameters and return the initial list of Pipelines
                    SearchipelineSK = null;
                    SearchRunNumber = null;
                    ViewBag.SearchRunNumber = null;
                    ViewBag.SearchipelineSK = null;

                }
                // Filtrar workItems com base nos critérios de pesquisa
                if (!string.IsNullOrEmpty(SearchipelineSK))
                {
                    pipelineRuns = pipelineRuns.Where(w => w.pipeline.PipelineSK.ToString().Contains(SearchipelineSK)).ToList();
                }

                if (!string.IsNullOrEmpty(SearchRunNumber))
                {
                    pipelineRuns = pipelineRuns.Where(w => w.RunNumber.ToString().Contains(SearchRunNumber)).ToList();
                }

                // Pagination logic
                var totalItems = pipelineRuns.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var itemsToDisplay = pipelineRuns.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                ViewBag.SearchRunNumber = SearchRunNumber;
                ViewBag.SearchipelineSK = SearchipelineSK;

                return View(pipelineRuns.Take(10));

            }
            catch (JsonSerializationException ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
                return View("Error"); // Assuming you have an "Error" view to display errors
            }
        }
    }

}
