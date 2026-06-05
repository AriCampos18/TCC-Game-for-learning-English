using UnityEngine;
using TMPro;

public class GeradorDeBlocos : MonoBehaviour
{
    public GameObject blocoPrefab;
    public Transform containerPalavras; // AreaPalavrasDisponiveis
    public Transform containerMontagem; // AreaDeMontagem

    public void CriarBlocosDasPalavras(string[] listaDePalavras)
    {
        // Remove blocos antigos de ambos os containers por segurança
        LimparContainers();

        // Cria novos blocos para cada palavra do vetor recebido
        foreach (string palavra in listaDePalavras)
        {
            GameObject novoBloco = Instantiate(blocoPrefab, containerPalavras);

            TextMeshProUGUI textoComponente = novoBloco.GetComponentInChildren<TextMeshProUGUI>();
            if (textoComponente != null)
            {
                textoComponente.text = palavra;
            }

            // Adiciona e configura o comportamento de clique no bloco
            BlocoItem itemScript = novoBloco.GetComponent<BlocoItem>();
            if (itemScript == null)
            {
                itemScript = novoBloco.AddComponent<BlocoItem>();
            }
            
            itemScript.Setup(containerPalavras, containerMontagem);
        }
    }

    public void LimparContainers()
    {
        foreach (Transform filho in containerPalavras) Destroy(filho.gameObject);
        foreach (Transform filho in containerMontagem) Destroy(filho.gameObject);
    }
}