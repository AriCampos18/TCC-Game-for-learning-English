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
        Instance = this;
    }

    public bool PodeFalarComCaixa()
    {
        return falouAtendente && falouPadaria;
    }
}