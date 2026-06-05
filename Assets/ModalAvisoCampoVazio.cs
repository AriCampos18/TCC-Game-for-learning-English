using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalAvisoCampoVazio : MonoBehaviour
{
    public Button botaoOK;

    void Start()
    {
        botaoOK.onClick.AddListener(ClickFechar);
    }

    void ClickFechar()
    {
        this.gameObject.SetActive(false);
    }
}
