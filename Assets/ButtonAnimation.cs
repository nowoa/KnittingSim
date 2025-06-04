using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    public GameObject Background;
    public Image BG_Img;
    public GameObject Text;
    public GameObject Icon;
    public LeanTweenType easeType;
    public LeanTweenType popUp;
    public LeanTweenType popUpEnd;
    public Animator Animator;
    private bool IsSelected;
    private Tool tool;
    public ToolEnum _toolEnum;
    public ButtonAnimation[] buttons;
    public Color color;
    public bool active;

    public enum ToolEnum
    {
        Default,
        Dragger,
        StitchBrush,
        ColorBrush,
        Selector,
    }

    // Start is called before the first frame update
    void Start()
    {
        active = true;
        switch (_toolEnum)
        {
            case ToolEnum.Dragger:
                tool = ToolManager.DraggerInstance;
                AnimateRelease();
                return;
            case ToolEnum.StitchBrush:
                tool = ToolManager.StitchBrushInstance;
                return;
            case ToolEnum.ColorBrush:
                tool = ToolManager.ColorBrushInstance;
                return;
            case ToolEnum.Selector:
                tool = ToolManager.SelectorInstance;
                return;
        }
        
    }


    public void AnimateEnter()
    {
        if (!active) return;
        if (ToolManager.ActiveTool == tool) return;
        LeanTween.scale(Background, new Vector3(1.3f, 1.3f, 1), 0.2f).setEase(easeType);
        LeanTween.scale(Text, new Vector3(1f, 1f, 1), 0.3f).setEase(popUp);
        LeanTween.scale(Icon, new Vector3(1f, 1f, 1f), 0.2f).setEase(popUp);
        Animator.SetTrigger("Highlighted");
    }

    public void AnimateExit()
    {
        if (!active) return;
        if (ToolManager.ActiveTool == tool) return;
        BG_Img.color = Color.white;
        LeanTween.scale(Background, new Vector3(0f, 0f, 1), 0.2f).setEase(easeType);
        LeanTween.scale(Text, new Vector3(0f, 0f, 1), 0.3f).setEase(popUpEnd);
        LeanTween.scale(Icon, new Vector3(0.9f, 0.9f, 1f), 0.2f).setEase(LeanTweenType.easeOutSine);
        Animator.SetTrigger("Normal");
    }

    public void AnimatePressDown()
    {
        if (!active) return;
        if (_toolEnum != default)
        {
            ToolManager.SetActiveTool(tool);
        }
        foreach (var b in buttons)
        {
            b.AnimateExit();
        }
        LeanTween.scale(Background, new Vector3(0.9f, 0.9f, 1), 0.1f).setEase(LeanTweenType.easeOutSine);
        
        LeanTween.scale(Icon, new Vector3(0.9f, 0.9f, 1f), 0.15f).setEase(LeanTweenType.easeOutSine);
        BG_Img.color = color;
        Animator.SetTrigger("Normal");
        
        
        
    }

    public void AnimateRelease()
    {
        if (!active) return;
        LeanTween.scale(Background, new Vector3(1.1f, 1.1f, 1), 0.1f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(Text, new Vector3(0f, 0f, 1), 0.1f).setEase(LeanTweenType.easeInSine);
        LeanTween.scale(Icon, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    public void TurnOffGameInputs()
    {
        InputHandler.ToggleGameInput(false);
    }

    public void TurnOnGameInputs()
    {
        InputHandler.ToggleGameInput(true);
    }

}

