using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using GestaoDemandas.Models;
using Newtonsoft.Json;
using static NPOI.SS.Formula.Functions.Countif;
/*
Campos DevOps Analytics
    Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a = Data Real da Homologação
    Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c = GerênciaProdesp
    Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e = DescriçãoProjeto (Descrição do Projeto, relacionada a sessão Gestão Prodesp)
    Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d = DataAbertura
    Custom_DataInicioPrevisto = DataInicioPrevisto
    Custom_DataPrevistaDaEntrega = DataPrevistaDaEntrega
    Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9 = Entrega Estratégica
    Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef = DataFechamento
    Custom_EntregaValor = EntregaValor (Entrega de valor para a sessão Gestão Prodesp)
    Custom_Finalidade = Finalidade
    Custom_NomeProjeto = NomeProjeto
    Custom_Prioridade_Epic = Prioridade_Epic
    Custom_SemanaProdesp = SemanaProdesp
    Custom_Sistema = Sistema
    Custom_DataInicioAtendimento = DataInicioAtendimento
    Custom_ClienteProdesp = CoordenadoriaProdesp
    Title = Title
    WorkItemType = Work Item Type
    CreatedDate = Created Date 
    State = State
    WorkItemId = WorkItemId
    Risk = Risk
    Custom_DataInicioPrevisto = DataInicioPrevisto
    Custom_GerenteProjeto = GerenteProjeto
*/

namespace GestaoDemandas.Controllers
{
    public class WorkItemsController : Controller
    {
        private static readonly HttpClient httpClient;

        static WorkItemsController()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://analytics.dev.azure.com/")
            };

            // Add Personal Access Token (PAT) for authentication
            var pat = "m7z3rlvo5kqaet4yrw7g2am5bp6rxu6optb77vf5x7gqxrw6tb3a"; // Replace with your PAT - Data da Expiração: 31/12/2024
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{pat}")));
        }

        public async Task<ActionResult> Index(string searchId, string searchDataAbertura, string searchDataFechamento,  string searchDataInicio, string searchDataConclusao, string searchStatus, string searchPrioridade, string searchSistema, bool clear = false, int page = 1, int pageSize = 25)
        {
            if (clear)
            {
                // Clear search parameters and return the initial list of work items
                searchId = null;
                searchDataInicio = null;
                searchDataConclusao = null;
                searchStatus = null;
                searchPrioridade = null;
                searchDataAbertura = null;
                searchDataFechamento = null;
                searchSistema = null;
                ViewBag.SearchId = null;
                ViewBag.SearchDataInicio = null;
                ViewBag.SearchDataConclusao = null;
                ViewBag.SearchStatus = null;
                ViewBag.SearchPrioridade = null;
                ViewBag.searchDataAbertura = null;
                ViewBag.searchDataFechamento = null;
                ViewBag.searchSistema = null;
            }
            /*
               WorkItemId,
               Title,
               State,
               Custom_Prioridade_Epic,
               Custom_Finalidade,
               Custom_NomeProjeto,
               Custom_GerenteProjeto,
               CreatedDate,
               Custom_DataInicioAtendimento,
               Custom_DataPrevistaDaEntrega,
               Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d Custom_DataAbertura, 
               Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef Custom_DataFechamento
               
               Comment
               Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e

             */
            var url = "devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?\r\n        $filter=(indexof(Custom_Sistema, 'Transporte Escolar') ge 0 or indexof(Custom_Sistema, 'Indicação Escolas PEI') ge 0 or indexof(Custom_Sistema, 'PLACON') ge 0) and WorkItemType eq 'User Story'\r\n        &$select=WorkItemId,Title,Custom_Atividade,State,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_SemanaProdesp,Custom_EntregaValor,Custom_Cliente,Custom_ClienteProdesp,Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef\r\n&$orderby=CreatedDate desc";
            
            //var url = "devopssee/CFIEE%20-%20Coordenadoria%20de%20Finanças%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?$filter=WorkItemType%20eq%20'User Story'%20and%20Custom_Sistema%20eq%20'Transporte%20Escolar'%20and%20Custom_Sistema%20eq%20'Indicação%20Escolas%20PEI'&$select=WorkItemId,Title,State,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_GerenteProjeto,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e, CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d, Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef&$orderby=CreatedDate%20desc";
            //var url = "devopssee/CFIEE%20-%20Coordenadoria%20de%20Finanças%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?$filter=WorkItemType%20eq%20'User Story'%20and%20 &$select=WorkItemId,Title,State,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_GerenteProjeto,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e, CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef&$orderby=CreatedDate%20desc";

            // Set Personal Access Token (PAT) for authentication
            var pat = "m7z3rlvo5kqaet4yrw7g2am5bp6rxu6optb77vf5x7gqxrw6tb3a"; // Replace with your PAT
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{pat}")));

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(response.StatusCode, response.ReasonPhrase);
            }
            try
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Criar um objeto anônimo para desserialização
                var responseObj = new { value = new List<WorkItem>() };
                responseObj = JsonConvert.DeserializeAnonymousType(responseData, responseObj);

                // Desserializar o JSON para uma lista de objetos WorkItem
                var workItems = responseObj.value;

                // Filtrar workItems com base nos critérios de pesquisa
                if (!string.IsNullOrEmpty(searchId))
                {
                    workItems = workItems.Where(w => w.WorkItemId.ToString().Contains(searchId)).ToList();
                }

                if (!string.IsNullOrEmpty(searchDataInicio) && DateTime.TryParseExact(searchDataInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDataInicio))
                {
                    workItems = workItems.Where(w => w.Custom_DataInicioAtendimento.HasValue && w.Custom_DataInicioAtendimento.Value.Date == parsedDataInicio.Date).ToList();
                }

                if (!string.IsNullOrEmpty(searchDataFechamento) && DateTime.TryParseExact(searchDataFechamento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDataFechamento))
                {
                    workItems = workItems.Where(w => w.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.HasValue && w.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.Value.Date == parsedDataFechamento.Date).ToList();
                }

                if (!string.IsNullOrEmpty(searchStatus))
                {
                    workItems = workItems.Where(w => w.State.Equals(searchStatus, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(searchPrioridade))
                {
                    workItems = workItems.Where(w => w.Custom_Prioridade_Epic.Equals(searchPrioridade, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(searchSistema))
                {
                    workItems = workItems.Where(w => w.Custom_Sistema.Equals(searchSistema, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                ViewBag.SearchId = searchId;
                ViewBag.SearchDataInicio = searchDataInicio;
                ViewBag.SearchDataConclusao = searchDataConclusao;
                ViewBag.SearchStatus = searchStatus;
                ViewBag.SearchPrioridade = searchPrioridade;
                ViewBag.SearcgDataAbertura = searchDataAbertura;
                ViewBag.SearchDataFechamento = searchDataFechamento;
                ViewBag.SeachSistema = searchStatus;

                var totalItems = workItems.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var itemsToDisplay = workItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                return View(itemsToDisplay);

                var jsonFilePath = Server.MapPath("~/Content/workitems.json");
                System.IO.File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(workItems, Formatting.Indented));

                return View(workItems.Take(10));
            }
            catch (JsonSerializationException ex)
            {
                // Log the exception or handle it appropriately
                // For example, you could return an error view or redirect to an error page
                Console.WriteLine("Erro ao desserializar JSON: " + ex.Message);
                return View("Error"); // Assuming you have an "Error" view to display errors
            }
        }
 

        /*
        public ActionResult Download()
        {
            var jsonFilePath = Server.MapPath("~/Content/workitems.json");
            var jsonContent = System.IO.File.ReadAllBytes(jsonFilePath);
            return File(jsonContent, "application/json", "workitems.json");
        }
        */
    }
}
