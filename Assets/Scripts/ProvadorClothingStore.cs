using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public sealed class ProvadorClothingStore : MonoBehaviour
{
    [Header("Peças selecionáveis")]
    [SerializeField] private Collider[] camiseta;
    [SerializeField] private Collider[] camisa;
    [SerializeField] private Collider[] conjunto;
    [SerializeField] private Camera cameraDesktop;
    [SerializeField] private Transform origemRaioVR;
    [SerializeField] private LayerMask camadasRaio = ~0;
    [SerializeField, Min(0.1f)] private float alcance = 5f;
    [SerializeField] private GameObject avisoProvarRoupa;

    [Header("Transição")]
    [SerializeField] private Image imagemFade;
    [SerializeField, Min(0f)] private float duracaoFade = 0.35f;
    [SerializeField, Min(0f)] private float duracaoTelaPreta = 2f;
    [SerializeField] private Behaviour[] suspenderDuranteFade;

    private Collider[] pecasAtuais;
    private bool emTransicao;
    private bool jaExperimentou;

    internal bool Preparar(string nivel)
    {
        jaExperimentou = false;
        switch (nivel)
        {
            case "A1":
            case "A1-A2": pecasAtuais = camiseta; break;
            case "A2": pecasAtuais = camisa; break;
            default: pecasAtuais = conjunto; break;
        }

        Canvas canvas = imagemFade != null ? imagemFade.GetComponentInParent<Canvas>() : null;
        return isActiveAndEnabled && imagemFade != null && imagemFade.gameObject.activeInHierarchy
            && canvas != null && canvas.isActiveAndEnabled
            && (XRSettings.isDeviceActive ? origemRaioVR != null : cameraDesktop != null || Camera.main != null)
            && pecasAtuais != null && Array.Exists(pecasAtuais, peca => peca != null && peca.enabled && peca.gameObject.activeInHierarchy);
    }

    // Uma interação com a peça tem prioridade sobre o avanço da fala.
    internal async Task<bool> AguardarEscolha(bool podeAvancarDialogo, Action validarVisita = null)
    {
        int visita = SessaoCenario.Atual.Versao;
        bool avisoEstavaVisivel = avisoProvarRoupa != null && avisoProvarRoupa.activeSelf;
        if (avisoProvarRoupa != null)
            avisoProvarRoupa.SetActive(true);

        try
        {
            while (true)
            {
                bool acertouPeca = false;
                await BotaoConfiguracoes.AguardarComandoDialogo(Pressionado, validarVisita,
                    () => acertouPeca = AtingiuPeca());
                if (acertouPeca || podeAvancarDialogo)
                    return acertouPeca;
            }
        }
        finally
        {
            if (avisoProvarRoupa != null && SessaoCenario.Atual.Corresponde(visita))
                avisoProvarRoupa.SetActive(avisoEstavaVisivel);
        }
    }

    internal async Task AguardarLiberacaoDoClique()
    {
        await ProximoFrame();
        while (Pressionado())
            await ProximoFrame();
    }

    private static bool Pressionado()
    {
        if (!XRSettings.isDeviceActive)
            return Input.GetMouseButton(0);

        var controle = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        return controle.TryGetFeatureValue(CommonUsages.primaryButton, out bool pressionado) && pressionado;
    }

    private bool AtingiuPeca()
    {
        Ray raio;
        if (XRSettings.isDeviceActive)
        {
            if (origemRaioVR == null)
                return false;

            raio = new Ray(origemRaioVR.position, origemRaioVR.forward);
        }
        else
        {
            Camera camera = cameraDesktop != null ? cameraDesktop : Camera.main;
            if (camera == null)
                return false;

            raio = camera.ScreenPointToRay(Input.mousePosition);
        }

        return Physics.Raycast(raio, out RaycastHit hit, alcance, camadasRaio)
            && Array.IndexOf(pecasAtuais, hit.collider) >= 0;
    }

    internal async Task<bool> Experimentar()
    {
        if (emTransicao || jaExperimentou || !isActiveAndEnabled)
            return false;

        var componentesAtivos = new List<Behaviour>();
        float escalaAnterior = Time.timeScale;
        bool imagemAtiva = imagemFade.enabled;
        bool raycastAnterior = imagemFade.raycastTarget;
        Color corAnterior = imagemFade.color;
        Sprite spriteAnterior = imagemFade.overrideSprite;
        Sprite spriteBase = imagemFade.sprite;
        Material materialAnterior = imagemFade.material;
        emTransicao = true;

        try
        {
            Time.timeScale = 0f;
            if (suspenderDuranteFade != null)
            {
                foreach (Behaviour componente in suspenderDuranteFade)
                {
                    if (componente == null || componente == this || componente == imagemFade || !componente.enabled)
                        continue;

                    componentesAtivos.Add(componente);
                    componente.enabled = false;
                }
            }

            // Uma Image sem sprite ou material customizado cobre o Canvas com preto uniforme.
            imagemFade.color = Color.clear;
            imagemFade.sprite = null;
            imagemFade.overrideSprite = null;
            imagemFade.material = null;
            imagemFade.raycastTarget = true;
            imagemFade.enabled = true;
            await Fade(0f, 1f);
            float ate = Time.unscaledTime + duracaoTelaPreta;
            while (Time.unscaledTime < ate)
                await ProximoFrame();
            await Fade(1f, 0f);
        }
        finally
        {
            if (imagemFade != null)
            {
                imagemFade.color = corAnterior;
                imagemFade.sprite = spriteBase;
                imagemFade.overrideSprite = spriteAnterior != spriteBase ? spriteAnterior : null;
                imagemFade.material = materialAnterior;
                imagemFade.raycastTarget = raycastAnterior;
                imagemFade.enabled = imagemAtiva;
            }

            Time.timeScale = escalaAnterior;
            foreach (Behaviour componente in componentesAtivos)
                if (componente != null)
                    componente.enabled = true;

            emTransicao = false;
        }

        jaExperimentou = true;
        return true;
    }

    private async Task Fade(float inicio, float fim)
    {
        float comecou = Time.unscaledTime;
        while (Time.unscaledTime - comecou < duracaoFade)
        {
            float progresso = (Time.unscaledTime - comecou) / duracaoFade;
            imagemFade.color = new Color(0f, 0f, 0f, Mathf.SmoothStep(inicio, fim, progresso));
            await ProximoFrame();
        }

        imagemFade.color = new Color(0f, 0f, 0f, fim);
    }

    private async Task ProximoFrame()
    {
        int visita = SessaoCenario.Atual.Versao;
        await Task.Yield();
        if (!SessaoCenario.Atual.Permite("Clothing Store", visita) || this == null || !isActiveAndEnabled || imagemFade == null || !imagemFade.gameObject.activeInHierarchy)
            throw new OperationCanceledException();
    }
}
