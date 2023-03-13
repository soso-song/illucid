using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonHover : MonoBehaviour
{
    public Image buttonSpanImage;
    public Color originalColor;
    public Color hoverColor;
    // Start is called before the first frame update
    void Start()
    {
        buttonSpanImage = GetComponent<Image>();
        originalColor = buttonSpanImage.color;
    }

    public void changeWhenHover()
    {
        buttonSpanImage.color = hoverColor;
    }

    public void changeWhenNotHover()
    {
        buttonSpanImage.color = originalColor;
    }
}
