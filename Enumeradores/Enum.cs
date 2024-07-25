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

        Concluido = 1,             
        Analise = 2,
        SuspensoTemp = 3,
        Desenvolvimento = 4,
        Aberto = 5,
        Suspenso = 6,
        DeployProducao = 7,
        AguardandoSolicitante = 8
    }

    public class ObservacaoHelper
    {
        public static string ObterObservacao(SituacaoAtividade situacao)
        {
            switch (situacao)
            {
                case SituacaoAtividade.Concluido:
                    // Lógica para calcular a data de publicação em produção
                    DateTime dataPublicacao = DateTime.Now.AddDays(-1);
                    if (dataPublicacao.DayOfWeek == DayOfWeek.Saturday)
                        dataPublicacao = dataPublicacao.AddDays(-2);
                    else if (dataPublicacao.DayOfWeek == DayOfWeek.Sunday)
                        dataPublicacao = dataPublicacao.AddDays(-3);

                    return $"Publicado em Produção: {dataPublicacao.ToShortDateString()}";

                case SituacaoAtividade.Analise:
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
}