﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDemandas.Models
{
    public class DailyReportViewModel
    {
        public string ReportTitle { get; set; }
        public string ProjectTitle { get; set; }
        public List<RevisionHistoryItem> RevisionHistory { get; set; }
        public List<EventDeliveryItem> EventsDeliveries { get; set; }
        public List<ProjectItem> OngoingProjects { get; set; }
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
        public DateTime? DataAbertura { get; set; }

        public DateTime? DataInicioAtendimento { get; set; }

        public string Status { get; set; }
        public DateTime? DataPrevistaEntrega { get; set; }

        public string Custom_Finalidade { get; set; }
        public DateTime? Conclusao { get; set; }
        public string Observacao { get; set; }
    }
    public class ProjectItem
    {
        public string Title { get; set; }
        public int WorkItemId { get; set; }
        public string Custom_Sistema { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataInicioAtendimento { get; set; }
        public string Status { get; set; }
 
        public DateTime? DataPrevistaEntrega { get; set; }
        public string Observacao { get; set; }
        public DateTime? Conclusao { get; set; }
        public DateTime? DataFechamento { get; set; }
    }
}
