using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ColorPalette : MonoBehaviour
{
    public Image colorDisplay;
    public Color currentColor;

    public Button[] colorButtons;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var button in colorButtons)
        {
            Color buttonColor = button.image.color;
            button.onClick.AddListener(()=> SetColor(buttonColor));
        }
    }

    private void Awake()
    {
        currentColor = Color.white;
    }

    private void SetColor(Color color)
    {
        colorDisplay.color = currentColor = color;
        ToolManager.SetActiveTool(ToolManager.ColorBrushInstance);
        
        Debug.Log("color updated");
    }
}
