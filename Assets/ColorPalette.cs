using UnityEngine;
using UnityEngine.UI;

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

    private void SetColor(Color color)
    {
        colorDisplay.color = currentColor = color;
        
        Debug.Log("color updated");
    }
}
