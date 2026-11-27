using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal static class VinculosModaisTcc
{
    internal static GameObject Painel(GameObject origem, string nome)
    {
        var candidatos = origem.scene.GetRootGameObjects()
            .SelectMany(r => r.GetComponentsInChildren<Transform>(true))
            .Where(t => t.name == nome).Take(2).ToArray();
        return candidatos.Length == 1 ? candidatos[0].gameObject : null;
    }

    internal static T Filho<T>(GameObject raiz, string nome) where T : Component =>
        raiz == null ? null : raiz.GetComponentsInChildren<T>(true).FirstOrDefault(c => c.gameObject.name == nome);

    internal static void Texto(TMP_Text texto, string valor)
    {
        if (texto == null) return;
        texto.richText = false;
        texto.text = valor ?? "";
    }

    internal static bool Ligar(Button botao, UnityEngine.Events.UnityAction acao)
    {
        if (botao == null) return false;
        if (botao.onClick.GetPersistentEventCount() != 0)
        {
            Debug.LogWarning("O botão " + botao.name + " já possui um vínculo no Inspector.", botao);
            return false;
        }
        botao.onClick.AddListener(acao);
        return true;
    }

    internal static GameObject Prefab(GameObject atual, string nome)
    {
#if UNITY_EDITOR
        if (atual == null)
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + nome + ".prefab");
#endif
        return atual;
    }
}

internal sealed class AvisoCompacto : IDisposable
{
    private static AvisoCompacto atual;
    private readonly GameObject painel, titulo, botao;
    private readonly TMP_Text mensagem;
    private readonly RectTransform rect, rectMensagem;
    private readonly Vector2 ancoraMin, ancoraMax, tamanho, posicao;
    private readonly Vector2 ancoraMinMensagem, ancoraMaxMensagem, tamanhoMensagem, posicaoMensagem;
    private readonly VerticalLayoutGroup layout;
    private readonly bool layoutAtivo, autoSize;
    private readonly Vector4 margensMensagem;
    private readonly float tamanhoFonte;
    private readonly float fonteMinima, fonteMaxima;
    private readonly Graphic[] alvosDeRaio;
    private readonly TextWrappingModes quebraTexto;
    private readonly bool tituloAtivo, botaoAtivo, mensagemAtiva, richText;
    private readonly int indice;
    private readonly string texto;
    private readonly TextAlignmentOptions alinhamento;
    private bool encerrado;
    private ModoJogoManager apresentacaoAutenticacao;

    internal static AvisoCompacto AbrirAutenticacao(GameObject origem, string texto)
    {
        var aviso = AbrirNoPainel(VinculosModaisTcc.Painel(origem, "ModalErroAviso"), texto, inferior: false);
        var modo = ModoJogoManager.Instance;
        if (aviso != null && modo != null)
        {
            aviso.apresentacaoAutenticacao = modo;
            modo.AproximarPainelAutenticacaoRV(aviso.rect);
        }
        return aviso;
    }

    internal static AvisoCompacto AbrirNoPainel(GameObject painel, string texto, bool inferior)
    {
        var titulo = painel?.transform.Find("Titulo");
        var botao = painel?.transform.Find("Ok");
        var mensagem = VinculosModaisTcc.Filho<TMP_Text>(painel, "MsgErroAviso");
        if (painel == null || painel.activeSelf || titulo == null || botao == null || mensagem == null
            || painel.transform is not RectTransform)
            return null;

        atual?.Dispose();
        atual = new AvisoCompacto(painel, titulo.gameObject, botao.gameObject, mensagem, texto, inferior);
        return atual;
    }

    internal static void EncerrarNoPainel(GameObject painel)
    {
        if (atual != null && atual.painel == painel)
            atual.Dispose();
    }

    private AvisoCompacto(GameObject painel, GameObject titulo, GameObject botao, TMP_Text mensagem,
        string textoAviso, bool inferior)
    {
        this.painel = painel;
        this.titulo = titulo;
        this.botao = botao;
        this.mensagem = mensagem;
        rect = (RectTransform)painel.transform;
        rectMensagem = mensagem.rectTransform;
        ancoraMin = rect.anchorMin;
        ancoraMax = rect.anchorMax;
        tamanho = rect.sizeDelta;
        posicao = rect.anchoredPosition;
        ancoraMinMensagem = rectMensagem.anchorMin;
        ancoraMaxMensagem = rectMensagem.anchorMax;
        tamanhoMensagem = rectMensagem.sizeDelta;
        posicaoMensagem = rectMensagem.anchoredPosition;
        layout = painel.GetComponent<VerticalLayoutGroup>();
        layoutAtivo = layout != null && layout.enabled;
        autoSize = mensagem.enableAutoSizing;
        tamanhoFonte = mensagem.fontSize;
        fonteMinima = mensagem.fontSizeMin;
        fonteMaxima = mensagem.fontSizeMax;
        margensMensagem = mensagem.margin;
        quebraTexto = mensagem.textWrappingMode;
        tituloAtivo = titulo.activeSelf;
        botaoAtivo = botao.activeSelf;
        mensagemAtiva = mensagem.gameObject.activeSelf;
        richText = mensagem.richText;
        texto = mensagem.text;
        alinhamento = mensagem.alignment;
        indice = painel.transform.GetSiblingIndex();
        alvosDeRaio = inferior
            ? painel.GetComponentsInChildren<Graphic>(true).Where(grafico => grafico.raycastTarget).ToArray()
            : Array.Empty<Graphic>();

        foreach (var alvo in alvosDeRaio)
            alvo.raycastTarget = false;
        titulo.SetActive(false);
        botao.SetActive(false);
        mensagem.gameObject.SetActive(true);
        if (layout != null) layout.enabled = false;
        rectMensagem.anchorMin = Vector2.zero;
        rectMensagem.anchorMax = Vector2.one;
        rectMensagem.offsetMin = new Vector2(20f, 12f);
        rectMensagem.offsetMax = -rectMensagem.offsetMin;
        mensagem.margin = Vector4.zero;
        mensagem.enableAutoSizing = false;
        mensagem.fontSize = 26f;
        mensagem.textWrappingMode = TextWrappingModes.NoWrap;
        if (inferior)
        {
            mensagem.enableAutoSizing = true;
            mensagem.fontSizeMin = 20f;
            mensagem.fontSizeMax = 26f;
            mensagem.textWrappingMode = TextWrappingModes.Normal;
        }
        mensagem.alignment = TextAlignmentOptions.Center;
        VinculosModaisTcc.Texto(mensagem, textoAviso);
        var canvas = painel.GetComponentInParent<Canvas>(true);
        bool overlay = canvas != null && canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
        var centro = new Vector2(0.5f, inferior && overlay ? 0.15f : 0.5f);
        var meiaFracaoDesktop = new Vector2(0.175f, 0.075f);
        rect.anchorMin = overlay ? centro - meiaFracaoDesktop : centro;
        rect.anchorMax = overlay ? centro + meiaFracaoDesktop : centro;
        rect.sizeDelta = overlay ? Vector2.zero : new Vector2(340f, 108f);
        rect.anchoredPosition = inferior && !overlay ? new Vector2(0f, -135f) : Vector2.zero;
        painel.transform.SetAsLastSibling();
        painel.SetActive(true);
    }

    public void Dispose()
    {
        if (encerrado) return;
        encerrado = true;
        if (atual == this) atual = null;
        if (painel == null) return;
        painel.SetActive(false);
        if (apresentacaoAutenticacao != null)
            apresentacaoAutenticacao.RestaurarPainelAutenticacaoRV(rect);
        rect.anchorMin = ancoraMin;
        rect.anchorMax = ancoraMax;
        rect.sizeDelta = tamanho;
        rect.anchoredPosition = posicao;
        painel.transform.SetSiblingIndex(indice);
        titulo.SetActive(tituloAtivo);
        botao.SetActive(botaoAtivo);
        mensagem.gameObject.SetActive(mensagemAtiva);
        mensagem.richText = richText;
        mensagem.text = texto;
        mensagem.alignment = alinhamento;
        mensagem.margin = margensMensagem;
        mensagem.enableAutoSizing = autoSize;
        mensagem.fontSize = tamanhoFonte;
        mensagem.fontSizeMin = fonteMinima;
        mensagem.fontSizeMax = fonteMaxima;
        mensagem.textWrappingMode = quebraTexto;
        rectMensagem.anchorMin = ancoraMinMensagem;
        rectMensagem.anchorMax = ancoraMaxMensagem;
        rectMensagem.sizeDelta = tamanhoMensagem;
        rectMensagem.anchoredPosition = posicaoMensagem;
        if (layout != null) layout.enabled = layoutAtivo;
        foreach (var alvo in alvosDeRaio)
            if (alvo != null) alvo.raycastTarget = true;
    }
}
