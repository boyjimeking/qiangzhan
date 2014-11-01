
using UnityEngine;
public class RoleModelScript : MonoBehaviour
{
    void OnDrag(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) < 0.001f)
            return;
        CreateRoleManager.Instance.Rotate(delta.x > 0);
    }
}
