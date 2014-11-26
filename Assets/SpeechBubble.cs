using UnityEngine;
using System.Collections;

public class SpeechBubble : MonoBehaviour 
{
    private string characterName = "No Name";
    private int windowID = WindowIDManager.GetWindowsID();
    public GUISkin mySkin;

	// Use this for initialization
	void Start () 
    {
        Label label = gameObject.GetComponent<Label>();
        characterName = label.labelText;
	}
	
	// Update is called once per frame
	void Update () 
    {
        GameObject player = GameObject.Find("Dude");

        if ((player.transform.position - gameObject.transform.position).magnitude < 2.5f)
        {
            Open();
        }
        else
        {
            Close();
        }
	}

    // 200x300 px window will apear in the center of the screen.
    private Rect windowRect = new Rect(20, Screen.height - 200, Screen.width - 40, 180);
    // Only show it if needed.
    private bool show = false;

    void OnGUI()
    {
        if(mySkin != null)
        {
            GUI.skin = mySkin;
        }
        if (show)
        {
            windowRect = GUI.Window(windowID, windowRect, DialogWindow, characterName + ":");
        }
    }

    void DialogWindow(int windowID)
    {
        float y = 20;
        GUI.Label(new Rect(10, y, windowRect.width, windowRect.height), "Hi, my name is " + characterName + " please don't kill me");
    }

    public void Open()
    {
        show = true;
    }

    public void Close()
    {
        show = false;
    }
}
