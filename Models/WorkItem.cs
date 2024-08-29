using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static NPOI.HSSF.Util.HSSFColor;

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
        public WorkItem(DateTime? custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d, DateTime? Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef)
        {
            if (custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d.HasValue)
            {
                /*Custom_DataAbertura*/
                Custom_DataAbertura = custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d.Value;
            }
            else
            {
                Custom_DataAbertura = null; // ou DateTime.MinValue, se preferir
            }

            if (Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.HasValue)
            {
                /*Custom_DataFechamento*/
                Custom_DataFechamento = Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef.Value;
            }
            else
            {
                Custom_DataFechamento = null; // ou DateTime.MinValue, se preferir
            }
        }
        public DateTime? Custom_DataInicioAtendimento { get; set; }
        public DateTime? Custom_DataPrevistaDaEntrega { get; set; }
        public string DescricaoProjeto {  get; set; }
        public string Custom_SemanaProdesp { get; set; }
        public string Custom_NomeProjeto { get; set; }
        public string EntregaEstrategica { get; set; }
        public string GerenciaProdesp { get; set; }
        public DateTime ? Custom_DataRealHomologacao {  get; set; }

        [JsonConstructor]
        public WorkItem(DateTime? Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a, 
             string Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e, 
             string Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9, string Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c)
        {
            //Data Real da Homologação
            if (Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a.HasValue)
            {
                Custom_DataRealHomologacao = Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a.Value;
            }
            else
            {
                Custom_DataRealHomologacao = null; // ou DateTime.MinValue, se preferir
            }

            DescricaoProjeto = Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e;  //DescriçãoProjeto (Descrição do Projeto, relacionada a sessão Gestão Prodesp)
            EntregaEstrategica = Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9;//Entrega Estratégica
            GerenciaProdesp = Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c;   //GerênciaProdesp
        }

        public string Custom_Atividade { get; set; }
        public DateTime? Custom_DataAbertura { get; set; }
        public DateTime? Custom_DataFechamento { get; set; }
        public AssignedTo AssignedTo { get; set; }
        public List<Team> Teams { get; set; } // Adicione esta propriedade
        public List<BoardLocations> BoardLocations { get; set; }
    }

    public class AssignedTo
    {
        public string UserName { get; set; }
    }

    public class Team
    {
        public string TeamName { get; set; }
    }
    public class BoardLocations 
    { 
        public string ColumnName { get; set; }
        public string IsDone { get; set; }
        public string BoardName  { get; set; }

    }
}
