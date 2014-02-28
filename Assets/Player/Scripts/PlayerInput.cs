using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    private static Vector3 GetDirectionalInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }
}