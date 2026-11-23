using System;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal sealed class RecuperacaoSenhaUI
{
    private const string Indisponivel = "Recuperação indisponível. O serviço de códigos por e-mail ainda não foi configurado.";
    private readonly GameObject email, codigo, senha;
    private readonly TMP_InputField endereco;
    private readonly TMP_Text mensagem;
    private readonly Action voltar;

    internal RecuperacaoSenhaUI(GameObject origem, Action voltar)
    {
        this.voltar = voltar;
        email = VinculosModaisTcc.Painel(origem, "ModalEsqueciSenha");
        codigo = VinculosModaisTcc.Painel(origem, "ModalCodigoEmail");
        senha = VinculosModaisTcc.Painel(origem, "ModalNovaSenha");
        endereco = VinculosModaisTcc.Filho<TMP_InputField>(email, "EmailUsername");
        mensagem = VinculosModaisTcc.Filho<TMP_Text>(email, "TxtMensagens");
        foreach (var painel in new[] { email, codigo, senha })
        {
            if (painel == null) continue;
            var seguir = VinculosModaisTcc.Filho<Button>(painel, "BotaoContinuar");
            if (seguir != null) seguir.interactable = false;
            VinculosModaisTcc.Ligar(VinculosModaisTcc.Filho<Button>(painel, "BotaoVoltarLogin"), Voltar);
            VinculosModaisTcc.Texto(VinculosModaisTcc.Filho<TMP_Text>(painel, "TxtMensagens"), Indisponivel);
        }
        if (endereco != null)
            endereco.onValueChanged.AddListener(ValidarEmail);
        Limpar();
    }

    internal bool Abrir()
    {
        Limpar();
        if (email == null || endereco == null || mensagem == null)
            return false;
        ValidarEmail("");
        email.SetActive(true);
        return true;
    }

    private void ValidarEmail(string valor)
    {
        string texto = valor.Trim();
        bool valido = false;
        if (texto.Length > 0)
        {
            try { valido = string.Equals(texto, new MailAddress(texto).Address, StringComparison.OrdinalIgnoreCase); }
            catch (FormatException) { }
        }
        VinculosModaisTcc.Texto(mensagem, texto.Length != 0 && !valido
            ? "Informe um endereço de e-mail válido. " + Indisponivel : Indisponivel);
    }

    internal void Limpar()
    {
        foreach (var painel in new[] { email, codigo, senha })
        {
            if (painel == null) continue;
            foreach (var campo in painel.GetComponentsInChildren<TMP_InputField>(true))
                campo.SetTextWithoutNotify("");
            painel.SetActive(false);
        }
    }

    private void Voltar() => voltar();
}
