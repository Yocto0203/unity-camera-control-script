using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MouseButtonDown
{
    MBD_LEFT = 0,
    MBD_RIGHT,
    MBD_MIDDLE,
};
public class MyCameraControl : MonoBehaviour


{
    [SerializeField]
    public Vector3 focusOffset = Vector3.zero;
    [SerializeField]
    private GameObject focusObj = null;
    [SerializeField]
    private float Angle_limit_top = 70;
    [SerializeField]
    private float Angle_limit_Bottom = -70;

    private Vector3 oldPos;
    // Start is called before the first frame update
    void Start()
    {
        if (this.focusObj == null)
        {
            Debug.LogError("focusObj is null");
        }
        Transform trans = this.transform;
        this.transform.parent = this.focusObj.transform;

        this.transform.rotation = Quaternion.LookRotation(this.focusObj.transform.position - this.transform.position + focusOffset);
    }

    // Update is called once per frame
    void Update()
    {
        mouseEvent();
    }

    void mouseEvent()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0.0f)
            this.mouseWheelEvent(delta);

        
        if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
            Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
            Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
            this.oldPos = Input.mousePosition;
        
        this.mouseDragEvent(Input.mousePosition);
        
        return;
    }
    void mouseDragEvent(Vector3 mousePos)
    {
        
        Vector3 diff = mousePos - oldPos;
        if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
        {
            if (diff.magnitude > Vector3.kEpsilon)
                this.cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
        }
        this.oldPos = mousePos;

        return;
    }

    public void cameraRotate(Vector3 eulerAngle)
    {
        //回転軸を求めるためのベクトル作成
        Vector3 v = this.transform.position - this.focusObj.transform.position;
        v.y = 0.0f;
        v.Normalize();

        //ベクトルを元に四元数を作成
        Quaternion rot_right = Quaternion.AngleAxis(90, Vector3.up);
        Vector3 z_rotate_axis = rot_right*v;
        Quaternion rot_z = Quaternion.identity;

        //角度制限
        if (this.transform.localEulerAngles.x - eulerAngle.x < Angle_limit_top
            || this.transform.localEulerAngles.x - eulerAngle.x > 360 + Angle_limit_Bottom)
        {
            rot_z = Quaternion.AngleAxis(eulerAngle.x, z_rotate_axis);
        }

        Quaternion rot_y = Quaternion.AngleAxis(eulerAngle.y, Vector3.up);

        //回転を行う
        Vector3 q = this.transform.position - this.focusObj.transform.position;
        this.transform.position = this.focusObj.transform.position + rot_z * rot_y * q;
        this.transform.rotation = Quaternion.LookRotation(this.focusObj.transform.position - this.transform.position + focusOffset);

        return;
    }

    public void mouseWheelEvent(float delta)
    {

        Vector3 focusToPosition = this.transform.position - this.focusObj.transform.position;

        Vector3 post = focusToPosition * (1.0f + delta);

        if (post.magnitude > 0.01)
        {
            this.transform.position = this.focusObj.transform.position + post;
        }
        this.transform.rotation = Quaternion.LookRotation(this.focusObj.transform.position - this.transform.position + focusOffset);
    }
}
