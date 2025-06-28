using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text HP;

    private int maxHealth;

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
        slider.maxValue = health;
        slider.value = health;
        HP.text = health + "/" + maxHealth;
    }

    public void SetHealth(int currentHealth)
    {
        slider.value = currentHealth;
        HP.text = currentHealth + "/" + maxHealth;
    }
}
