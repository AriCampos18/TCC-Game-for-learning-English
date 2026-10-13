using System;
using UnityEngine;

public class HorarioIluminacao : MonoBehaviour
{
    public Light directionalLight;

    void Start()
    {
        AplicarHorario();
    }

    void AplicarHorario()
    {
        if (directionalLight == null) return;

        int hora = DateTime.Now.Hour;
        Debug.Log($"Horário detectado no PC: {hora}h");

        // MANHÃ
        if (hora >= 6 && hora < 12)
        {
            directionalLight.intensity = 0.8f;
            directionalLight.color = new Color(1f, 0.9f, 0.7f);
            directionalLight.transform.rotation = Quaternion.Euler(20f, 50f, 0f);
        }
        // TARDE
        else if (hora >= 12 && hora < 18)
        {
            directionalLight.intensity = 1.3f;
            directionalLight.color = Color.white;
            directionalLight.transform.rotation = Quaternion.Euler(45f, 135f, 0f);
        }
        // NOITE
        else
        {
            directionalLight.intensity = 0.1f;
            directionalLight.color = new Color(0.2f, 0.25f, 0.45f);
            directionalLight.transform.rotation = Quaternion.Euler(-20f, 135f, 0f);
        }

        // Atualiza a iluminação global (isso aqui já basta)
        DynamicGI.UpdateEnvironment();
    }
}