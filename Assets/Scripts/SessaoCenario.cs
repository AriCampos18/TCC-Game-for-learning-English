// A identidade da visita muda desde a seleção, antes de qualquer carregamento.
internal sealed class SessaoCenario
{
    internal static readonly SessaoCenario Atual = new SessaoCenario();

    internal string Nome { get; private set; }
    internal string Nivel { get; private set; }
    internal int Versao { get; private set; }
    internal bool EmJogo { get; private set; }
    internal bool EmResultado { get; private set; }
    internal bool Bloqueada { get; set; }

    internal static bool Conhecido(string nome) => Id(nome) != 0;

    internal static int Id(string nome)
    {
        switch (nome)
        {
            case "Market": return 1;
            case "Airport": return 2;
            case "Clothing Store": return 3;
            case "Hotel": return 4;
            case "Home": return 5;
            case "Visitor Center": return 6;
            case "Restaurant": return 7;
            case "College": return 8;
            default: return 0;
        }
    }

    internal static string CenarioDoNpc(string npc)
    {
        switch (npc)
        {
            case "shelf_npc": case "bakery_npc": case "cashier_npc": return "Market";
            case "airport_info_npc": case "checkin_npc": return "Airport";
            case "clothing_seller_npc": return "Clothing Store";
            case "recepcionist_hotel_npc": return "Hotel";
            case "mother_npc": case "father_npc": case "brother_npc": return "Home";
            case "visitor_center_npc": return "Visitor Center";
            case "restaurant_waiter_npc": case "restaurant_client": return "Restaurant";
            case "recepcionist_college_npc": case "secretary_college_npc": return "College";
            default: return null;
        }
    }

    internal int Preparar(string nome, string nivel)
    {
        if (Bloqueada || EmJogo || EmResultado || !Conhecido(nome)
            || (nivel != "A1" && nivel != "A1-A2" && nivel != "A2" && nivel != "A2-B1" && nivel != "B1"))
            return 0;

        Nome = nome;
        Nivel = nivel;
        return ++Versao;
    }

    internal bool Corresponde(int versao) => Nome != null && Versao == versao;
    internal bool Permite(string nome) => !Bloqueada && EmJogo && Nome == nome;
    internal bool Permite(string nome, int versao) => Corresponde(versao) && Permite(nome);

    internal bool Iniciar(int versao)
    {
        if (Bloqueada || !Corresponde(versao) || EmJogo || EmResultado)
            return false;

        EmJogo = true;
        return true;
    }

    internal bool MostrarResultado(int versao)
    {
        if (Bloqueada || !Corresponde(versao) || !EmJogo)
            return false;

        EmJogo = false;
        EmResultado = true;
        return true;
    }

    internal void Encerrar()
    {
        ++Versao;
        Nome = null;
        Nivel = null;
        EmJogo = false;
        EmResultado = false;
    }
}
