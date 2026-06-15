using UnityEngine;
using UnityEngine.UI;

public class FlowLayoutGroup : LayoutGroup
{
    public float spacingX = 6f;
    public float spacingY = 6f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        Organizar();
    }

    public override void CalculateLayoutInputVertical()
    {
        Organizar();
    }

    public override void SetLayoutHorizontal()
    {
        Organizar();
    }

    public override void SetLayoutVertical()
    {
        Organizar();
    }

    private void Organizar()
    {
        float larguraDisponivel = rectTransform.rect.width - padding.left - padding.right;

        float x = padding.left;
        float y = padding.top;
        float alturaLinha = 0f;

        foreach (RectTransform child in rectChildren)
        {
            if (!child.gameObject.activeSelf)
                continue;

            float largura = LayoutUtility.GetPreferredWidth(child);
            float altura = LayoutUtility.GetPreferredHeight(child);

            if (x + largura > larguraDisponivel && x > padding.left)
            {
                x = padding.left;
                y += alturaLinha + spacingY;
                alturaLinha = 0f;
            }

            SetChildAlongAxis(child, 0, x, largura);
            SetChildAlongAxis(child, 1, y, altura);

            x += largura + spacingX;
            alturaLinha = Mathf.Max(alturaLinha, altura);
        }
    }
}