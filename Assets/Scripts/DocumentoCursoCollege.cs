using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class DocumentoCursoCollege : MonoBehaviour
{
    [SerializeField] private Button botaoAbrir;
    [SerializeField] private GameObject painel;
    [SerializeField] private TMP_Text textoDocumento;
    [SerializeField] private Button botaoFechar;

    private int visita;

    internal bool Disponivel { get; private set; }
    internal bool Consultado { get; private set; }
    internal bool Aberto => painel != null && painel.activeInHierarchy;
    internal int UltimoQuadroInteracao { get; private set; } = -1;

    private void Awake()
    {
        if (botaoAbrir != null)
            botaoAbrir.onClick.AddListener(Abrir);
        if (botaoFechar != null)
            botaoFechar.onClick.AddListener(Fechar);
        Resetar();
    }

    private void OnDestroy()
    {
        if (botaoAbrir != null)
            botaoAbrir.onClick.RemoveListener(Abrir);
        if (botaoFechar != null)
            botaoFechar.onClick.RemoveListener(Fechar);
    }

    internal void Resetar()
    {
        Disponivel = false;
        Consultado = false;
        if (painel != null)
            painel.SetActive(false);
        MostrarBotao(false);
    }

    internal bool Disponibilizar(string nivel)
    {
        if (!SessaoCenario.Atual.Permite("College") || !isActiveAndEnabled || botaoAbrir == null || painel == null || textoDocumento == null || botaoFechar == null
            || botaoAbrir.transform.IsChildOf(painel.transform)
            || !botaoFechar.transform.IsChildOf(painel.transform)
            || !textoDocumento.transform.IsChildOf(painel.transform)
            || (painel.transform.parent != null && !painel.transform.parent.gameObject.activeInHierarchy))
            return false;

        string data;
        string sala;
        if (nivel == "A1-A2")
        {
            data = "February 10";
            sala = "Room 204, second floor";
        }
        else if (nivel == "A2-B1")
        {
            data = "February 15";
            sala = "Room 325, third floor";
        }
        else
            return false;

        textoDocumento.text = $"Course: Computer Science\nTuition: R$ 500 per month\nClasses start: {data}\nFirst class: {sala}";
        visita = SessaoCenario.Atual.Versao;
        Disponivel = true;
        return true;
    }

    internal void MostrarBotao(bool mostrar)
    {
        if (botaoAbrir != null)
            botaoAbrir.gameObject.SetActive(mostrar && Disponivel && !Aberto);
    }

    internal bool PodeAguardarConsulta => SessaoCenario.Atual.Permite("College", visita) && isActiveAndEnabled && Disponivel
        && ((botaoAbrir != null && botaoAbrir.isActiveAndEnabled && botaoAbrir.IsInteractable())
            || (Aberto && botaoFechar != null && botaoFechar.isActiveAndEnabled && botaoFechar.IsInteractable()));

    private void Abrir()
    {
        if (!SessaoCenario.Atual.Permite("College", visita) || !isActiveAndEnabled || !Disponivel || Aberto || !botaoAbrir.isActiveAndEnabled)
            return;

        painel.SetActive(true);
        if (!Aberto)
            return;

        UltimoQuadroInteracao = Time.frameCount;
        MostrarBotao(false);
        if (!Consultado)
        {
            Consultado = true;
            MissionManager.Instance?.RegistrarEventoCollege(MissoesCollege.Marco, MissoesCollege.DocumentoConsultado);
        }
    }

    private void Fechar()
    {
        if (!Aberto)
            return;

        UltimoQuadroInteracao = Time.frameCount;
        painel.SetActive(false);
        MostrarBotao(true);
    }
}
