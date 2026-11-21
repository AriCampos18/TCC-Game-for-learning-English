using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class InteracaoRestaurant : MonoBehaviour
{
    internal enum Etapa { Levantar, Retornar, Comer }

    [SerializeField] private RestaurantWaiterNPC garcom;
    [SerializeField] private Collider mesa;
    [SerializeField] private string checkpointKey;
    [SerializeField] private Collider prato;
    [SerializeField] private Transform jogador;
    [SerializeField] private Transform assento;
    [SerializeField] private Transform posicaoEmPe;
    [SerializeField] private CanvasGroup telaPreta;
    [SerializeField, Min(0f)] private float duracaoFade = 0.35f;
    [SerializeField, Min(0f)] private float duracaoRefeicao = 2f;
    [SerializeField] private Behaviour[] suspenderDuranteFade;
    [SerializeField] private Behaviour[] suspenderEnquantoSentado;

    private readonly List<Behaviour> suspensosNoAssento = new List<Behaviour>();

    internal static InteracaoRestaurant Pendente { get; private set; }
    private Etapa etapa;
    private Collider alvo;
    private bool confirmado;
    private bool sentado;
    private int quadroLiberado;
    private int visitaEtapa;

    private void Awake()
    {
        if (prato != null && prato != mesa)
            prato.enabled = false;
    }

    internal bool PertenceAoGarcom(RestaurantWaiterNPC candidato) => garcom == candidato;

    internal string ChaveCheckpoint => !string.IsNullOrEmpty(checkpointKey)
        && System.Text.RegularExpressions.Regex.IsMatch(checkpointKey, @"\A[a-zA-Z0-9_-]{1,64}\z")
        ? checkpointKey : null;

    internal bool Sentado => sentado;

    internal void RetomarPosicao(bool estavaSentado)
    {
        if (estavaSentado)
            Sentar();
        else
        {
            Encerrar();
            MoverJogador(posicaoEmPe);
        }
    }

    internal bool PodeSentar(Collider colisor) => mesa != null && colisor == mesa;

    internal void TentarSentar()
    {
        if (garcom != null && garcom.PodeIniciarAtendimento())
            garcom.IniciarAtendimento(this);
    }

    internal void Sentar()
    {
        if (mesa == null || prato == null || mesa == prato || telaPreta == null || !telaPreta.gameObject.activeInHierarchy)
            throw new InvalidOperationException("Configure mesa, prato e a tela preta do Restaurant.");

        MoverJogador(assento);
        sentado = true;
        Suspender(suspenderEnquantoSentado, suspensosNoAssento);
    }

    internal async Task Aguardar(Etapa proxima)
    {
        bool deveEstarSentado = proxima != Etapa.Retornar;
        Collider colisor = proxima == Etapa.Comer ? prato : mesa;
        if (!isActiveAndEnabled || Pendente != null || sentado != deveEstarSentado
            || colisor == null || (proxima != Etapa.Comer && !colisor.enabled) || !colisor.gameObject.activeInHierarchy)
            throw new InvalidOperationException($"Interação do Restaurant indisponível: {proxima}.");

        etapa = proxima;
        alvo = colisor;
        if (proxima == Etapa.Comer)
            colisor.enabled = true;
        confirmado = false;
        quadroLiberado = Time.frameCount;
        visitaEtapa = SessaoCenario.Atual.Versao;
        Pendente = this;
        try
        {
            while (!confirmado)
            {
                await ProximoFrame();
                if (alvo == null || !alvo.enabled || !alvo.gameObject.activeInHierarchy)
                    throw new InvalidOperationException($"Collider indisponível durante a etapa {etapa}.");
            }
        }
        finally
        {
            if (Pendente == this)
                Pendente = null;
            alvo = null;
            if (proxima == Etapa.Comer && colisor != null)
                colisor.enabled = false;
        }

        if (proxima == Etapa.Levantar)
        {
            MoverJogador(posicaoEmPe);
            sentado = false;
            Restaurar(suspensosNoAssento);
        }
        else if (proxima == Etapa.Retornar)
        {
            MoverJogador(assento);
            sentado = true;
            Suspender(suspenderEnquantoSentado, suspensosNoAssento);
        }
    }

    internal void TentarConfirmar(Collider colisor)
    {
        if (SessaoCenario.Atual.Permite("Restaurant", visitaEtapa) && Pendente == this && !confirmado && colisor == alvo && Time.frameCount > quadroLiberado)
            confirmado = true;
    }

    internal void Encerrar()
    {
        sentado = false;
        Restaurar(suspensosNoAssento);
    }

    private void Suspender(Behaviour[] componentes, List<Behaviour> suspensos)
    {
        if (componentes == null)
            return;

        foreach (Behaviour item in componentes)
        {
            if (item == null || !item.enabled || item == this || item == telaPreta)
                continue;

            suspensos.Add(item);
            item.enabled = false;
        }
    }

    private static void Restaurar(List<Behaviour> suspensos)
    {
        foreach (Behaviour item in suspensos)
            if (item != null)
                item.enabled = true;

        suspensos.Clear();
    }

    private void MoverJogador(Transform destino)
    {
        if (jogador == null || destino == null)
            throw new InvalidOperationException("Configure o jogador, o assento e a posição em pé no Restaurant.");

        CharacterController controlador = jogador.GetComponent<CharacterController>();
        bool habilitado = controlador != null && controlador.enabled;
        if (habilitado)
            controlador.enabled = false;
        try
        {
            jogador.position = destino.position;
        }
        finally
        {
            if (habilitado)
                controlador.enabled = true;
        }
    }

    internal async Task SimularRefeicao()
    {
        if (telaPreta == null || !telaPreta.gameObject.activeInHierarchy)
            throw new InvalidOperationException("A tela preta do Restaurant precisa estar ativa.");

        float escalaAnterior = Time.timeScale;
        float alphaAnterior = telaPreta.alpha;
        bool bloqueava = telaPreta.blocksRaycasts;
        var suspensos = new List<Behaviour>();
        try
        {
            Time.timeScale = 0f;
            telaPreta.blocksRaycasts = true;
            Suspender(suspenderDuranteFade, suspensos);

            await Fade(0f, 1f);
            float fim = Time.unscaledTime + duracaoRefeicao;
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
            Restaurar(suspensos);
        }
    }

    private async Task Fade(float inicio, float fim)
    {
        float comecou = Time.unscaledTime;
        while (Time.unscaledTime - comecou < duracaoFade)
        {
            float t = (Time.unscaledTime - comecou) / duracaoFade;
            telaPreta.alpha = Mathf.SmoothStep(inicio, fim, t);
            await ProximoFrame();
            if (telaPreta == null || !telaPreta.gameObject.activeInHierarchy)
                throw new OperationCanceledException();
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
            if (!SessaoCenario.Atual.Permite("Restaurant", visita) || this == null || !isActiveAndEnabled || MissionManager.Instance?.UsaCatalogoRestaurant != true)
                throw new OperationCanceledException();
        }
        while (Time.frameCount == quadro);
    }
}
