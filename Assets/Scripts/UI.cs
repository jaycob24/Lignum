using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region Spells

    
    
    [Header("Slots for spells")]
    public List<Slot> slots;
    
    [Header("Spells")]
    public List<UiSpell> spellsList;
    public Text spellName;
    public Text spellDescription;

    public void OpenSpellTree()
    {
        foreach (var uiSpell in spellsList)
        {
            uiSpell.UpdateState();
        }
    }
    public void SetSpellInfo(UiSpell spellId)
    {
        spellName.text = spellId.spellName;
        spellDescription.text = spellId.description;
    }
    public void ClearSpellInfo()
    {
        spellName.text = defaultSpellName;
        spellDescription.text = defaultSpellDescription;
    }

    public void TryLearn(UiSpell uiSpell)
    {
        if (uiSpell.canLearn)
        {
            if (!uiSpell.learned)
            {
                uiSpell.learned = true;
                if (uiSpell.nextState != null)
                {
                    uiSpell.nextState.canLearn = true;
                    uiSpell.nextState.learned = false;
                    uiSpell.nextState.UpdateState();
                }
            }
            else
            {
                spellDescription.text = "You already know this spell!";
            }
        }
        uiSpell.UpdateState();
    }
    
    [Header("Default info")]
    public String defaultSpellName;
    [TextArea]
    public String defaultSpellDescription;

    #endregion

    public GameObject spellUi;
    public GameObject inventoryUi;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenSpellTree();
            Time.timeScale = spellUi.activeSelf ? 1f : 0f;
            spellUi.SetActive(!spellUi.activeSelf);
            
            for (var i = 0; i < spellsList.Count; i++)
            {
                if (i == 2 && spellsList[2].learned)
                {
                    slots[1].slot.sprite = spellsList[2].learntState;
                    continue;
                }
                
                if (spellsList[i].learned)
                {
                    slots[i].slot.sprite = spellsList[i].image.sprite;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = inventoryUi.activeSelf ? 1f : 0f;
            inventoryUi.SetActive(!inventoryUi.activeSelf);
        }
    }

    [SerializeField] private Image healthBar; 
    [SerializeField] private Image manaBar;

    public float GetHealth() => healthBar.fillAmount;
    public float GetMana() => manaBar.fillAmount;

    public void DamageHealth(float value)
    {
        healthBar.fillAmount = value;
    }

    public void DamageMana(float value)
    {
        manaBar.fillAmount = value;
    }
}
