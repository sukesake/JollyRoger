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

    public GameObject CurrentTarget
    {
        get { return _targetSelected ? _target: null; }
    }

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
            if (hitInfo.collider.gameObject.GetComponent<Targetable>() != null)
            {
                _targetSelected = true;
                _target = hitInfo.collider.gameObject;
            }
            else
            {
                _targetSelected = false;
            }
            //  Debug.DrawLine(camera.transform.position, hitInfo.point, Color.green, 20f);
        }


       // _targetSelected = false;

        //mitter.Emit();
        // Instantiate(emitter, transform.position, camera.transform.rotation); // transform.rotation); 
    }

    void OnGUI()
    {
      
        if (_targetSelected && _target != null)
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

            string contents = " ";

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

            var width = 0f; //horizontalVals.Max() - horizontalVals.Min();
            var height = 0f;
            FindWidthInScreen(_target, out width, out height);



            var left = position.x - width / 2;  //horizontalVals.Min();
            var top = position.y - height/2;
          
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

    private void FindWidthInScreen(GameObject _TargetGameObject, out float width, out float height)
    {
        var xMin = new Vector2(Screen.width, 0);
        var xMax = Vector2.zero;
        var yMin = new Vector2(Screen.height, 0);
        var    yMax = Vector2.zero;

        for (var i = 0; i != 8; i++)
        {
            //FindBoundCord() locates the eight coordinates that forms the boundries, followed by converting the coordinates to screen space.
            // The entire script starts in FindBoundCord

            Vector2 _ObjectScreenCord = camera.WorldToScreenPoint(FindBoundCord(i, _TargetGameObject));

            if (_ObjectScreenCord.x > xMax.x)
            {
                xMax.x = _ObjectScreenCord.x;
            }
            else if (_ObjectScreenCord.x < xMin.x)
            {
                xMin.x = _ObjectScreenCord.x;
            }
            if (_ObjectScreenCord.y > yMax.x)
            {
                yMax.x = _ObjectScreenCord.y;
            }
            else if (_ObjectScreenCord.y < yMin.x)
            {
                yMin.x = _ObjectScreenCord.y;
            }

        }

        var targetHeight = Vector2.Distance(yMax, yMin);
        if (targetHeight > Screen.height || targetHeight < 0)
        {
            targetHeight = 0;
        }

        var targetWidth = Vector2.Distance(xMax, xMin);
        if (targetWidth > Screen.width || targetWidth < 0)
        {
            targetWidth = 0;
        }

        // Here we simply make a check on which of the height or the 
        // width is the biggest. It was a necessary part for my project.
        int minSize = 70;
        height = Math.Max(targetHeight, minSize);
        width = Math.Max(targetWidth, minSize);
        /*
        The method appears to work, though there are som serious issues when 
        Screen coordinates gets into negative values.
        When screencoordinates get negative the width/height 
        explodes, and I think it is because of the 
        Trapez formed viewport. */

    }

    private Vector3 FindBoundCord(int i, GameObject _GameObject)
    {
        /*This is basically where the code starts. It starts out by creating a 
           * bounding box around the target GameObject. 
          It calculates the 8 coordinates forming the bounding box, and 
          returns them all to the for loop.
          Because there are no real method which returns the coordinates 
          from the bounding box I had to create a switch/case which utillized 
          Bounds.center and Bounds.extents.*/

        Bounds _TargetBounds = _GameObject.collider.bounds;
        Vector3 _TargetCenter = _TargetBounds.center;
        Vector3 _TargetExtents = _TargetBounds.extents;


        switch (i)
        {
            case 0:
                return _TargetCenter + new Vector3(_TargetExtents.x, _TargetExtents.y, _TargetExtents.z);
            case 1:
                return _TargetCenter + new Vector3(_TargetExtents.x, _TargetExtents.y, _TargetExtents.z*-1);
            case 2:
                return _TargetCenter + new Vector3(_TargetExtents.x, _TargetExtents.y*-1, _TargetExtents.z);
            case 3:
                return _TargetCenter + new Vector3(_TargetExtents.x, _TargetExtents.y*-1, _TargetExtents.z*-1);
            case 4:
                return _TargetCenter + new Vector3(_TargetExtents.x*-1, _TargetExtents.y, _TargetExtents.z);
            case 5:
                return _TargetCenter + new Vector3(_TargetExtents.x*-1, _TargetExtents.y, _TargetExtents.z*-1);
            case 6:
                return _TargetCenter + new Vector3(_TargetExtents.x*-1, _TargetExtents.y*-1, _TargetExtents.z);
            case 7:
                return _TargetCenter + new Vector3(_TargetExtents.x*-1, _TargetExtents.y*-1, _TargetExtents.z*-1);
            default:
                return Vector3.zero;
        }

    }


    public
        void Update()
    {
        // Fire if the left mouse button is clicked 
        if (Input.GetButtonDown("Target"))
        {
            Target();
        }
    }
}
