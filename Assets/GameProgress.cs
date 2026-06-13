using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    public bool falouAtendente = false;
    public bool falouPadaria = false;

    public bool seguindoAtendente = false;

    public static bool EstaEmDialogo { get; set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool PodeFalarComCaixa()
    {
        return falouAtendente && falouPadaria;
    }
}