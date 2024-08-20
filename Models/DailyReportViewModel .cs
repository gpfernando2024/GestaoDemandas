using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static NPOI.HSSF.Util.HSSFColor;

namespace GestaoDemandas.Models
{
    public class DailyReportViewModel
    {
        public string ReportTitle { get; set; }
        public string ProjectTitle { get; set; }
        public List<RevisionHistoryItem> RevisionHistory { get; set; }
        public List<EventDeliveryItem> EventsDeliveries { get; set; }
        public List<ProjectItem> OngoingProjects { get; set; }
        public string Observacao { get; set; }
        public List<ProjectSuspendItem> projectSuspendItems { get; set; }
    }

    public class RevisionHistoryItem
    {
        public DateTime Data { get; set; }
        public string Versao { get; set; }
        public string Descricao { get; set; }
        public string Autor { get; set; }
    }

    public class EventDeliveryItem
    {
        public string Title { get; set; }
        public int WorkItemId { get; set; }
        public string Custom_Sistema { get; set; }
        public string TagNames { get; set; }
        public DateTime? DataAbertura { get; set; }

        public DateTime? DataInicioAtendimento { get; set; }

        public string Status { get; set; }
        public DateTime? DataPrevistaEntrega { get; set; }

        public string Custom_Finalidade { get; set; }
        public DateTime? Conclusao { get; set; }
        public string Observacao { get; set; }
        public string DescriçãoProjeto { get; set; }
        public string GerênciaProdesp { get; set; }
        public string EntregaEstratégica { get; set; }
        public string SemanaProdesp { get; set; }
        public string NomeProjeto { get; set; }
        public string Finalidade { get; set; }
        public DateTime? DataRealHomologação { get; set; }
    }
    public class ProjectItem
    {
        public string Title { get; set; }
        public int WorkItemId { get; set; }
        public string Custom_Sistema { get; set; }
        public string TagNames { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataInicioAtendimento { get; set; }
        public string Status { get; set; }
 
        public DateTime? DataPrevistaEntrega { get; set; }
        public string Observacao { get; set; }
        public DateTime? Conclusao { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string DescriçãoProjeto { get; set; }
        public string GerênciaProdesp { get; set; }
        public string EntregaEstratégica { get; set; }
        public string SemanaProdesp { get; set; }
        public string NomeProjeto { get; set; }
        public string Finalidade { get; set; }
        public DateTime? DataRealHomologação { get; set; }
    }

    public class ProjectSuspendItem
    {
        public string Title { get; set; }
        public int WorkItemId { get; set; }
        public string Custom_Sistema { get; set; }
        public string TagNames { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataInicioAtendimento { get; set; }
        public string Status { get; set; }

        public DateTime? DataPrevistaEntrega { get; set; }
        public string Observacao { get; set; }
        public DateTime? Conclusao { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string DescriçãoProjeto { get; set; }
        public string GerênciaProdesp { get; set; }
        public string EntregaEstratégica { get; set; }
        public string SemanaProdesp { get; set; }
        public string NomeProjeto { get; set; }
        public string Finalidade { get; set; }
        public DateTime? DataRealHomologação { get; set; }
    }
}
