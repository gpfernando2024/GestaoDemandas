﻿using System;
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
        public string TagNames { get; set; }
        public string State { get; set; }
        public string Custom_Prioridade_Epic { get; set; }
        public string Custom_Finalidade { get; set; }
        /*Custom_DataAbertura*/
        public DateTime Custom_c4b5f670__002D39f1__002D40fd__002Dace5__002D329f6170c36d { get; set; }
        public DateTime? Custom_DataInicioAtendimento { get; set; }
        public DateTime? Custom_DataPrevistaDaEntrega { get; set; }
        /*Custom_DataFechamento*/
        public DateTime? Custom_e9e5e387__002D39de__002D4875__002D94a5__002Db5721f8e21ef { get; set; }
        //DescriçãoProjeto (Descrição do Projeto, relacionada a sessão Gestão Prodesp)
        public string Custom_b4f03334__002D2822__002D4015__002D8439__002D3f002a94bf8e { get; set; }
        //GerênciaProdesp
        public string Custom_768b8fc1__002D37ad__002D4ebb__002Da7e1__002Df8f7bc8e2c1c { get; set; }
        //Entrega Estratégica
        public string Custom_dd460af2__002D5f88__002D4581__002D8205__002De63c777ecef9 { get; set; }
        public string Custom_SemanaProdesp { get; set; }
        public string Custom_NomeProjeto { get; set; }
        //Data Real da Homologação
        public DateTime? Custom_22fc3f0b__002D6c54__002D4770__002Dacb3__002D8d7b813ae13a { get; set; }
        public string Custom_Atividade { get; set; }
    }

}