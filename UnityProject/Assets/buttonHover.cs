using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonHover : MonoBehaviour
{
    public Image buttonSpanImage;
    public Color originalColor;
    public Color hoverColor;
    private Button button;
    private ColorBlock colorBlock;
    private Color originalColorBlock;
    private Color hoverColorBlock;
    // Start is called before the first frame update
    void Start()
    {
        buttonSpanImage = GetComponent<Image>();
        originalColor = buttonSpanImage.color;
        button = GetComponent<Button>();
        colorBlock = button.colors;
        originalColorBlock = colorBlock.selectedColor;
        hoverColorBlock = colorBlock.highlightedColor;
    }

    public void changeWhenHover()
    {
        buttonSpanImage.color = hoverColor;
        colorBlock.selectedColor = hoverColorBlock;
        button.colors = colorBlock;
    }

    public void changeWhenNotHover()
    {
        buttonSpanImage.color = originalColor;
        // change button color back to original color
        colorBlock.selectedColor = originalColorBlock;
        button.colors = colorBlock;
    }
}
