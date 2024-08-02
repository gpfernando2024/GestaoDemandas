using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.XWPF.UserModel;
using GestaoDemandas.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using NPOI.Util;
using System.Linq;
using DocumentFormat.OpenXml.EMMA;
using NPOI.SS.Formula;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.Model;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using NPOI.SS.Formula.Functions;
using System.Drawing;
using GestaoDemandas.Enumeradores;
using System.Diagnostics;
using System.Web.Services.Description;

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

    URL:https://analytics.dev.azure.com/devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?
        $filter=(indexof(Custom_Sistema, 'Transporte Escolar') ge 0 or indexof(Custom_Sistema, 'Indicação Escolas PEI') ge 0 or indexof(Custom_Sistema, 'PLACON') ge 0) and WorkItemType eq 'User Story'
        &$select=WorkItemId,Title,State,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_SemanaProdesp,Custom_EntregaValor,Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef
        &$orderby=CreatedDate desc

 */

namespace GestaoDemandas.Controllers
{
      
    public class DailyReportController : Controller
    {
        private static readonly string AzureAnalyticsUrl = "https://analytics.dev.azure.com/devopssee/CFIEE%20-%20Coordenadoria%20de%20Finan%C3%A7as%20e%20Infra%20Estrutura%20Escolar/_odata/v3.0-preview/WorkItems?\r\n        $filter=(indexof(Custom_Sistema, 'Transporte Escolar') ge 0 or indexof(Custom_Sistema, 'Indicação Escolas PEI') ge 0 or indexof(Custom_Sistema, 'PLACON') ge 0) and WorkItemType eq 'User Story'\r\n        &$select=WorkItemId,Custom_Atividade,Title,State,Custom_Sistema,Custom_Prioridade_Epic,Custom_Finalidade,Custom_NomeProjeto,Custom_SemanaProdesp,Custom_EntregaValor,Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,CreatedDate,Custom_DataInicioAtendimento,Custom_DataPrevistaDaEntrega,Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d,Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef\r\n&$orderby=CreatedDate desc";

        // Token de autorização (substitua com seu token real - Data da Expiração: 31/12/2024)
        private static readonly string AuthToken = "noroimmyoi74rv7ev6m3yao5an4smxjsxqu6kohrfs7j2esmjxcq";
        private SituacaoAtividade Status;

        public async Task<ActionResult> StatusReport()
        {
            var model = await GetDailyReportData();
            return View(model);
        }

        public async Task<ActionResult> GenerateDocx(DailyReportViewModel model)
        {
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            var fileName = $"ReportDiario-{currentDate}.docx";

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Crie um novo documento DOCX
                    XWPFDocument doc = new XWPFDocument();

                    // Cria o cabeçalho ocupando toda a largura da página
                    CreateHeader(doc, currentDate);

                    // Título do documento
                    XWPFParagraph titleParagraph = doc.CreateParagraph();
                    titleParagraph.Alignment = ParagraphAlignment.RIGHT;
                    titleParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    titleParagraph.SpacingAfterLines = 1;
                    XWPFRun titleRun = titleParagraph.CreateRun();
                    //titleRun.SetText("Revisão Diária");
                    titleRun.IsBold = false;
                    titleRun.FontSize = 22;  // Reduzido de 11 para 9

                    // Subtítulo do documento
                    XWPFParagraph subtitleParagraph = doc.CreateParagraph();
                    subtitleParagraph.Alignment = ParagraphAlignment.RIGHT;
                    subtitleParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    subtitleParagraph.SpacingAfterLines = 1;
                    XWPFRun subtitleRun = subtitleParagraph.CreateRun();
                    subtitleRun.SetText("Projeto SEDUC");
                    subtitleRun.IsBold = false;
                    subtitleRun.FontSize = 16;  // Reduzido de 16 para 12

                    // Obtém os dados do relatório diário
                    model = await GetDailyReportData();

                    // Histórico de Revisão
                    AddRevisionHistory(doc, model.RevisionHistory);

                    // Salto de página
                    XWPFParagraph pageBreakParagraph = doc.CreateParagraph();
                    XWPFRun pageBreakRun = pageBreakParagraph.CreateRun();
                    pageBreakRun.AddBreak(BreakType.PAGE);

                    // Eventos / Entregas
                    AddEventsDeliveries(doc, model.EventsDeliveries);

                    // Projetos em Andamento
                    AddOngoingProjects(doc, model.OngoingProjects);

                    // Adicionar a observação
                    if (!string.IsNullOrEmpty(model.Observacao))
                    {
                        XWPFParagraph observacaoParagraph = doc.CreateParagraph();
                        observacaoParagraph.Alignment = ParagraphAlignment.LEFT;
                        XWPFRun observacaoRun = observacaoParagraph.CreateRun();
                        observacaoRun.SetText("Observação: " + model.Observacao);
                        observacaoRun.IsBold = true;
                        observacaoRun.FontSize = 12;
                    }

                    // Projetos Suspenso e Abortados
                    AddSuspendProjects(doc, model.projectSuspendItems);

                    // Escreve o documento no stream de memória
                    doc.Write(ms);
                    byte[] fileBytes = ms.ToArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
                }
            }
            catch (Exception ex)
            {
                // Lidar com exceções aqui, se necessário
                throw new ApplicationException("Falha ao gerar relatório DOCX.", ex);
            }
        }


        private async Task<DailyReportViewModel> GetDailyReportData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Configurando o cabeçalho de autorização
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{AuthToken}")));

                    HttpResponseMessage response = await client.GetAsync(AzureAnalyticsUrl);

                    // Verificar se a resposta foi bem-sucedida
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Caso seja uma resposta 401 Unauthorized, lidar com isso aqui
                            throw new ApplicationException("Unauthorized access to Azure Analytics. Check authorization token and permissions.");
                        }
                        else
                        {
                            // Lidar com outros códigos de status de erro aqui, se necessário
                            throw new ApplicationException($"Error fetching data: {response.StatusCode}");
                        }
                    }

                    // Ler o corpo da resposta
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject jsonData = JObject.Parse(responseBody);

                    // Criar um objeto anônimo para desserialização
                    var responseObj = new { value = new List<WorkItem>() };
                    // Desserializar o JSON para uma lista de objetos WorkItem
                   
                    responseObj = JsonConvert.DeserializeAnonymousType(responseBody, responseObj);
                    var workItems = responseObj.value;

                    // Construir o modelo de visualização com os dados obtidos
                    var model = new DailyReportViewModel
                    {
                        ReportTitle = "Revisão Diária",
                        ProjectTitle = "Projeto SEDUC",
                        RevisionHistory = new List<RevisionHistoryItem>
                        {
                            new RevisionHistoryItem
                            {
                                Data = new DateTime(2022, 07, 05),
                                Versao = "1",
                                Descricao = "Acompanhamento Executivo projetos SEDUC",
                                Autor = "Valdemir A. Ereno Junior"
                            }
                        },
                        EventsDeliveries = ParseEventDeliveryItems(workItems),
                        OngoingProjects = ParseProjectItems(workItems),
                        projectSuspendItems = ParseProjectSuspendItems(workItems)
                    };

                    return model;
                }
            }
            catch (HttpRequestException ex)
            {
                // Logar a exceção ou lidar com ela apropriadamente
                throw new ApplicationException("Failed to retrieve data from Azure Analytics. Check network connection or try again later.", ex);
            }
            catch (Exception ex)
            {
                // Lidar com outras exceções aqui, se necessário
                throw new ApplicationException("An error occurred while fetching data from Azure Analytics.", ex);
            }
        }

        private List<EventDeliveryItem> ParseEventDeliveryItems(List<WorkItem> workItems)
        {
            var items = new List<EventDeliveryItem>();
            foreach (var item in workItems)
            {
                string state = item.State;
                string customSistema = item.Custom_Sistema;

                // Verificar se o estado é diferente de "Concluido"
                if (state == "Concluido" &&
                    (customSistema == "Transporte Escolar" || customSistema == "Indicação de Escolas PEI" || customSistema == "PLACON"))
                {
                    var eventItem = new EventDeliveryItem
                    {
                        Custom_Sistema = customSistema,
                        WorkItemId = item.WorkItemId,
                        Title = item.Title,
                        DataAbertura = GetNullableDateTime(item, "Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d"),
                        DataInicioAtendimento = GetNullableDateTime(item, "Custom_DataInicioAtendimento"),
                        DataPrevistaEntrega = GetNullableDateTime(item, "Custom_DataPrevistaDaEntrega"),
                        Status = item.State,
                        //Observacao = item.Custom_Finalidade,
                        //Observacao = ObservacaoHelper.ObterObservacao(Status),
                        Conclusao = GetNullableDateTime(item, "Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef"), // Data Fechamento
                        DescriçãoProjeto = item.Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,
                        GerênciaProdesp = item.Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c,
                        EntregaEstratégica = item.Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,
                        SemanaProdesp = item.Custom_SemanaProdesp,
                        NomeProjeto = item.Custom_NomeProjeto,
                        DataRealHomologação = GetNullableDateTime(item, "Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a"), // Data Real da Homologação
                    };

                    // Convertendo datas com tratamento para evitar exceções
                    if (item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d != null)
                    {
                        eventItem.DataAbertura = item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d;
                    }

                    if (item.Custom_DataInicioAtendimento != null)
                    {
                        eventItem.DataInicioAtendimento = item.Custom_DataInicioAtendimento.Value;
                    }

                    if (item.Custom_DataPrevistaDaEntrega != null)
                    {
                        eventItem.DataPrevistaEntrega = item.Custom_DataPrevistaDaEntrega.Value;
                    }

                    // Verificar se a data de conclusão é igual à data atual
                    
                    if (eventItem.Conclusao.HasValue)
                    {
                        DateTime conclusaoData;
                        bool isValidFormat = DateTime.TryParseExact(eventItem.Conclusao.Value.ToString("dd/MM/yyyy"),
                                                                    "dd/MM/yyyy",
                                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                                    System.Globalization.DateTimeStyles.None,
                                                                    out conclusaoData);

                        if (isValidFormat && conclusaoData.Date == DateTime.Today)
                        {
                            items.Add(eventItem);
                        }
                    }
                    
                }
            }
            return items;
        }


        // Método para obter DateTime? com verificação de nulo


        private List<ProjectItem> ParseProjectItems(List<WorkItem> workItems)
        {
            var items = new List<ProjectItem>();
            foreach (var item in workItems)
            {
                string status = item.State;
                string customSistema = item.Custom_Sistema;

                // Verificar se o estado está entre os permitidos
                if (status == "Desenvolvimento" ||
                    status == "Aberto" ||
                    status == "Suspenso" ||
                    status == "Suspenso - Temp" ||
                    status == "Suspenso-Temp" ||
                    status == "Análise" ||
                    status == "Deploy Producao" ||
                    status == "Aguardando Solicitante" &&
                    //(customSistema == "Transporte Escolar" || customSistema == "Indicação de Escolas PEI" || customSistema == "PLACON"))
                    (customSistema == "Transporte Escolar"))
                {
                    var projectItem = new ProjectItem
                    {
                        Custom_Sistema = customSistema,
                        WorkItemId = item.WorkItemId,
                        Title = item.Title,
                        DataAbertura = GetNullableDateTime(item, "Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d"),
                        DataInicioAtendimento = GetNullableDateTime(item, "Custom_DataInicioAtendimento"),
                        DataPrevistaEntrega = GetNullableDateTime(item, "Custom_DataPrevistaDaEntrega"),
                        Status = status,
                        //Observacao = ObservacaoHelper.ObterObservacaoAndamento(Status),
                        Conclusao = GetNullableDateTime(item, "Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef"), // Data Fechamento
                        DescriçãoProjeto = item.Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,
                        GerênciaProdesp = item.Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c,
                        EntregaEstratégica = item.Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,
                        SemanaProdesp = item.Custom_SemanaProdesp,
                        NomeProjeto = item.Custom_NomeProjeto,
                        DataRealHomologação = GetNullableDateTime(item, "Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a"), // Data Real da Homologação
                        
                    };

                    // Convertendo datas com tratamento para evitar exceções
                    if (item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d != null)
                    {
                        // Data de Abertura
                        projectItem.DataAbertura = item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d;
                    }

                    if (item.Custom_DataInicioAtendimento != null)
                    {
                        projectItem.DataInicioAtendimento = item.Custom_DataInicioAtendimento;
                    }

                    if (item.Custom_DataPrevistaDaEntrega != null)
                    {
                        projectItem.DataPrevistaEntrega = item.Custom_DataPrevistaDaEntrega;
                    }

                    if (item.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef != null)
                    {
                        projectItem.Conclusao = item.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef;
                    }

                    items.Add(projectItem);
                }
            }
            // Ordenar os projetos pelo sistema e pela data de abertura
            var orderedItems = items
                .OrderBy(p => p.Custom_Sistema) // Ordena pelo sistema (ordem alfabética)
                .ThenBy(p => p.DataAbertura)   // Em seguida, ordena pela data de abertura
                .ToList();
            return items;
        }

        private List<ProjectSuspendItem> ParseProjectSuspendItems(List<WorkItem> workItems)
        {
            var items = new List<ProjectSuspendItem>();
            foreach (var item in workItems)
            {
                string status = item.State;
                string customSistema = item.Custom_Sistema;

                // Verificar se o estado está entre os permitidos
                if (status == "Suspenso" ||
                    status == "Suspenso-Temp" &&
                    //(customSistema == "Transporte Escolar" || customSistema == "Indicação de Escolas PEI" || customSistema == "PLACON"))
                    (customSistema == "Indicação Escolas PEI" || customSistema == "PLACON"))
                {
                    var projectSuspendItem = new ProjectSuspendItem
                    {
                        Custom_Sistema = customSistema,
                        WorkItemId = item.WorkItemId,
                        Title = item.Title,
                        DataAbertura = GetNullableDateTime(item, "Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d"),
                        DataInicioAtendimento = GetNullableDateTime(item, "Custom_DataInicioAtendimento"),
                        DataPrevistaEntrega = GetNullableDateTime(item, "Custom_DataPrevistaDaEntrega"),
                        Status = status,
                        //Observacao = ObservacaoHelper.ObterObservacao(Status),
                        Conclusao = GetNullableDateTime(item, "Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef"), // Data Fechamento
                        DescriçãoProjeto = item.Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e,
                        GerênciaProdesp = item.Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c,
                        EntregaEstratégica = item.Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9,
                        SemanaProdesp = item.Custom_SemanaProdesp,
                        NomeProjeto = item.Custom_NomeProjeto,
                        DataRealHomologação = GetNullableDateTime(item, "Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a"), // Data Real da Homologação
                    };

                    // Convertendo datas com tratamento para evitar exceções
                    if (item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d != null)
                    {
                        // Data de Abertura
                        projectSuspendItem.DataAbertura = item.Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d;
                    }

                    if (item.Custom_DataInicioAtendimento != null)
                    {
                        projectSuspendItem.DataInicioAtendimento = item.Custom_DataInicioAtendimento;
                    }

                    if (item.Custom_DataPrevistaDaEntrega != null)
                    {
                        projectSuspendItem.DataPrevistaEntrega = item.Custom_DataPrevistaDaEntrega;
                    }

                    if (item.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef != null)
                    {
                        projectSuspendItem.Conclusao = item.Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef;
                    }

                    items.Add(projectSuspendItem);

                }

            }

            return items;
        }

        // Ajuste a função GetNullableDateTime para aceitar um WorkItem
        private DateTime? GetNullableDateTime(WorkItem item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return (DateTime?)property.GetValue(item);
            }
            return null;
        }

        private void SetCellText(XWPFTableCell cell, string text, int fontSize, string bgColor = null, ParagraphAlignment alignment = ParagraphAlignment.LEFT)
        {
            XWPFParagraph paragraph = cell.Paragraphs[0];
            paragraph.Alignment = alignment; // Definindo o alinhamento do parágrafo
            XWPFRun run = paragraph.CreateRun();
            run.SetText(text);
            run.FontSize = fontSize;
            run.SetFontFamily("Verdana",FontCharRange.None );

            if (bgColor != null)
            {
                cell.SetColor(bgColor);
            }
        }

        private void SetCellImage(XWPFTableCell cell, string imagePath)
        {
            XWPFParagraph p = cell.Paragraphs.Count > 0 ? cell.Paragraphs[0] : cell.AddParagraph();
            XWPFRun r = p.CreateRun();

            try
            {
                using (FileStream imgStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    r.AddPicture(imgStream, (int)PictureType.JPEG, imagePath, Units.ToEMU(50), Units.ToEMU(50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar imagem: {ex.Message}");
            }
        }

        private void CreateHeader(XWPFDocument doc, string currentDate)
        {
            // Cria uma seção de propriedades se não existir
            CT_SectPr sectPr = doc.Document.body.sectPr;
            if (sectPr == null)
            {
                sectPr = new CT_SectPr();
                doc.Document.body.sectPr = sectPr;
            }

            // Configura a seção para ter um cabeçalho em todas as páginas
            XWPFHeaderFooterPolicy headerFooterPolicy = new XWPFHeaderFooterPolicy(doc, sectPr);

            // Criação do cabeçalho
            XWPFHeader header = headerFooterPolicy.CreateHeader(XWPFHeaderFooterPolicy.DEFAULT);

            // Calcula a largura da página A4 em twips no sentido retrato
            long pageWidthTwips = Units.ToEMU(210);  // Largura da página A4 em mm
            pageWidthTwips = Units.PixelToEMU((int)(pageWidthTwips * 96 / 25.4));  // Convertendo de mm para pixels e depois para EMUs

            // Criação da tabela no cabeçalho com 1 linha e 3 colunas
            XWPFTable headerTable = header.CreateTable(1, 3);

            // Define a largura da tabela para 100% da largura da página em twips
            headerTable.Width = (int)pageWidthTwips;
            headerTable.SetColumnWidth(0, (ulong)(pageWidthTwips / 3));
            headerTable.SetColumnWidth(1, (ulong)(pageWidthTwips / 3));
            headerTable.SetColumnWidth(2, (ulong)(pageWidthTwips / 3));

            // Adiciona a imagem à célula do cabeçalho central
            XWPFTableCell cell = headerTable.GetRow(0).GetCell(1); // A célula central da tabela
            XWPFParagraph paragraph = cell.Paragraphs[0];
            paragraph.Alignment = ParagraphAlignment.CENTER;

            XWPFTableRow headerRow = headerTable.GetRow(0);

            // Adiciona a primeira imagem (usando recursos incorporados)
            using (MemoryStream ms = new MemoryStream())
            {
                Properties.Resources.Novo_Logo_Prodesp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                AddImageToCell(doc, headerRow.GetCell(0), ms.ToArray(), 150, 50);
            }

            // Adiciona o título "Revisão Diária - data atual" centralizado na célula
            XWPFParagraph titleParagraph = headerRow.GetCell(1).AddParagraph();
            titleParagraph.Alignment = ParagraphAlignment.CENTER;
            XWPFRun titleRun = titleParagraph.CreateRun();
            titleRun.SetText($"Revisão Diária - {currentDate}");
            titleRun.IsBold = false;
            titleRun.FontSize = 9;  // Reduzido de 11 para 9 

            // Adiciona a segunda imagem (usando recursos incorporados)
            using (MemoryStream ms = new MemoryStream())
            {
                Properties.Resources.Novo_Logo_SEDUC.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                AddImageToCell(doc, headerRow.GetCell(2), ms.ToArray(), 150, 50);
            }
        }

        private void AddImageToCell(XWPFDocument doc, XWPFTableCell cell, byte[] imageBytes, int width, int height)
        {
            XWPFParagraph paragraph = cell.Paragraphs[0];
            XWPFRun run = paragraph.CreateRun();
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                run.AddPicture(ms, (int)PictureType.JPEG, "image.jpg", Units.ToEMU(width), Units.ToEMU(height));
            }
        }


        private void AddRevisionHistory(XWPFDocument doc, List<RevisionHistoryItem> revisionHistory)
        {
            var paragraph = doc.CreateParagraph();
            XWPFRun run = paragraph.CreateRun();
            paragraph.SpacingBetween = 1; // Espaçamento simples entre linhas
            paragraph.SpacingAfterLines = 1;

            run.SetText("Histórico de Revisões");
            run.FontSize = 16;
            run.SetFontFamily("Verdana", FontCharRange.None);
            run.IsBold = true;

            var table = doc.CreateTable(revisionHistory.Count + 1, 4);

            SetCellText(table.GetRow(0).GetCell(0), "Data", 11, "#B8CCE4", ParagraphAlignment.CENTER); // Cor azul claro
            SetCellText(table.GetRow(0).GetCell(1), "Versão", 11, "#B8CCE4", ParagraphAlignment.CENTER); // Cor azul claro
            SetCellText(table.GetRow(0).GetCell(2), "Descrição", 11, "#B8CCE4", ParagraphAlignment.CENTER); // Cor azul claro
            SetCellText(table.GetRow(0).GetCell(3), "Autor", 11, "#B8CCE4", ParagraphAlignment.CENTER); // Cor azul claro

            for (int i = 0; i < revisionHistory.Count; i++)
            {
                var item = revisionHistory[i];
                SetCellText(table.GetRow(i + 1).GetCell(0), item.Data.ToString("dd/MM/yyyy"),9,null, ParagraphAlignment.CENTER);
                SetCellText(table.GetRow(i + 1).GetCell(1), item.Versao, 9, null, ParagraphAlignment.CENTER);
                SetCellText(table.GetRow(i + 1).GetCell(2), item.Descricao,9, null, ParagraphAlignment.CENTER);
                SetCellText(table.GetRow(i + 1).GetCell(3), item.Autor, 9, null, ParagraphAlignment.CENTER);
            }
        }

        private void AddEventsDeliveries(XWPFDocument doc, List<EventDeliveryItem> eventsDeliveries)
        {
            //DateTime yesterday = DateTime.Now.AddDays(-1);
            //DateTime yesterday = DateTime.Now;
            DateTime today = DateTime.Now.Date;  // Data atual

            XWPFParagraph sectionTitle = doc.CreateParagraph();
            sectionTitle.Alignment = ParagraphAlignment.LEFT;
            sectionTitle.SpacingAfter = 0; // Remove o espaçamento após o parágrafo
            XWPFRun sectionTitleRun = sectionTitle.CreateRun();
            sectionTitleRun.SetText("Eventos / Entregas");
            sectionTitleRun.IsBold = true;
            sectionTitleRun.FontSize = 14;

            // Crie um estilo para o parágrafo
            CT_PPr pPr = sectionTitle.GetCTP().AddNewPPr();
            CT_Shd shd = pPr.AddNewShd();
            shd.val = ST_Shd.clear;
            shd.fill = "#B8CCE4"; // Cor azul em hexadecimal

            // Linha 2: Eventos em andamento na SEDUC
            XWPFParagraph sectionTitle2 = doc.CreateParagraph();
            sectionTitle2.Alignment = ParagraphAlignment.LEFT;
            sectionTitle2.SpacingAfter = 0; // Remove o espaçamento após o parágrafo
            XWPFRun sectionTitleRun2 = sectionTitle2.CreateRun();
            sectionTitleRun2.SetText("Eventos em andamento na SEDUC:");
            sectionTitleRun2.IsBold = true;
            sectionTitleRun2.FontSize = 10;

            // Defina a cor de fundo (azul claro)
            CT_PPr pPr2 = sectionTitle2.GetCTP().AddNewPPr();
            CT_Shd shd2 = pPr2.AddNewShd();
            shd2.val = ST_Shd.clear;
            shd2.fill = "#FFFFFF"; // Cor azul em hexadecimal

            // Linha 3: Entregas do dia
            XWPFParagraph sectionTitle3 = doc.CreateParagraph();
            sectionTitle3.Alignment = ParagraphAlignment.LEFT;
            sectionTitle3.SpacingBefore = 0; // Remove o espaçamento antes do parágrafo
            XWPFRun sectionTitleRun3 = sectionTitle3.CreateRun();
            sectionTitleRun3.SetText("Entregas do dia:");
            sectionTitleRun3.IsBold = true;
            sectionTitleRun3.FontSize = 10;

            // Defina a cor de fundo (azul claro)
            CT_PPr pPr3 = sectionTitle3.GetCTP().AddNewPPr();
            CT_Shd shd3 = pPr3.AddNewShd();
            shd3.val = ST_Shd.clear;
            shd3.fill = "#FFFFFF"; // Cor azul em hexadecimal

            /*
            Eventos em andamento na SEDUC:
            Entregas do dia:
            */
            var groupedEvents = eventsDeliveries
               .Where(e => e.Status == "Concluido" &&
                           e.Custom_Sistema == "Transporte Escolar" &&
                           e.Conclusao.HasValue &&
                           e.Conclusao.Value.Date == today)  // Verifique a data específica de conclusão
               .GroupBy(e => e.Custom_Sistema);

            // Verificando se há eventos agrupados
            if (groupedEvents.Any())
            {
                foreach (var group in groupedEvents)
                {
                    Console.WriteLine($"Sistema: {group.Key}");
                    foreach (var eventDelivery in group)
                    {
                        // Imprimir os campos relevantes do evento
                        Console.WriteLine($"Evento ID: {eventDelivery.WorkItemId}, Conclusão: {eventDelivery.Conclusao}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nenhum evento encontrado para os critérios especificados.");
            }

            int systemCounter = 1;

            foreach (var group in groupedEvents)
            {
                // Sistema
                XWPFParagraph paragraph = doc.CreateParagraph();
                paragraph.Alignment = ParagraphAlignment.LEFT;
                paragraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                paragraph.SpacingAfterLines = 1;
                XWPFRun run = paragraph.CreateRun();
                run.SetText($"{systemCounter} - {group.Key}");
                run.IsBold = true;
                run.FontSize = 10;
                systemCounter++;

                foreach (var item in group)
                {
                    char subCounter = 'a';

                    // Título da atividade
                    XWPFParagraph activityTitleParagraph = doc.CreateParagraph();
                    activityTitleParagraph.Alignment = ParagraphAlignment.LEFT;
                    activityTitleParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    activityTitleParagraph.SpacingAfterLines = 1;
                    activityTitleParagraph.IndentationLeft = 567; // 5cm in twips (1 cm = 567 twips)
                    XWPFRun activityTitleRun = activityTitleParagraph.CreateRun();
                    activityTitleRun.SetText($"{subCounter}) Atividade: {item.Title}");
                    activityTitleRun.FontSize = 10;
                    subCounter++;

                    // Demanda
                    XWPFParagraph demandParagraph = doc.CreateParagraph();
                    demandParagraph.Alignment = ParagraphAlignment.LEFT;
                    demandParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    demandParagraph.SpacingAfterLines = 1;
                    demandParagraph.IndentationLeft = 1134; // 10cm in twips (2 cm = 1134 twips)
                    XWPFRun demandRun = demandParagraph.CreateRun();
                    demandRun.SetText($"• Demanda: {item.WorkItemId}");
                    demandRun.IsBold = true;
                    demandRun.FontSize = 10;

                    // Informações adicionais
                    CreateInfoParagraph(doc, "• Abertura", item.DataAbertura?.ToString("dd/MM/yyyy"));
                    CreateInfoParagraph(doc, "• Início", item.DataInicioAtendimento != default ? item.DataInicioAtendimento?.ToString("dd/MM/yyyy") : "N/A");
                    CreateInfoParagraph(doc, "• Previsão", item.DataPrevistaEntrega != default ? item.DataPrevistaEntrega?.ToString("dd/MM/yyyy") : "N/A");
                    CreateInfoParagraph(doc, "• Status", item.Status);
                    CreateInfoParagraph(doc, "• Data Conclusão", item.Conclusao != default ? item.Conclusao?.ToString("dd/MM/yyyy") : "N/A");
                    //CreateInfoParagraph(doc, "• Observação", item.Observacao);
                    SituacaoAtividade? situacao = SituacaoAtividadeExtensions.FromString(item.Status);
                    CreateInfoParagraph(doc, "• Observação", ObservacaoHelper.ObterObservacao(situacao));



                    // Ajuste de espaço entre parágrafos
                    demandParagraph.SpacingAfter = 0;
                    demandParagraph.SpacingBeforeLines = 0;
                }

                // Ajuste de espaço entre grupos de parágrafos
                sectionTitle.SpacingAfter = 0;
                sectionTitle.SpacingBeforeLines = 0;
            }
        }

        // Função auxiliar para criar parágrafos de informações adicionais
        private void CreateInfoParagraph(XWPFDocument doc, string title, string value)
        {
            XWPFParagraph infoParagraph = doc.CreateParagraph();
            infoParagraph.Alignment = ParagraphAlignment.LEFT;
            infoParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
            infoParagraph.SpacingAfterLines = 1;
            infoParagraph.IndentationLeft = 1134    ; // 5cm in twips (1 cm = 567 twips)
            XWPFRun infoRun = infoParagraph.CreateRun();
            infoRun.SetText($"{title}: {value}");
            infoRun.FontSize = 10;
        }

        private void AddOngoingProjects(XWPFDocument doc, List<ProjectItem> ongoingProjects)
        {
            string[] validStatuses = { "Aberto", "Desenvolvimento", "Análise", "Suspenso", "Suspenso - Temp", "Suspenso-Temp", "Aguardando Solicitante", "Homologacao", "Deploy Producao" };
            string[] validSistemas = { "Transporte Escolar" };

            XWPFParagraph sectionTitle = doc.CreateParagraph();
            sectionTitle.Alignment = ParagraphAlignment.LEFT;
            XWPFRun sectionTitleRun = sectionTitle.CreateRun();
            sectionTitleRun.SetText("Projetos em Andamento");
            sectionTitleRun.IsBold = true;
            sectionTitleRun.FontSize = 14;

            // Crie um estilo para o parágrafo
            CT_PPr pPr = sectionTitle.GetCTP().AddNewPPr();
            CT_Shd shd = pPr.AddNewShd();
            shd.val = ST_Shd.clear;
            shd.fill = "#B8CCE4"; // Cor azul em hexadecimal

            var groupedProjects = ongoingProjects
               .Where(p => p.Custom_Sistema == "Transporte Escolar")
               .GroupBy(p => p.Custom_Sistema);

            int systemCounter = 1;

            foreach (var group in groupedProjects)
            {
                // Sistema
                XWPFParagraph paragraph = doc.CreateParagraph();
                paragraph.Alignment = ParagraphAlignment.LEFT;
                paragraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                paragraph.SpacingAfterLines = 1;
                XWPFRun run = paragraph.CreateRun();
                run.SetText($"{systemCounter} - {group.Key}");
                run.IsBold = true;
                run.FontSize = 10;
                systemCounter++;

                char subCounter = 'a';

                foreach (var item in group)
                {

                    // Recuo para a direita a partir do campo WorkItemId

                    if (item.WorkItemId != default)
                    {
                        paragraph.IndentationLeft = 0; // Valor em unidades de 1/20 de ponto, 500 equivale a 5cm
                    }

                    // Título da atividade
                    XWPFParagraph activityTitleParagraph = doc.CreateParagraph();
                    activityTitleParagraph.Alignment = ParagraphAlignment.LEFT;
                    activityTitleParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    activityTitleParagraph.SpacingAfterLines = 1;
                    activityTitleParagraph.IndentationLeft = 567; // 5cm in twips (1 cm = 567 twips)
                    XWPFRun activityTitleRun = activityTitleParagraph.CreateRun();
                    activityTitleRun.SetText($"{subCounter}) Atividade: {item.Title}");
                    activityTitleRun.FontSize = 10;
                    subCounter++;

                    // Demanda
                    XWPFParagraph demandParagraph = doc.CreateParagraph();
                    demandParagraph.Alignment = ParagraphAlignment.LEFT;
                    demandParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    demandParagraph.SpacingAfterLines = 1;
                    demandParagraph.IndentationLeft = 1134; // 10cm in twips (2 cm = 1134 twips)
                    XWPFRun demandRun = demandParagraph.CreateRun();
                    demandRun.SetText($"• Demanda: {item.WorkItemId}");
                    demandRun.IsBold = true;
                    demandRun.FontSize = 10;

                    // Informações adicionais
                    CreateInfoParagraph(doc, "• Abertura", item.DataAbertura?.ToString("dd/MM/yyyy"));
                    CreateInfoParagraph(doc, "• Início", item.DataInicioAtendimento != default ? item.DataInicioAtendimento?.ToString("dd/MM/yyyy") : "N/A");
                    CreateInfoParagraph(doc, "• Previsão", item.DataPrevistaEntrega != default ? item.DataPrevistaEntrega?.ToString("dd/MM/yyyy") : "N/A");
                    //CreateInfoParagraph(doc, "• Previsão", item.DataPrevistaEntrega?.ToString("dd/MM/yyyy"));
                    CreateInfoParagraph(doc, "• Status", item.Status);
                    CreateInfoParagraph(doc, "• Data Conclusão", item.Conclusao != default ? item.Conclusao?.ToString("dd/MM/yyyy") : "N/A");
                    SituacaoAtividade? situacao = SituacaoAtividadeExtensions.FromString(item.Status);
                    CreateInfoParagraph(doc, "• Observação", ObservacaoHelper.ObterObservacao(situacao));
                    //CreateInfoParagraph(doc, "• Observação", item.Observacao);


                    // Ajuste de espaço entre parágrafos
                    paragraph.SpacingAfter = 0;
                    paragraph.SpacingBeforeLines = 0;
                }

                // Ajuste de espaço entre grupos de parágrafos
                sectionTitle.SpacingAfter = 0;
                sectionTitle.SpacingBeforeLines = 0;
            }

        }

        private void AddSuspendProjects(XWPFDocument doc, List<ProjectSuspendItem> projectSuspendItems)
        {
            string[] validStatuses = { "Suspenso", "Suspenso-Temp" };
            string[] validSistemas = { "Indicação Escolas PEI", "PLACON (Plataforma Conviva SP)" };

            XWPFParagraph sectionTitle = doc.CreateParagraph();
            sectionTitle.Alignment = ParagraphAlignment.LEFT;
            XWPFRun sectionTitleRun = sectionTitle.CreateRun();
            sectionTitleRun.SetText("Projetos Suspenso / Abortado");
            sectionTitleRun.IsBold = true;
            sectionTitleRun.FontSize = 14;

            // Crie um estilo para o parágrafo
            CT_PPr pPr = sectionTitle.GetCTP().AddNewPPr();
            CT_Shd shd = pPr.AddNewShd();
            shd.val = ST_Shd.clear;
            shd.fill = "#B8CCE4"; // Cor azul em hexadecimal

            XWPFParagraph sectionTitle2 = doc.CreateParagraph();
            sectionTitle.Alignment = ParagraphAlignment.LEFT;
            XWPFRun sectionTitleRun2 = sectionTitle2.CreateRun();
            sectionTitleRun2.SetText("Por decisão do cliente, abortaram qualquer mudança no novo sistema de Indicação de Escola SED, passando a usar o sistema antigo – Indicação de Escolas PEI do PortalNet. Estamos acompanhando o processo.");
            sectionTitleRun2.IsBold = true;
            sectionTitleRun2.IsItalic = true;
            sectionTitleRun2.FontSize = 9;

            // Crie um estilo para o parágrafo
            CT_PPr pPr2 = sectionTitle2.GetCTP().AddNewPPr();
            CT_Shd shd2 = pPr2.AddNewShd();
            shd2.val = ST_Shd.clear;
            shd2.fill = "#FFFFFF"; // Cor branco

            var groupedProjects = projectSuspendItems
            .GroupBy(p => p.Custom_Sistema);

            int systemCounter = 1;

            foreach (var group in groupedProjects)
            {
                // Sistema
                XWPFParagraph paragraph = doc.CreateParagraph();
                paragraph.Alignment = ParagraphAlignment.LEFT;
                paragraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                paragraph.SpacingAfterLines = 1;
                XWPFRun run = paragraph.CreateRun();
                run.SetText($"{systemCounter} - {group.Key}");
                run.IsBold = true;
                run.FontSize = 10;
                systemCounter++;

                char subCounter = 'a';

                foreach (var item in group)
                {

                    // Recuo para a direita a partir do campo WorkItemId

                    if (item.WorkItemId != default)
                    {
                        paragraph.IndentationLeft = 0; // Valor em unidades de 1/20 de ponto, 500 equivale a 5cm
                    }

                    // Título da atividade
                    XWPFParagraph activityTitleParagraph = doc.CreateParagraph();
                    activityTitleParagraph.Alignment = ParagraphAlignment.LEFT;
                    activityTitleParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    activityTitleParagraph.SpacingAfterLines = 1;
                    activityTitleParagraph.IndentationLeft = 567; // 5cm in twips (1 cm = 567 twips)
                    XWPFRun activityTitleRun = activityTitleParagraph.CreateRun();
                    activityTitleRun.SetText($"{subCounter}) Atividade: {item.Title}");
                    activityTitleRun.FontSize = 10;
                    subCounter++;

                    // Demanda
                    XWPFParagraph demandParagraph = doc.CreateParagraph();
                    demandParagraph.Alignment = ParagraphAlignment.LEFT;
                    demandParagraph.SpacingBetween = 1; // Espaçamento simples entre linhas
                    demandParagraph.SpacingAfterLines = 1;
                    demandParagraph.IndentationLeft = 1134; // 10cm in twips (2 cm = 1134 twips)
                    XWPFRun demandRun = demandParagraph.CreateRun();
                    demandRun.SetText($"• Demanda: {item.WorkItemId}");
                    demandRun.IsBold = true;
                    demandRun.FontSize = 10;

                    // Informações adicionais
                    CreateInfoParagraph(doc, "• Abertura", item.DataAbertura?.ToString("dd/MM/yyyy"));
                    CreateInfoParagraph(doc, "• Início", item.DataInicioAtendimento?.ToString("dd/MM/yyyy"));
                    CreateInfoParagraph(doc, "• Previsão", item.DataPrevistaEntrega != default ? item.DataPrevistaEntrega?.ToString("dd/MM/yyyy") : "N/A");
                    CreateInfoParagraph(doc, "• Status", item.Status);
                    CreateInfoParagraph(doc, "• Data Conclusão", item.Conclusao != default ? item.Conclusao?.ToString("dd/MM/yyyy") : "N/A");
                    SituacaoAtividade? situacao = SituacaoAtividadeExtensions.FromString(item.Status);
                    CreateInfoParagraph(doc, "• Observação", ObservacaoHelper.ObterObservacao(situacao));
                    //CreateInfoParagraph(doc, "• Observação", item.Observacao);


                    // Ajuste de espaço entre parágrafos
                    paragraph.SpacingAfter = 0;
                    paragraph.SpacingBeforeLines = 0;
                }

                // Ajuste de espaço entre grupos de parágrafos
                sectionTitle.SpacingAfter = 0;
                sectionTitle.SpacingBeforeLines = 0;
            }

        }

        // Método para adicionar a seção Custom_Sistema nos Eventos e Entregas
        private void AddCustomSistemaEventDelivery(XWPFDocument doc, List<EventDeliveryItem> eventsDeliveries)
        {
            XWPFParagraph customSistemaTitle = doc.CreateParagraph();
            customSistemaTitle.Alignment = ParagraphAlignment.LEFT;
            XWPFRun customSistemaTitleRun = customSistemaTitle.CreateRun();
            customSistemaTitleRun.SetText("Custom_Sistema - Eventos e Entregas:");
            customSistemaTitleRun.IsBold = true;
            customSistemaTitleRun.FontSize = 12;

            foreach (var item in eventsDeliveries)
            {
                XWPFParagraph customSistemaItem = doc.CreateParagraph();
                customSistemaItem.Alignment = ParagraphAlignment.LEFT;

                // Formato customizado para Custom_Sistema: Title - Demanda - Abertura - Início - Previsão - Status - Observação
                XWPFRun customSistemaItemRun = customSistemaItem.CreateRun();
                customSistemaItemRun.SetText($"Custom_Sistema: {item.Title} - {item.Status} - {item.DataAbertura?.ToString("dd/MM/yyyy")} - {item.DataInicioAtendimento?.ToString("dd/MM/yyyy")} - {item.DataPrevistaEntrega?.ToString("dd/MM/yyyy")} - {(item.Conclusao != null ? item.Conclusao.Value.ToString("dd/MM/yyyy") : "N/A")} - {item.Custom_Finalidade}");
                customSistemaItemRun.FontSize = 11;
            }
        }

        // Método para adicionar a seção Custom_Sistema nos Projetos em Andamento
        private void AddCustomSistemaOngoingProject(XWPFDocument doc, List<ProjectItem> ongoingProjects)
        {
            XWPFParagraph customSistemaTitle = doc.CreateParagraph();
            customSistemaTitle.Alignment = ParagraphAlignment.LEFT;
            XWPFRun customSistemaTitleRun = customSistemaTitle.CreateRun();
            customSistemaTitleRun.SetText("Custom_Sistema - Projetos em Andamento:");
            customSistemaTitleRun.IsBold = true;
            customSistemaTitleRun.FontSize = 12;

            foreach (var item in ongoingProjects)
            {
                XWPFParagraph customSistemaItem = doc.CreateParagraph();
                customSistemaItem.Alignment = ParagraphAlignment.LEFT;

                // Formato customizado para Custom_Sistema: Title - Demanda - Abertura - Início - Previsão - Status - Observação
                XWPFRun customSistemaItemRun = customSistemaItem.CreateRun();
                customSistemaItemRun.SetText($"Custom_Sistema: {item.Title} - {item.Status} - {item.DataAbertura?.ToString("dd/MM/yyyy")} - {item.DataInicioAtendimento?.ToString("dd/MM/yyyy")} - {item.DataPrevistaEntrega?.ToString("dd/MM/yyyy")} - {item.Conclusao?.ToString("dd/MM/yyyy")} - {item.Observacao}");
                customSistemaItemRun.FontSize = 11;
            }
        }
    }
}
