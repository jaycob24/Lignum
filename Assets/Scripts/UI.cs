using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

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
