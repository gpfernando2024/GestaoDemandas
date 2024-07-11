using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDemandas.Models
{
    public class WorkItem
    {
        public int WorkItemId { get; set; }
        public string Title { get; set; }
        public string Custom_Sistema { get; set; }
        public string State { get; set; }
        public string Custom_Prioridade_Epic { get; set; }
        public string Custom_Finalidade { get; set; }
        /*Custom_DataAbertura*/
        public DateTime Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d { get; set; }
        public DateTime? Custom_DataInicioAtendimento { get; set; }
        public DateTime? Custom_DataPrevistaDaEntrega { get; set; }
        /*Custom_DataFechamento*/
        public DateTime? Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef { get; set; }
    }

}