using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    private bool shiftEnabled;

    private void Awake()
    {
        SetAllowedFields(allowedFields);
        BindKeyboardButtons();
        HideKeyboard();
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
        if (field == null || !allowedFieldSet.Contains(field))
        {
            return false;
        }

        focusedField = field;
        shiftEnabled = false;
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(true);
        }

        return true;
    }

    public void PressKey(string key)
    {
        if (focusedField == null || !allowedFieldSet.Contains(focusedField) || string.IsNullOrEmpty(key))
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

    private void InsertText(string value)
    {
        string currentText = focusedField.text ?? string.Empty;
        int caret = Mathf.Clamp(focusedField.caretPosition, 0, currentText.Length);
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
