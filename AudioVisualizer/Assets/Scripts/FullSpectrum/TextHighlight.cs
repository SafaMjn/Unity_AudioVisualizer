using TMPro;
using UnityEngine;

public class TextHighlight : MonoBehaviour
{
    public Color highlightColor;
    public Color defaultColor;
    public TMP_Text text;

    public void HighlightText()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.color = highlightColor;
    }

    public void ResetText()
    {
        text = GetComponentInChildren<TMP_Text>();
        text.color = defaultColor;
    }
}
