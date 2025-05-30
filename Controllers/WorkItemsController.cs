﻿using System;
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
 /// <summary>
 /// Classe que representa controle do lista de atividade - WorkItems Analytics
 /// </summary>
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
            var pat = "3BjP6NggUWt6gwdMc0F6CKDZmg4IpiCAjzfscH0hl9Y19HIl4iaxJQQJ99BBACAAAAAlfsUKAAASAZDO2Oiv"; // Replace with your PAT - Data da Expiração: 31/07/2025
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{pat}")));
        }

        /// <summary>
        /// Classe que representa a lista de atividades no Azure DevOps (Boards)
        /// Possui filtros de pesquisa
        /// </summary>
        /// <param name="searchId"></param>
        /// <param name="searchDataAbertura"></param>
        /// <param name="searchDataPrevisaoEntrega"></param>
        /// <param name="searchDataFechamento"></param>
        /// <param name="searchDataInicio"></param>
        /// <param name="searchDataConclusao"></param>
        /// <param name="searchDataRealEntrega"></param>
        /// <param name="searchStatus"></param>
        /// <param name="searchPrioridade"></param>
        /// <param name="searchSistema"></param>
        /// <param name="searchTeam"></param>
        /// <param name="team"></param>
        /// <param name="clear"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string searchId, string searchDataAbertura, string searchDataPrevisaoEntrega,  string searchDataFechamento,  string searchDataInicio, string searchDataConclusao, string searchDataRealEntrega,   string searchStatus, string searchPrioridade, string searchSistema, string searchTeam, string team, bool clear = false, int page = 1, int pageSize = 160)
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
                searchTeam = null;
                searchDataPrevisaoEntrega = null;
                searchDataRealEntrega = null;
                ViewBag.SearchId = null;
                ViewBag.SearchDataInicio = null;
                ViewBag.SearchDataConclusao = null;
                ViewBag.SearchStatus = null;
                ViewBag.SearchPrioridade = null;
                ViewBag.searchDataAbertura = null;
                ViewBag.searchDataFechamento = null;
                ViewBag.searchConclusao = null;
                ViewBag.searchSistema = null;
                ViewBag.SearchTeam = null;
                ViewBag.SearchDataPrevisaoEntrega = null;
                ViewBag.SearchDataRealEntrega = null;
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
            //var url = "devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?$filter=(indexof(Custom_Sistema, 'Transporte Escolar') ge 0 or indexof(Custom_Sistema, 'Indicação Escolas PEI') ge 0 or indexof(Custom_Sistema, 'PLACON') ge 0) and WorkItemType eq 'User Story'       &$select=WorkItemId,Title,Custom_Atividade,State,TagNames,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_SemanaProdesp,Custom_EntregaValor,Custom_Cliente,Custom_ClienteProdesp,Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef &$expand=AssignedTo($select=UserName),Teams($select=TeamName),BoardLocations($select=ColumnName,IsDone,BoardName)&$orderby=CreatedDate desc";
            var url = "devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?$filter=WorkItemType eq 'User Story'       &$select=WorkItemId,Title,Custom_Atividade,State,TagNames,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_SemanaProdesp,Custom_EntregaValor,Custom_Cliente,Custom_ClienteProdesp,Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega, Custom_4c82d7ee__002Dbf7c__002D4b3f__002Db22f__002D0f09ef055fcc, Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef &$expand=AssignedTo($select=UserName),Teams($select=TeamName),BoardLocations($select=ColumnName,IsDone,BoardName)&$orderby=CreatedDate desc";

            // Set Personal Access Token (PAT) for authentication
            var pat = "3BjP6NggUWt6gwdMc0F6CKDZmg4IpiCAjzfscH0hl9Y19HIl4iaxJQQJ99BBACAAAAAlfsUKAAASAZDO2Oiv"; // Replace with your PAT em 31/07/2025
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

                if (!string.IsNullOrEmpty(searchDataPrevisaoEntrega) && DateTime.TryParseExact(searchDataPrevisaoEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDataPrevisaoEntrega))
                {
                    workItems = workItems.Where(w => w.Custom_DataPrevistaDaEntrega.HasValue && w.Custom_DataPrevistaDaEntrega.Value.Date == parseDataPrevisaoEntrega.Date).ToList();
                }

                if (!string.IsNullOrEmpty(searchDataConclusao) && DateTime.TryParseExact(searchDataConclusao, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDataConclusao))
                {
                    workItems = workItems.Where(w => w.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.HasValue && w.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.Value.Date == parsedDataConclusao.Date).ToList();
                }

                if (!string.IsNullOrEmpty(searchDataRealEntrega) && DateTime.TryParseExact(searchDataRealEntrega, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parseDateRealEntrega)) 
                {
                    workItems = workItems.Where(w => w.Custom_4c82d7ee__002Dbf7c__002D4b3f__002Db22f__002D0f09ef055fcc.HasValue && w.Custom_4c82d7ee__002Dbf7c__002D4b3f__002Db22f__002D0f09ef055fcc.Value.Date == parseDateRealEntrega.Date).ToList();
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
                    workItems = workItems
                        .Where(w => !string.IsNullOrEmpty(w.Custom_Sistema) && w.Custom_Sistema.Equals(searchSistema, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Novo filtro por Team
                if (!string.IsNullOrEmpty(searchTeam))
                {
                    workItems = workItems.Where(w => w.Teams != null && w.Teams.Any(t => t.TeamName.Equals(searchTeam, StringComparison.OrdinalIgnoreCase))).ToList();
                }

                ViewBag.SearchId = searchId;
                ViewBag.SearchDataInicio = searchDataInicio;
                ViewBag.SearchDataConclusao = searchDataConclusao;
                ViewBag.SearchStatus = searchStatus;
                ViewBag.SearchPrioridade = searchPrioridade;
                ViewBag.SearcgDataAbertura = searchDataAbertura;
                ViewBag.SearchDataFechamento = searchDataFechamento;
                ViewBag.SeachSistema = searchSistema;
                ViewBag.SearchTeam = searchTeam;
                ViewBag.SearchDataPrevisaoEntrega = searchDataPrevisaoEntrega;

                // Inicializa a lista de equipes
                var teams = workItems
                    .SelectMany(w => w.Teams)
                    .Where(t => !string.IsNullOrEmpty(t.TeamName))
                    .Select(t => t.TeamName)
                    .Distinct()
                    .ToList();

                ViewBag.Teams = teams ?? new List<string>(); // Garantir que ViewBag.Teams nunca seja nulo

                //var teams = workItems.SelectMany(w => w.Teams).Where(t => t.TeamName != null).Select(t => t.TeamName).Distinct().ToList();
                //ViewBag.Teams = new SelectList(teams);

                var sistemas = workItems
                    .Where(w => w.Custom_Sistema != null) // Filtra itens onde Custom_Sistema não é nulo
                    .Select(w => w.Custom_Sistema)
                    .Distinct()
                    .ToList();

                // Adiciona uma opção padrão se nenhum sistema foi encontrado
                if (!sistemas.Any())
                {
                    sistemas.Add("Nenhum sistema disponível");
                }

                ViewBag.Sistemas = sistemas ?? new List<string>();
                //ViewBag.Sistemas = new SelectList(sistemas);

                //var sistemas = workItems.Select(w => w.Custom_Sistema ?? "Não Especificado").Distinct().ToList();
                //ViewBag.Sistemas = new SelectList(sistemas);

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
