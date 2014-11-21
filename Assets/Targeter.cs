using System;
using System.Linq;
using UnityEngine;
using System.Collections;


public class Targeter : MonoBehaviour {

    private FireBall Bullet;
    public Camera camera;
    public GameObject emitter;

    // The text rendered in the label.
    public GUISkin customSkin = null;
    // The skin containing the style used to render the label (leave as null to use the default skin)
    public string styleName = "Box";
    // The style used to render the label. Must be available in the used skin.
    public Camera guiCamera = null;
    // The camera used to display the GUI. Used for coordinate and distance calculations.
    public float fadeDistance = 30.0f, hideDistance = 35.0f;
    // Specifies when the label should start fading and when it should hide
    public float maxViewAngle = 90.0f;
    private bool _targetSelected;
    private GameObject _target;
    // Specifies at which angle to the camera forward vector, the label should no longer render
   

    // Fire a bullet 
    public void Target()
    {
        const int layerMask = ~(1 << 9);
        // Create a new bullet pointing in the same direction as the gun 
        //Bullet = Instantiate(Fire, transform.position, transform.rotation); 
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 30f, layerMask))
        {
            Debug.Log(string.Format("HIT: {0}", hitInfo.collider.gameObject.name));
            Vector3 origin = camera.transform.position;
            Debug.DrawRay(origin, ray.direction * Vector3.Distance(origin, hitInfo.point), Color.cyan, 2f);
            _targetSelected = true;
            _target = hitInfo.collider.gameObject;
            //  Debug.DrawLine(camera.transform.position, hitInfo.point, Color.green, 20f);
        }


       // _targetSelected = false;

        //mitter.Emit();
        // Instantiate(emitter, transform.position, camera.transform.rotation); // transform.rotation); 
    }

    void OnGUI()
    {
      
        if (_targetSelected)
        {
            useGUILayout = false;
            // We're not using GUILayout, so don't spend processing on it

            if (Event.current.type != EventType.Repaint)
                // We are only interested in repaint events
            {
                return;
            }

            Vector3 worldPosition = _target.collider.bounds.center;// + Vector3.up*collider.bounds.size.y*0.5f;
            // Place the label on top of the collider
            float cameraDistance = (worldPosition - guiCamera.transform.position).magnitude;

            if (
                cameraDistance > hideDistance ||
                Vector3.Angle(
                    guiCamera.transform.forward,
                    worldPosition - guiCamera.transform.position
                    ) >
                maxViewAngle
                )
                // If the world position is outside of the field of view or further away than hideDistance, don't render the label
            {
                return;
            }

            if (cameraDistance > fadeDistance)
                // If the distance to the label position is greater than the fade distance, apply the needed fade to the label
            {
                GUI.color = new Color(
                    1.0f,
                    1.0f,
                    1.0f,
                    1.0f - (cameraDistance - fadeDistance)/(hideDistance - fadeDistance)
                    );
            }

            Vector2 position = guiCamera.WorldToScreenPoint(worldPosition);

            position = new Vector2(position.x, Screen.height - position.y);
            // Get the GUI space position

            GUI.skin = customSkin;
            // Set the custom skin. If no custom skin is set (null), Unity will use the default skin

            string contents = string.Format("Name: {0} /n Race: Human /n Profession: Ass Kicking", _target.name);

            Vector2 size = GUI.skin.GetStyle(styleName).CalcSize(new GUIContent(contents));
            // Get the content size with the selected style
            var colliderWidth = Math.Max(_target.collider.bounds.size.x, _target.collider.bounds.size.z);

            var topLeft = worldPosition;
            var bottomRight = worldPosition;
            topLeft.x -= colliderWidth / 2;
            topLeft.y -= _target.collider.bounds.size.y / 2;
            topLeft.z -= colliderWidth / 2;
            bottomRight.x += colliderWidth / 2;
            bottomRight.y += _target.collider.bounds.size.y / 2;
            bottomRight.z += colliderWidth / 2;
        //   _target.transform.rotation

         

            topLeft = guiCamera.WorldToScreenPoint(topLeft);
            topLeft.y = Screen.height - topLeft.y;
            bottomRight = guiCamera.WorldToScreenPoint(bottomRight);
            bottomRight.y = Screen.height - bottomRight.y;

            Debug.Log(string.Format("Xs = {0} {1}", topLeft.x, bottomRight.x) );

            var horizontalVals = new[]
            {
                bottomRight.x,
                topLeft.x,
                //bottomRight.z,
              //  topLeft.z
            };

            var verticalVals = new[]
            {
                bottomRight.y,
                topLeft.y,
            };

            var left = horizontalVals.Min();
            var top = verticalVals.Min();
            var width = horizontalVals.Max() - horizontalVals.Min();
            var height = verticalVals.Max() - verticalVals.Min();
            //var width = Math.Max(bottomRight.x - topLeft.x, bottomRight.z - topLeft.z);
            //width = Math.Max(width, topLeft.x - bottomRight.x);
            //width = Math.Max(width, topLeft.z - bottomRight.z);

            Debug.Log(string.Format("Location X:{0} Y:{1} Width:{2} Height{3}", left, top, width, height));
            Rect rect = new Rect(left,top, width, height);
            // Construct a rect based on the calculated position and size
         //   Debug.Log(string.Format("Draw a box! {0}", rect));
            GUI.skin.GetStyle(styleName).Draw(rect, contents, false, false, false, false);
        }
        // Draw the label with the selected style
    }

    public void Update()
    {
        // Fire if the left mouse button is clicked 
        if (Input.GetButtonDown("Target"))
        {
            Target();
        }
    }
}
