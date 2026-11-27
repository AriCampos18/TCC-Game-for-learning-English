using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public sealed class VRVirtualKeyboard : MonoBehaviour
{
    private const string KeyPrefix = "Key_";
    private const string ShiftKey = "SHIFT";
    private const string BackspaceKey = "BACKSPACE";
    private const string SpaceKey = "SPACE";
    private const string ClearKey = "CLEAR";
    private const string DoneKey = "DONE";

    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private TMP_InputField[] allowedFields = new TMP_InputField[0];

    private readonly HashSet<TMP_InputField> allowedFieldSet = new HashSet<TMP_InputField>();
    private readonly Dictionary<TMP_InputField, UnityAction<string>> fieldHandlers =
        new Dictionary<TMP_InputField, UnityAction<string>>();
    private readonly Dictionary<Button, UnityAction> buttonHandlers =
        new Dictionary<Button, UnityAction>();

    private TMP_InputField focusedField;
    private GameObject ultimaSelecao;
    private bool shiftEnabled;
    private Func<bool> isVrActive = () => XRSettings.isDeviceActive;

    internal GameObject PainelPara(Transform origem) =>
        focusedField != null && focusedField.transform.IsChildOf(origem) ? keyboardPanel : null;

    private void Awake()
    {
        SetAllowedFields(allowedFields);
        if (keyboardPanel == gameObject && keyboardPanel.GetComponentsInChildren<Button>(true).Length == 0)
            CriarTeclas();
        BindKeyboardButtons();
    }

    internal void ObservarSelecao(Camera camera)
    {
        var selecao = EventSystem.current?.currentSelectedGameObject;
        if (selecao == ultimaSelecao) return;
        ultimaSelecao = selecao;
        var campo = selecao != null ? selecao.GetComponentInParent<TMP_InputField>() : null;
        if (campo != null && campo.isActiveAndEnabled && campo.IsInteractable() && !campo.readOnly)
        {
            if (focusedField != campo) AbrirCampo(campo);
            Posicionar(camera);
        }
        else if (selecao != null && keyboardPanel != null && !selecao.transform.IsChildOf(keyboardPanel.transform))
        {
            HideKeyboard();
        }
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<TMP_InputField, UnityAction<string>> pair in fieldHandlers)
        {
            if (pair.Key != null)
            {
                pair.Key.onSelect.RemoveListener(pair.Value);
            }
        }

        foreach (KeyValuePair<Button, UnityAction> pair in buttonHandlers)
        {
            if (pair.Key != null)
            {
                pair.Key.onClick.RemoveListener(pair.Value);
            }
        }
    }

    public void SetAllowedFields(params TMP_InputField[] fields)
    {
        foreach (KeyValuePair<TMP_InputField, UnityAction<string>> pair in fieldHandlers)
        {
            if (pair.Key != null)
            {
                pair.Key.onSelect.RemoveListener(pair.Value);
            }
        }

        fieldHandlers.Clear();
        allowedFieldSet.Clear();
        allowedFields = fields ?? new TMP_InputField[0];

        foreach (TMP_InputField field in allowedFields)
        {
            if (field == null || !allowedFieldSet.Add(field))
            {
                continue;
            }

            TMP_InputField capturedField = field;
            UnityAction<string> handler = value => FocusField(capturedField);
            field.onSelect.AddListener(handler);
            fieldHandlers.Add(field, handler);
        }

        if (focusedField != null && !allowedFieldSet.Contains(focusedField))
        {
            focusedField = null;
            HideKeyboard();
        }
    }

    public bool FocusField(TMP_InputField field)
    {
        if (!isVrActive())
        {
            HideKeyboard();
            return false;
        }

        if (field == null || !allowedFieldSet.Contains(field))
        {
            return false;
        }

        AbrirCampo(field);
        return true;
    }

    private void AbrirCampo(TMP_InputField campo)
    {
        focusedField = campo;
        shiftEnabled = false;
        if (keyboardPanel == gameObject)
            Posicionar(Camera.main);
        if (keyboardPanel != null) keyboardPanel.SetActive(true);
    }

    private void Posicionar(Camera camera)
    {
        if (camera == null || keyboardPanel != gameObject || transform is not RectTransform rect
            || !TryGetComponent<Canvas>(out var canvas))
            return;

        rect.SetParent(null, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
        rect.sizeDelta = new Vector2(520f, 234f);
        rect.localScale = Vector3.one * 0.0015f;
        canvas.worldCamera = camera;
        var vista = camera.transform;
        rect.SetPositionAndRotation(vista.position + vista.forward * 1.1f - vista.up * 0.32f, vista.rotation);
    }

    private void Update()
    {
        if (keyboardPanel != null && keyboardPanel.activeSelf
            && (!isVrActive() || focusedField == null || !focusedField.isActiveAndEnabled
                || !focusedField.IsInteractable() || focusedField.readOnly))
            HideKeyboard();
    }

    public void PressKey(string key)
    {
        if (focusedField == null || !focusedField.isActiveAndEnabled || !focusedField.IsInteractable()
            || focusedField.readOnly || string.IsNullOrEmpty(key))
        {
            return;
        }

        switch (key)
        {
            case ShiftKey:
                shiftEnabled = !shiftEnabled;
                return;
            case BackspaceKey:
                DeletePreviousCharacter();
                return;
            case SpaceKey:
                InsertText(" ");
                return;
            case "ENTER":
                if (focusedField.lineType == TMP_InputField.LineType.MultiLineNewline)
                    InsertText("\n");
                return;
            case ClearKey:
                focusedField.text = string.Empty;
                focusedField.caretPosition = 0;
                return;
            case DoneKey:
                focusedField = null;
                HideKeyboard();
                return;
            default:
                InsertText(shiftEnabled ? key.ToUpperInvariant() : key.ToLowerInvariant());
                return;
        }
    }

    public void HideKeyboard()
    {
        if (keyboardPanel != null && keyboardPanel.activeSelf)
        {
            keyboardPanel.SetActive(false);
        }

        focusedField = null;
        shiftEnabled = false;
    }

    private void BindKeyboardButtons()
    {
        if (keyboardPanel == null)
        {
            return;
        }

        Button[] buttons = keyboardPanel.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (!button.name.StartsWith(KeyPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            string key = button.name.Substring(KeyPrefix.Length);
            UnityAction handler = () => PressKey(key);
            button.onClick.AddListener(handler);
            buttonHandlers.Add(button, handler);
        }
    }

    private void CriarTeclas()
    {
        string[][] linhas =
        {
            new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "@", ".", "-", "_", "+" },
            new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "Á", "É", "Í", "Ó", "Ú" },
            new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ç", "Ã", "Õ", "Ê", "Ô", "Â" },
            new[] { "Z", "X", "C", "V", "B", "N", "M", ",", "?", "!", "'", "/", ":", "À", "Ü" },
            new[] { "#", "$", "%", "&", "*", "(", ")", "=", "[", "]", "{", "}", ";", "^", "~" },
            new[] { ShiftKey, SpaceKey, BackspaceKey, ClearKey, "ENTER", DoneKey }
        };
        if (TryGetComponent<Image>(out var fundo))
            fundo.color = new Color32(23, 29, 41, 255);
        for (int linha = 0; linha < linhas.Length; linha++)
        {
            var teclas = linhas[linha];
            float largura = 500f / teclas.Length;
            for (int coluna = 0; coluna < teclas.Length; coluna++)
            {
                string tecla = teclas[coluna];
                var objeto = new GameObject(KeyPrefix + tecla, typeof(RectTransform), typeof(Image), typeof(Button));
                var rect = (RectTransform)objeto.transform;
                rect.SetParent(transform, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(largura - 4f, 32f);
                rect.anchoredPosition = new Vector2(-250f + largura * (coluna + 0.5f), -22f - linha * 38f);
                objeto.GetComponent<Image>().color = new Color32(56, 70, 91, 255);

                var texto = new GameObject("Legenda", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                texto.rectTransform.SetParent(rect, false);
                texto.rectTransform.anchorMin = Vector2.zero;
                texto.rectTransform.anchorMax = Vector2.one;
                texto.rectTransform.offsetMin = texto.rectTransform.offsetMax = Vector2.zero;
                texto.text = tecla == BackspaceKey ? "DELETE" : tecla;
                texto.fontSize = 15f;
                texto.alignment = TextAlignmentOptions.Center;
                texto.raycastTarget = false;
            }
        }
    }

    private void InsertText(string value)
    {
        string currentText = focusedField.text ?? string.Empty;
        int caret = Mathf.Clamp(focusedField.caretPosition, 0, currentText.Length);
        if (focusedField.characterLimit > 0 && currentText.Length >= focusedField.characterLimit)
            return;
        if (focusedField.characterValidation == TMP_InputField.CharacterValidation.Digit
            || focusedField.characterValidation == TMP_InputField.CharacterValidation.Integer)
        {
            if (value.Length != 1 || value[0] < '0' || value[0] > '9') return;
        }
        if (focusedField.onValidateInput != null)
        {
            char validado = focusedField.onValidateInput(currentText, caret, value[0]);
            if (validado == '\0') return;
            value = validado.ToString();
        }
        focusedField.text = currentText.Insert(caret, value);
        focusedField.caretPosition = caret + value.Length;
    }

    private void DeletePreviousCharacter()
    {
        string currentText = focusedField.text ?? string.Empty;
        int caret = Mathf.Clamp(focusedField.caretPosition, 0, currentText.Length);
        if (caret == 0)
        {
            return;
        }

        focusedField.text = currentText.Remove(caret - 1, 1);
        focusedField.caretPosition = caret - 1;
    }
}
