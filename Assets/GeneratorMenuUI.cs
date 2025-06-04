using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GeneratorMenuUI : MonoBehaviour
{
    
    public GameObject PanelButton;
    public Sprite Panel_Idle;
    public Sprite Panel_Active;

    public GameObject SweaterButton;
    public Sprite Sweater_Idle;
    public Sprite Sweater_Active;

    public GameObject SweaterMenu;

    public GameObject PanelMenu;

    [FormerlySerializedAs("Menu")] public GameObject GenerationMenu;

    public GameObject PreviewWindow;

    public GameObject GaugeWindow;

    public GameObject GenerationPromptButton;

    public GameObject[] buttons;

    private bool PanelBool;


    public Slider PanelWidth;
    public Slider PanelHeight;

    public Slider BodyWidth;
    public Slider BodyLength;
    public Slider SleeveWidth;
    public Slider SleeveLength;
    public Slider CollarWidth;
    public Slider CollarHeight;

    public Slider GaugeHorizontal;
    public Slider GaugeVertical;

    public SweaterGenerator SweaterGenerator;

    public InputHandler InputHandler;

    public SweaterPreview SweaterPreview;

    private bool preview;
    
    

    private void Start()
    {
        GenerationMenu.SetActive(false);
        GenerationPromptButton.SetActive(true);
        GenerationMenu.transform.localScale = new Vector3(0, 0, 0);
        preview = false;
    }

    public void OnEmptyScene()
    {
        GenerationMenu.SetActive(false);
        GenerationPromptButton.SetActive(true);
    }

    public void OnGenerationPromptButtonClick()
    {
        ToggleToolBar(false);
        GenerationPromptButton.SetActive(false);
        GenerationMenu.SetActive(true);
        EnterGenerationMenu();
        SweaterMenu.SetActive(false);
        PanelMenu.SetActive(true);
        SetSweaterIdle();
        SetPanelActive();
        PanelBool = true;
        preview = true;
    }

    private void ToggleToolBar(bool active)
    {
        foreach (var b in buttons)
        {
            b.GetComponent<ButtonAnimation>().active = active;
        }
    }

    private void EnterGenerationMenu()
    {
        LeanTween.scale(GenerationMenu, new Vector3(1, 1, 1), 0.25f).setEase(LeanTweenType.easeOutExpo);
    }

    public void OnPanelButtonClick()
    {
        SetSweaterIdle();
        SetPanelActive();
        SweaterMenu.SetActive(false);
        PanelMenu.SetActive(true);
        PanelBool = true;
        //set preview values
    }

    public void SetPanelActive()
    {
        PanelButton.GetComponent<Image>().overrideSprite = Panel_Active;
    }

    public void SetPanelIdle()
    {
        PanelButton.GetComponent<Image>().overrideSprite = Panel_Idle;
    }

    public void SetSweaterActive()
    {
        SweaterButton.GetComponent<Image>().overrideSprite = Sweater_Active;
    }

    public void SetSweaterIdle()
    {
        SweaterButton.GetComponent<Image>().overrideSprite = Sweater_Idle;
    }

    public void OnSweaterButtonClick()
    {
       SetPanelIdle();
       SetSweaterActive();
        PanelMenu.SetActive(false);
        SweaterMenu.SetActive(true);
        PanelBool = false;
        //set preview values
    }

    public void ButtonHoverEnter(GameObject button)
    {
        LeanTween.scale(button, new Vector3(0.55f, 0.55f, 1), 0.2f).setEase(LeanTweenType.easeOutBack);
    }
    public void ButtonHoverExit(GameObject button)
    {
        LeanTween.scale(button, new Vector3(0.5f, 0.5f, 1), 0.1f).setEase(LeanTweenType.easeInSine);
    }

    public void ButtonPointerDown(GameObject button)
    {
        LeanTween.scale(button, new Vector3(0.45f, 0.45f, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
    }
    
    public void ButtonPointerUp(GameObject button)
    {
        LeanTween.scale(button, new Vector3(0.5f, 0.5f, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
    }
    
    public void C_ButtonHoverEnter(GameObject button)
    {
        LeanTween.scale(button, new Vector3(1.2f, 1.2f, 1), 0.2f).setEase(LeanTweenType.easeOutBack);
    }
    public void C_ButtonHoverExit(GameObject button)
    {
        LeanTween.scale(button, new Vector3(1f, 1f, 1), 0.1f).setEase(LeanTweenType.easeInSine);
    }

    public void C_ButtonPointerDown(GameObject button)
    {
        LeanTween.scale(button, new Vector3(0.9f, 0.9f, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
    }
    
    public void C_ButtonPointerUp(GameObject button)
    {
        LeanTween.scale(button, new Vector3(1f, 1f, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
    }

    public void CloseMenu()
    {
        LeanTween.scale(GenerationMenu, new Vector3(0, 0, 0), 0.2f).setEase(LeanTweenType.easeInBack);
        ToggleToolBar(true);
        preview = false;
    }

    public void Generate()
    {
        if (PanelBool)
        {
            var config = new PanelConfig(
            "panel",
            new Vector2Int(Mathf.FloorToInt(PanelWidth.value), Mathf.FloorToInt(PanelHeight.value)),
            false,
            new Vector2Int((int)GaugeHorizontal.value, (int)GaugeVertical.value),
            new Vector3(-(PanelWidth.value * 10f / GaugeHorizontal.value)/2,-(PanelHeight.value * 10f / GaugeVertical.value)/2, 0)
            );
            
            PanelGenerator.GeneratePanel(config);
        }

        else
        {
            SweaterGenerator.InitializeSweaterValues((int)BodyWidth.value,
                (int)BodyLength.value,
                SleeveWidth.value,
                (int)SleeveLength.value,
                CollarWidth.value,
                (int)CollarHeight.value);
        }
    }

    public void Update()
    {
        if (preview)
        {
            if (!PanelBool)
            {
                SweaterPreview.SetPoints(SweaterPreview.PointsFromSweaterParameters(BodyWidth.value,
                    BodyLength.value,
                    SleeveLength.value,
                    SweaterGenerator.StitchCountFromRelativeSize(SleeveWidth.value,
                        (int)BodyLength.value),
                    SweaterGenerator.StitchCountFromRelativeSize(CollarWidth.value,
                        (int)BodyWidth.value),
                    CollarHeight.value,
                    new Vector2(GaugeHorizontal.value,
                        GaugeVertical.value)));
            }
            else
            {
                SweaterPreview.SetPoints(SweaterPreview.PointsFromSweaterParameters(PanelWidth.value,
                    PanelHeight.value,
                    0,
                    0,
                    0,
                    0,
                    new Vector2(GaugeHorizontal.value,
                        GaugeVertical.value)));
            }
            
        }
    
}
    
    
    
    
    






}
