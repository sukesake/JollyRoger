using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
    #region Skill bar variables
    public GUISkin skillbarSkin;
    private float skillBarButtonSize = 55;
    private float skillBarButtonSpacing = 20;
    private int skillBarWindowID = WindowIDManager.GetWindowsID();
    private Rect skillBarRect;
    #endregion

    #region Health bar variables
    public GUISkin healthBarSkin;
    private int healthBarWindowID = WindowIDManager.GetWindowsID();
    private Rect healthBarRect;
    int currentHealth = 50;
    int maxHealth = 100;
    #endregion

    #region Mana bar variables
    public GUISkin manaBarSkin;
    private int manaBarWindowID = WindowIDManager.GetWindowsID();
    private Rect manaBarRect;
    int currentMana = 0;
    int maxMana = 100;
    #endregion

    // Use this for initialization
	void Start () 
    {
        skillBarRect = new Rect(300, Screen.height - 100, (skillBarButtonSize * 10) + (skillBarButtonSpacing * 11), 80);
        healthBarRect = new Rect(skillBarRect.xMin, skillBarRect.yMin - 30, skillBarRect.width / 2, 30);
        manaBarRect = new Rect(skillBarRect.xMin + skillBarRect.width / 2, skillBarRect.yMin - 30, skillBarRect.width / 2, 30);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (currentHealth < maxHealth)
        {
            ++currentHealth;
        }

        if (currentMana < maxMana)
        {
            ++currentMana;
        }
	}

    void OnGUI()
    {
        GUISkin tempSkin = GUI.skin;

        // Draw Skill Bar
        GUI.skin = skillbarSkin;
        skillBarRect = GUI.Window(skillBarWindowID, skillBarRect, SkillBar, "");

        // Draw Health Bar
        GUI.skin = healthBarSkin;
        healthBarRect = GUI.Window(healthBarWindowID, healthBarRect, HealthBar, "");

        // Draw Mana Bar
        GUI.skin = manaBarSkin;
        manaBarRect = GUI.Window(manaBarWindowID, manaBarRect, ManaBar, "");

        GUI.skin = tempSkin;
    }

    void SkillBar(int windowID)
    {
        for (int i = 0; i < 10; ++i)
        {
            GUI.Button(new Rect(20 + ((skillBarButtonSize + 20) * i), (skillBarRect.height - skillBarButtonSize) / 2, skillBarButtonSize, skillBarButtonSize), "Skill " + (i + 1));
        }
    }

    void HealthBar(int windowID)
    {
        GUI.Box(new Rect(5, 5, (healthBarRect.width - 10) * ((float)currentHealth / (float)maxHealth), healthBarRect.height - 10), "");
        GUI.Label(new Rect(5, 5, healthBarRect.width - 10, healthBarRect.height - 10), currentHealth + " / " + maxHealth);
    }

    void ManaBar(int windowID)
    {
        GUI.Box(new Rect(5, 5, (manaBarRect.width - 10) * ((float)currentMana / (float)maxMana), manaBarRect.height - 10), "");
        GUI.Label(new Rect(5, 5, manaBarRect.width - 10, manaBarRect.height - 10), currentMana + " / " + maxMana);
    }
}
