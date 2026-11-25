using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

internal static class EstadoInicialModaisTcc
{
    private const string CenaPrincipal =
        "Assets/Versatile Studio Assets/Demo City By Versatile Studio/Scenes/demo_city_night.unity";

    private static readonly HashSet<Scene> cenasPreparadas = new();

    private static readonly string[] PaineisInterface =
    {
        "AuthenticationUI/ModalLogin", "AuthenticationUI/ModalRegister", "VirtualKeyboard",
        "ModalAvisoErroLoginRegister", "ModalEsqueciSenha", "ModalCodigoEmail", "ModalNovaSenha",
        "ModalEscolhaCenario", "ModalConfiguracoes", "Configuracoes", "Crosshair",
        "ModalGerenciarExercicios", "ModalFormFalaNPC", "ModalSelecionarTipoEx", "ModalSelecionarExFala",
        "FormWriting", "FormularioSpeaking", "FormularioAlternativas", "FormularioCriarFrase",
        "FormularioCompletarFrase", "FormularioAssociacao", "ModalErroAviso"
    };

    private static readonly string[] PaineisJogo =
    {
        "ModalBoasVindas", "ModalNivelamento", "ModalRespostaNivelamento", "ModalInstrucoes",
        "ModalExercíciosPanel", "ModalFinal", "ModalErroSpeaking", "ModalAvisoCamposVazio",
        "ModalAvisoRevisao", "ModalEsperaNivelamento", "ModalAvisoInteracaoMouse",
        "ModalLegenda", "ModalLegendaRepeat", "BarraProgressao", "ModalMissoes"
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Registrar()
    {
        cenasPreparadas.Clear();
        SceneManager.sceneLoaded -= AoCarregar;
        SceneManager.sceneLoaded += AoCarregar;
        SceneManager.sceneUnloaded -= AoDescarregar;
        SceneManager.sceneUnloaded += AoDescarregar;
        Preparar(SceneManager.GetActiveScene());
    }

    private static void AoCarregar(Scene cena, LoadSceneMode _) => Preparar(cena);

    private static void AoDescarregar(Scene cena) => cenasPreparadas.Remove(cena);

    private static void Preparar(Scene cena)
    {
        if (cena.path != CenaPrincipal || cenasPreparadas.Contains(cena))
            return;

        var autenticacao = ComponenteNaCena<AuthenticationUIController>(cena);
        var final = ComponenteNaCena<ModalFinalManager>(cena);
        if (autenticacao == null || final == null)
            return;

        cenasPreparadas.Add(cena);
        var interfaceInicial = autenticacao.transform.parent;
        var interfaceJogo = final.transform.parent;
        FecharPaineis(interfaceInicial, PaineisInterface);
        FecharPaineis(interfaceJogo, PaineisJogo);

        var exercicio = ComponenteNaCena<ModalExercicio>(cena);
        if (exercicio != null && exercicio.botaoRepetirFalaNPC != null)
            exercicio.botaoRepetirFalaNPC.gameObject.SetActive(false);

        var tooltip = ComponenteNaCena<TooltipScript>(cena);
        if (tooltip != null)
            tooltip.PrepararEstadoInicial();

        AtivarHierarquia(interfaceJogo);
        AtivarHierarquia(autenticacao.transform);
        AtivarComponenteNaCena<EventSystem>(cena);
        AtivarComponenteNaCena<ModoJogoManager>(cena);
        AtivarComponenteNaCena<GerenciadorProgressao>(cena);
        AtivarComponenteNaCena<MissionManager>(cena);
    }

    private static T ComponenteNaCena<T>(Scene cena) where T : Component
    {
        foreach (var componente in Object.FindObjectsByType<T>(FindObjectsInactive.Include))
            if (componente.gameObject.scene == cena)
                return componente;
        return null;
    }

    private static void FecharPaineis(Transform raiz, string[] paineis)
    {
        foreach (var caminho in paineis)
        {
            var painel = raiz.Find(caminho);
            if (painel != null)
                painel.gameObject.SetActive(false);
            else
                Debug.LogWarning($"Estado inicial dos modais: painel '{caminho}' ausente.", raiz);
        }
    }

    private static void AtivarComponenteNaCena<T>(Scene cena) where T : Component
    {
        var componente = ComponenteNaCena<T>(cena);
        if (componente != null)
            AtivarHierarquia(componente.transform);
    }

    private static void AtivarHierarquia(Transform objeto)
    {
        if (objeto.parent != null)
            AtivarHierarquia(objeto.parent);
        objeto.gameObject.SetActive(true);
    }
}
