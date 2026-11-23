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
