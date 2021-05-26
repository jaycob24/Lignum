using System;
using UnityEngine;
using UnityEngine.UI;

public class UiSpell : MonoBehaviour
{
    public Image image;
    [Header("Spell info")]
    public String spellName;
    [TextArea]
    public String description;

    public bool learned;
    public bool canLearn;
    
    [Header("States")]
    public Sprite learnState;
    public Sprite learntState;
    public Sprite unabledState;

    public UiSpell nextState;

    public void UpdateState()
    {
        if(!learned && !canLearn)
            image.sprite = unabledState;
        if(!learned && canLearn)
            image.sprite = learnState;
        if(learned)
            image.sprite = learntState;
    }
}
