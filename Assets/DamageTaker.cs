using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof (Collider))]
public class DamageTaker : MonoBehaviour
{
    private Vector2 _position;
    public Transform combatTextPrefab;
    public GUISkin customSkin = null;
    public Camera guiCamera = null;
    public int health = 100;
    public float maxViewAngle = 90.0f;
    public string styleName = "WorldObjectLabel";

    private Queue<Damage> _damages;

    // Use this for initialization
    private void Start()
    {
        _damages = new Queue<Damage>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void Reset()
        // Fallback for the camera reference
    {
        if (guiCamera == null)
        {
            guiCamera = Camera.main;
            maxViewAngle = guiCamera.fieldOfView*0.5f;
        }
    }

    private void OnGUI()
    {
   
        useGUILayout = false;
        // We're not using GUILayout, so don't spend processing on it

        if (Event.current.type != EventType.Repaint)
            // We are only interested in repaint events
        {
            return;
        }
        var timeDelta = Time.deltaTime;
        var worldPosition = collider.bounds.center + Vector3.up*collider.bounds.size.y*0.6f;

        if (Vector3.Angle(guiCamera.transform.forward, worldPosition - guiCamera.transform.position) > maxViewAngle)
        // If the world position is outside of the field of view or further away than hideDistance, don't render the label
        {
            return;
        }

        var finishedDamagesCount = 0;
        foreach (var damage in _damages)
        {


            damage.ReduceAlpha(timeDelta / 1.5f);
            damage.IncreaseScrollPosition(0.6f*timeDelta);
            var damagePosition = worldPosition;
            damagePosition.y += damage.CurrentScrollPosition;
           // worldPosition.y += damage.CurrentScrollPosition;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, damage.Alpha);

          

           
            Vector2 position = guiCamera.WorldToScreenPoint(damagePosition);
            position = new Vector2(position.x, Screen.height - position.y);
            // Get the GUI space position

            GUI.skin = customSkin;
            // Set the custom skin. If no custom skin is set (null), Unity will use the default skin

            var contents = string.IsNullOrEmpty(damage.ToString()) ? "" : damage.Amount.ToString();

            var size = GUI.skin.GetStyle(styleName).CalcSize(new GUIContent(contents));
            // Get the content size with the selected style

            var rect = new Rect(position.x - size.x*0.5f, position.y - size.y, size.x, size.y);
            // Construct a rect based on the calculated position and size

            GUI.skin.GetStyle(styleName).Draw(rect, contents, false, false, false, false);

            if (damage.Alpha <= 0)
            {
                finishedDamagesCount++;
            }
        }

        for (int i = 0; i < finishedDamagesCount; i++)
        {
            _damages.Dequeue();
        }

        // Draw the label with the selected style
    }

    public void TakeDamage(int damage)
    {
        _damages.Enqueue(new Damage(damage));
        health -= damage;

        if (health <= 0)
        {
            var dropper = this.GetComponent<LootDropper>();
            dropper.DropLoot();
        }
    }
}

internal class Damage
{
    public Damage(int amount)
    {
        Amount = amount;
        Alpha = 1.0f;
        CurrentScrollPosition = 0.0f;
    }

    public int Amount { get; private set; }
     public float Alpha { get; private set; }
     public float CurrentScrollPosition { get; private set; }

    public void ReduceAlpha(float a)
    {
        Alpha -= a;
    }

    public void IncreaseScrollPosition(float f)
    {
        CurrentScrollPosition += f;
    }
}