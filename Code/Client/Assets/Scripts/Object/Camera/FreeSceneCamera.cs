using System;
using UnityEngine;

/// <summary>
/// 场景自由摄像机
/// </summary>
public class FreeSceneCamera : MonoBehaviour
{
    private Ray look_ray;
    public Camera mCamera;
    private Transform obj;
    private RaycastHit ray_hit;
    private float rotate_round_speed_x = 360f;
    private float rotate_round_speed_y = 100f;
    private Vector3 target_pos = Vector3.zero;
    public float target_trans_speed = -60f;
    private float target_y;
    public bool toogle = true;
    public float trans_speed_h = 60f;
    public float trans_speed_v = 45f;
    public float zoom_speed = 6000f;

    private void Start()
    {
        obj = mCamera.transform;
    }

    private void Update()
    {
        if (toogle)
        {
            target_y = target_pos.y;
            float num = (zoom_speed * Mathf.Abs((float)(obj.position.y - target_y))) / 100f;

            //鼠标中间滑轮
            float num2 = (Input.GetAxis("Mouse ScrollWheel") * num) * Time.deltaTime;
            obj.Translate((Vector3)(num2 * Vector3.forward));

            if (Input.GetMouseButton(2))
            {
                float x = (Input.GetAxis("Mouse X") * target_trans_speed) * Time.deltaTime;
                float y = (Input.GetAxis("Mouse Y") * target_trans_speed) * Time.deltaTime;
                obj.Translate(new Vector3(x, y, 0f));
            }
            if (Input.GetMouseButton(1))
            {
                float angle = (Input.GetAxis("Mouse X") * rotate_round_speed_x) * Time.deltaTime;
                float num6 = (Input.GetAxis("Mouse Y") * rotate_round_speed_y) * Time.deltaTime;
                obj.RotateAround(target_pos, Vector3.up, angle);
                obj.RotateAround(target_pos, obj.TransformDirection(Vector3.right), -num6);
                float num7 = (Input.GetAxis("Vertical") * trans_speed_v) * Time.deltaTime;
                float num8 = (Input.GetAxis("Horizontal") * trans_speed_h) * Time.deltaTime;
                obj.Translate((Vector3)(num7 * Vector3.forward));
                obj.Translate((Vector3)(num8 * Vector3.right));
            }
            look_ray = mCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(look_ray, out ray_hit, 8000f))
            {
            }
            if (!Input.anyKey)
            {
                target_y = ray_hit.point.y;

                float dy = look_ray.direction.y;
                dy = Math.Abs(dy) < 0.001f ? 0.001f : dy;

                target_pos = new Vector3(look_ray.origin.x - ((look_ray.direction.x * (look_ray.origin.y - target_y)) / dy), ray_hit.point.y, look_ray.origin.z - ((look_ray.direction.z * (look_ray.origin.y - target_y)) / dy));
            }
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0))
            {
                float num9 = (Input.GetAxis("Mouse X") * rotate_round_speed_x) * Time.deltaTime;
                float num10 = (Input.GetAxis("Mouse Y") * rotate_round_speed_y) * Time.deltaTime;
                obj.RotateAround(target_pos, Vector3.up, num9);
                obj.RotateAround(target_pos, obj.TransformDirection(Vector3.right), -num10);
            }
        }
    }
}

