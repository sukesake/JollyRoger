using System;
using System.Net.Mime;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public float alpha = 2.0f;
    public Color color = new Color(0.8f, 0.8f, 0.0f, 1.0f);
    public float duration = 1.5f; // time to die
    public float scroll = 0.05f; // scrolling velocity

    // Use this for initialization
    private void Start()
    {
        Debug.Log("Create DamageText");
        guiText.material.color = color; // set text color
        guiText.text = "";
        alpha = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        if (alpha > 0)
        {
        //    Debug.Log("update Alpha and pos");
          //  transform.position += new Vector3(0, scroll * Time.deltaTime,0);
          //  alpha -= Time.deltaTime/duration;
         //   guiText.material.color = new Color(guiText.material.color.r, guiText.material.color.g, guiText.material.color.b, alpha);
        }
        else
        {
            if (!String.IsNullOrEmpty(guiText.text))
            {
                Debug.Log("Destory DamageText : " + guiText.text);
                Destroy(gameObject); // text vanished - destroy itself
            }
        }
    }

    public void UpdateText(string text)
    {
        Debug.Log(string.Format("updating text to: {0} @ {1}", text, transform.position));
        guiText.text = text;
    }
}