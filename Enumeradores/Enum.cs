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
        Análise
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
                default:
                    return null; // ou lance uma exceção
            }
        }
    }
}
