using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour 
{
    private int currentHealth;
    private int maxHealth;
    private int currentMana;
    private int maxMana;
    private Timer healthRegenTimer;
    private Timer manaRegenTimer;

	// Use this for initialization
	void Start () 
    {
        currentHealth = 75;
        maxHealth = 100;
        currentMana = 0;
        maxMana = 100;
        healthRegenTimer = new Timer(1);
        manaRegenTimer = new Timer(.5f);
	}
	
	// Update is called once per frame
	void Update () 
    {
        healthRegenTimer.Update();
        manaRegenTimer.Update();
        if(healthRegenTimer.HasElapsed())
        {
            ++currentHealth;
            healthRegenTimer.Reset();
        }
        if (manaRegenTimer.HasElapsed())
        {
            ++currentMana;
            manaRegenTimer.Reset();
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentMana < 0)
        {
            currentMana = 0;
        }
        else if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
	}

    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    public int CurrentMana
    {
        get
        {
            return currentMana;
        }
        set
        {
            currentMana = value;
        }
    }

    public int MaxMana
    {
        get
        {
            return maxMana;
        }
        set
        {
            maxMana = value;
        }
    }
}
