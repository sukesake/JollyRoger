using UnityEngine;
using System.Collections;

public class InspectBubble : MonoBehaviour
{
    private string characterName = "No Name";
    public GUISkin mySkin;
    public float Left = 20;
    public float Top = 20;
    public float Width = 300;
    public float Height = 500;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

       // if (Input.GetButtonDown("InspectTarget"))
      //  {
            _target = gameObject.GetComponent<Targeter>().CurrentTarget;
            if (_target != null)
            {
                 Open();
            }
            else
            {
                Close();
            }
       // }
    }

    private bool show;
    private GameObject _target;


    void OnGUI()
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        if (show)
        {
            GUI.skin = mySkin ?? GUI.skin;
            string content = string.Format("Target Details\n" +
                                           "Name: {0}\n" +
                                           "Status: {1}\n" +
                                           "Class: {2}\n" +
                                           "Profession: {3}\n" +
                                           "Mood: {4}\n", 
                                           _target.name,
                                           _target.GetComponent<Statuser>().GetStatus(),
                                           _target.GetComponent<Classer>().GetClass(),
                                           _target.GetComponent<Professioner>().GetProfession(),
                                           _target.GetComponent<Mooder>().GetMood());
            var guiStyle = GUI.skin.GetStyle("box");
            Vector2 size = guiStyle.CalcSize(new GUIContent(content));

            var windowRect = new Rect(Left, Top, size.x, size.y);
            guiStyle.Draw(windowRect, content, false, false, false, false);
        }
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
