using DocumentFormat.OpenXml.Office.CustomUI;
using GestaoDemandas.Enumeradores;
using Microsoft.Ajax.Utilities;
using NPOI.HSSF.Record.Chart;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;

namespace GestaoDemandas.Enumeradores
{
    public enum SituacaoAtividade
    {
        /*Status Atividade: 
         Concluido = 1,             
         Analise = 2,
         SuspensoTemp = 3,
         Desenvolvimento = 4,
         Aberto = 5,
         Suspenso = 6,
         Deploy Producao = 7,
         Aguardando Solicitante = 8
         */

        Aberto,
        Concluido,
        SuspensoTemp,
        Desenvolvimento,
        AguardandoSolicitante,
        Suspenso,
        Análise,
        Homologacao,
        DeployProducao,
        AguardandoDesenvolvimento,
        RevisãoTecnica
    }

    public enum Complemento
    {
        EmTeste,
        ElaboracaoDocumentacaoTecnica,
        Impedido,
        ExecuçãoScript,
        Aguardando,
        Pendente_COFI,
        SuspensoTemporariamente,
        AguardandoDeploy,
        AguardandoCliente,
        TesteFinalizado,
        EmailCliente,
        AtividadeCancelado,
        ManualUsuario,
        AtividadeImpedidoReuniaoCliente,
        ElaboracaoRequisito,
        ReunicaoCliente,
        AtividadeFinalizado,
        AprovacaoRequisito,
        AguardandoDefinicao
    }
        

    public class ObservacaoHelper
    {
        public static string ObterObservacao(SituacaoAtividade? status)
        {
            switch (status)
            {
                case SituacaoAtividade.Concluido:
                    // Lógica para calcular a data de publicação em produção
                    DateTime dataPublicacao = DateTime.Now.AddDays(-1);
                    if (dataPublicacao.DayOfWeek == DayOfWeek.Saturday) 
                        dataPublicacao = dataPublicacao.AddDays(-2);
                    else if (dataPublicacao.DayOfWeek == DayOfWeek.Sunday)
                        dataPublicacao = dataPublicacao.AddDays(-3);

                    //return $"Publicado em Produção: {dataPublicacao.ToShortDateString()}";
                    return $"Publicado em Produção: ";

                case SituacaoAtividade.Análise:
                    return "Em análise";

                case SituacaoAtividade.SuspensoTemp:
                    return "Atividade suspensa temporariamente para atendimento de outras demandas urgentes. ";

                case SituacaoAtividade.Desenvolvimento:
                    return "Atividade em desenvolvimento.";

                case SituacaoAtividade.Suspenso:
                    return "Projeto suspenso e/ou abortado pelo cliente.";

                case SituacaoAtividade.AguardandoSolicitante:
                    return "Atividade no aguardo de homologação do cliente ou outra ação relevante do cliente.";

                case SituacaoAtividade.Aberto:
                    return "Atividade pendente de início.";
                case SituacaoAtividade.Homologacao:
                    return "Atividade pendente de homologação pelo cliente.";
                case SituacaoAtividade.DeployProducao:
                    return "Atividade pendente de publicação em Produção (aplicação) ou execução de script de correção na tabela.";
                case SituacaoAtividade.AguardandoDesenvolvimento:
                    return "Atividade pendente de ação de desenvolvimento.";
                case SituacaoAtividade.RevisãoTecnica:
                    return "Atividade em análise técnica.";
                default:
                    return "Situação não reconhecida.";
            }
        }
    }

    public static class SituacaoAtividadeExtensions
    {
        public static SituacaoAtividade? FromString(string status)
        {
            switch (status)
            {
                case "Concluido":
                    return SituacaoAtividade.Concluido;
                case "Aberto":
                    return SituacaoAtividade.Aberto;
                case "Suspenso-Temp":
                    return SituacaoAtividade.SuspensoTemp;
                case "Desenvolvimento":
                    return SituacaoAtividade.Desenvolvimento;
                case "Aguardando Solicitante":
                    return SituacaoAtividade.AguardandoSolicitante;
                case "Suspenso":
                    return SituacaoAtividade.Suspenso;
                case "Análise":
                    return SituacaoAtividade.Análise;
                case "Homologacao":
                    return SituacaoAtividade.Homologacao;
                case "Deploy Producao":
                    return SituacaoAtividade.DeployProducao;
                case "Aguardando Desenvolvimento":
                    return SituacaoAtividade.AguardandoDesenvolvimento;
                case "Revisão Técnica":
                    return SituacaoAtividade.RevisãoTecnica;
                default:
                    return null; // ou lance uma exceção
            }
        }
    }

    public static class ComplementoObservacao
    {
        public static string ObterComplemento(Complemento? complemento)  
        {
            switch (complemento)
            {
                case Complemento.ElaboracaoDocumentacaoTecnica:
                    return " Elaboração da Documentação Técnica.";
                case Complemento.Impedido:
                    return " Atividade com algum impedimento de concluir a atividade.";
                case Complemento.AguardandoCliente:
                    return " Atividade com pendência de ação do cliente ou homologação da funcionalidade";
                case Complemento.EmTeste:
                    return " Atividade Em Teste.";
                case Complemento.SuspensoTemporariamente:
                    return " Atividade supenso temporariamente pelo cliente para atendimento de outra(s) demandas";
                case Complemento.ExecuçãoScript:
                    return " Criação de script de ajuste de alguma correção em Tabelas.";
                case Complemento.Aguardando:
                    return " Também depende de uma ação do cliente de homologação.";
                case Complemento.Pendente_COFI:
                    return " Também depende de uma ação de outra(s) equipe(s) para o desenvolvimento necessário.";
                case Complemento.AguardandoDeploy:
                    return " Atividade na esteira de publicação em produção.";
                case Complemento.TesteFinalizado:
                    return " Teste da funcionalidade concluída conforme solicitado.";
                case Complemento.EmailCliente:
                    return " Enviado e-mail ao cliente. No aguardo do feedback.";
                case Complemento.AtividadeCancelado:
                    return " Atividade cancelado por não haver necessidade. Não foi identificado nenhum problema com a aplicação.";
                case Complemento.ManualUsuario:
                    return " Atividade finalizada. Elaboração manual do usuário.";
                case Complemento.AtividadeImpedidoReuniaoCliente:
                    return "Atividade com impedimento. Motivo: Regra de negócio já implementado. Reunião com o cliente solicitado.";
                case Complemento.ElaboracaoRequisito:
                    return "Elaboração da proposta técnica.";
                case Complemento.ReunicaoCliente:
                    return "Realizado reunião com o cliente para tratar de assuntos de dúvidas ou novas demandas.";
                case Complemento.AtividadeFinalizado:
                    return "Atividade finalizada. Não foi identificado nenhum problema com a aplicação.";
                case Complemento.AprovacaoRequisito:
                    return "Atividade pendente da aprovação do documento de requisitos pelo cliente.";
                case Complemento.AguardandoDefinicao:
                    return "Atividade no aguardo da definição da regra de negócio.";
                default:
                    return null;
            }
        }
    }

    public static class ComplementoExtensions 
    {
        public static Complemento? FromString(string complemento) 
        {
            switch (complemento)
            {
                case "Elaboração Documentação Técnica":
                    return Complemento.ElaboracaoDocumentacaoTecnica;
                case "Impedido":
                    return Complemento.Impedido;
                case "Aguardando":
                    return Complemento.Aguardando;
                case "Aguardando Deploy":
                    return Complemento.AguardandoDeploy;
                case "Aguardando Cliente":
                    return Complemento.AguardandoCliente;
                case "Execução Script":
                    return Complemento.ExecuçãoScript;
                case "Pendente COFI":
                    return Complemento.Pendente_COFI;
                case "Teste Finalizado":
                    return Complemento.TesteFinalizado;
                case "Enviado E-mail Cliente":
                    return Complemento.EmailCliente;
                case "Em Teste":
                    return Complemento.EmTeste;
                case "Atividade Cancelada":
                    return Complemento.AtividadeCancelado;
                case "Manual Usuario":
                    return Complemento.ManualUsuario;
                case "Atividade Impedido":
                    return Complemento.AtividadeImpedidoReuniaoCliente;
                case "Elaboração Requisito":
                    return Complemento.ElaboracaoRequisito;
                case "Reunião Cliente":
                    return Complemento.ReunicaoCliente;
                case "Atividade Finalizada":
                    return Complemento.AtividadeFinalizado;
                case "Aprovação Requisito":
                    return Complemento.AprovacaoRequisito;
                case "Aguardando Definição":
                    return Complemento.AguardandoDefinicao;
                default: return null;   
            }
        }
    }
}
