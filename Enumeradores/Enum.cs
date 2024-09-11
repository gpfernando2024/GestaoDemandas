using GestaoDemandas.Enumeradores;
using System;
using System.Collections.Generic;
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
        AguardandoDesenvolvimento
    }

    public enum Complemento
    {
        EmTeste,
        ElaboracaoDocumentacaoTecnica,
        Impedido,
        ExecuçãoScript,
        Aguardando,
        SuspensoTemporariamente,
        AguardandoDeploy,
        AguardandoCliente
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

                    return $"Publicado em Produção: {dataPublicacao.ToShortDateString()}";

                case SituacaoAtividade.Análise:
                    return "Em análise";

                case SituacaoAtividade.SuspensoTemp:
                    return "Atividade suspensa temporariamente para atendimento de outras demandas urgentes ou projeto/atividade suspenso por decisão do cliente";

                case SituacaoAtividade.Desenvolvimento:
                    return "Atividade em desenvolvimento";

                case SituacaoAtividade.Suspenso:
                    return "Projeto suspenso e/ou abortado pelo cliente";

                case SituacaoAtividade.AguardandoSolicitante:
                    return "Atividade no aguardo de homologação do cliente ou outra ação relevante do cliente";

                case SituacaoAtividade.Aberto:
                    return "Atividade pendente de início";
                case SituacaoAtividade.Homologacao:
                    return "Atividade pendente de homologação pelo cliente";
                case SituacaoAtividade.DeployProducao:
                    return "Atividade pendente de publicação em Produção";
                case SituacaoAtividade.AguardandoDesenvolvimento:
                    return "Atividade pendente de ação de desenvolvimento";
                default:
                    return "Situação não reconhecida";
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
                    return ".Elaboração da Documentação Técnica.";
                case Complemento.Impedido:
                    return ".Atividade com algum impedimento de concluir a atividade.";
                case Complemento.AguardandoCliente:
                    return ".Atividade com pendência de ação do cliente ou homologação da funcionalidade";
                case Complemento.EmTeste:
                    return ".Atividade Em Teste.";
                case Complemento.SuspensoTemporariamente:
                    return ". Atividade supenso temporariamente pelo cliente para atendimento de outra(s) demandas";
                case Complemento.ExecuçãoScript:
                    return ". Execução de script de ajuste de alguma correção.";
                case Complemento.Aguardando:
                    return ". Também depende de uma ação de outra(s) equipe(s) para o desenvolvimento necessário.";
                case Complemento.AguardandoDeploy:
                    return ". Atividade na esteira de publicação em produção.";
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
                default: return null;   
            }
        }
    }
}
