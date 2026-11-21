using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InteracaoHome : MonoBehaviour
{
    [Header("Exploração e saída")]
    [SerializeField] private Transform jogador;
    [SerializeField] private Transform banheiro;
    [SerializeField] private Transform quarto;
    [SerializeField] private Transform cozinha;
    [SerializeField] private Transform saida;
    [SerializeField, Min(0.1f)] private float raio = 1.5f;

    [Header("Jantar")]
    [SerializeField] private Collider mesa;
    [SerializeField] private CanvasGroup telaPreta;
    [SerializeField, Min(0f)] private float duracaoFade = 0.35f;
    [SerializeField, Min(0f)] private float duracaoJantar = 2f;
    [SerializeField] private Behaviour[] suspenderDuranteFade;

    // Mesmos limites usados pelo Airport e Visitor Center.
    private const float TempoParado = 3f;
    private const float VelocidadeParada = 0.15f;

    internal static InteracaoHome MesaDisponivel { get; private set; }
    private int quadroDisponibilidade;
    private bool mesaConfirmada;
    private int visitaMesa;

    internal async Task AguardarPermanencia(string comodo)
    {
        Transform ponto;
        switch (comodo)
        {
            case "banheiro": ponto = banheiro; break;
            case "quarto": ponto = quarto; break;
            case "cozinha": ponto = cozinha; break;
            default: throw new ArgumentException("Cômodo desconhecido.", nameof(comodo));
        }

        if (jogador == null || ponto == null)
            throw new InvalidOperationException($"Configure o jogador e o ponto {comodo} do Home.");

        float tempo = 0f;
        Vector3 anterior = jogador.position;
        while (tempo < TempoParado)
        {
            await ProximoFrame();
            Vector3 posicao = jogador.position;
            float dt = Time.deltaTime;
            bool dentro = (posicao - ponto.position).sqrMagnitude <= raio * raio;
            bool parado = (posicao - anterior).magnitude <= VelocidadeParada * dt;
            tempo = dentro && parado ? tempo + dt : 0f;
            anterior = posicao;
        }
    }

    internal async Task AguardarTravessiaDaSaida()
    {
        if (jogador == null || saida == null)
            throw new InvalidOperationException("Configure o jogador e o ponto da saída do Home.");

        // A frente do ponto indica o lado externo. Só vale cruzar de dentro para fora.
        Vector3 anterior = jogador.position;
        while (true)
        {
            await ProximoFrame();
            Vector3 atual = jogador.position;
            float ladoAnterior = Vector3.Dot(anterior - saida.position, saida.forward);
            float ladoAtual = Vector3.Dot(atual - saida.position, saida.forward);
            if (ladoAnterior <= 0f && ladoAtual > 0f)
            {
                Vector3 cruzamento = Vector3.Lerp(anterior, atual, -ladoAnterior / (ladoAtual - ladoAnterior));
                if ((cruzamento - saida.position).sqrMagnitude <= raio * raio)
                    return;
            }

            anterior = atual;
        }
    }

    internal async Task AguardarMesa()
    {
        if (mesa == null || !mesa.enabled || !mesa.gameObject.activeInHierarchy || telaPreta == null
            || !telaPreta.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Configure o Collider da mesa e a tela de jantar do Home.");

        quadroDisponibilidade = Time.frameCount;
        mesaConfirmada = false;
        visitaMesa = SessaoCenario.Atual.Versao;
        MesaDisponivel = this;
        try
        {
            while (!mesaConfirmada)
                await ProximoFrame();
        }
        finally
        {
            if (MesaDisponivel == this)
                MesaDisponivel = null;
        }
    }

    internal void TentarInteragirComMesa(Collider alvo)
    {
        if (!SessaoCenario.Atual.Permite("Home", visitaMesa))
            return;
        if (MesaDisponivel != this || mesaConfirmada || alvo != mesa || Time.frameCount <= quadroDisponibilidade)
            return;

        mesaConfirmada = true;
    }

    internal async Task SimularJantar()
    {
        if (telaPreta == null || !telaPreta.gameObject.activeInHierarchy)
            throw new InvalidOperationException("A tela de jantar do Home precisa estar ativa.");

        float escalaAnterior = Time.timeScale;
        float alphaAnterior = telaPreta.alpha;
        bool bloqueava = telaPreta.blocksRaycasts;
        var suspensos = new List<Behaviour>();
        try
        {
            Time.timeScale = 0f;
            telaPreta.blocksRaycasts = true;
            if (suspenderDuranteFade != null)
            {
                foreach (Behaviour componente in suspenderDuranteFade)
                {
                    if (componente == null || !componente.enabled || componente == this || componente == telaPreta)
                        continue;

                    suspensos.Add(componente);
                    componente.enabled = false;
                }
            }

            await Fade(0f, 1f);
            float fim = Time.unscaledTime + duracaoJantar;
            while (Time.unscaledTime < fim)
                await ProximoFrame();

            await Fade(1f, 0f);
        }
        finally
        {
            Time.timeScale = escalaAnterior;
            if (telaPreta != null)
            {
                telaPreta.alpha = alphaAnterior;
                telaPreta.blocksRaycasts = bloqueava;
            }

            foreach (Behaviour componente in suspensos)
                if (componente != null)
                    componente.enabled = true;
        }
    }

    private async Task Fade(float inicio, float fim)
    {
        float comecou = Time.unscaledTime;
        while (Time.unscaledTime - comecou < duracaoFade)
        {
            float progresso = (Time.unscaledTime - comecou) / duracaoFade;
            telaPreta.alpha = Mathf.SmoothStep(inicio, fim, progresso);
            await ProximoFrame();
        }

        telaPreta.alpha = fim;
    }

    private async Task ProximoFrame()
    {
        int visita = SessaoCenario.Atual.Versao;
        int quadro = Time.frameCount;
        do
        {
            await Task.Yield();
            if (!SessaoCenario.Atual.Permite("Home", visita) || this == null || !isActiveAndEnabled)
                throw new OperationCanceledException();
        }
        while (Time.frameCount == quadro);
    }
}
