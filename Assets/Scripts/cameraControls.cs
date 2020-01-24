using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    public float scrollSpeed = 10f;
    public float panSpeed = 70f;
    public float panBorderThickness = 10f;
    public float minHeight;
    public float maxHeight;
    public int len = 128;
    public float planeborder = 0F;
    public Camera orthographicCamera;

    public Vector3 rot;
    public Vector3 pos;
    private float scroll;
    private float hightRatio;
    private bool midButtonDown;
    private Vector3 mousePosAtClick;
    private Vector3 mousePosDiffClick;
    private Vector3 mousePosDiffClickLast;

    // Start is called before the first frame update
    void Start()
    {
        midButtonDown = false;
    }

    public void ResetRotAndHeight()
    {
        pos = transform.position;
        // reset rot
        mousePosAtClick = new Vector3(0f, 0f, 0f);
        mousePosDiffClick = new Vector3(0f, 0f, 0f);
        mousePosDiffClickLast = new Vector3(0f, 0f, 0f);

        float pMin = orthographicCamera.GetComponent<orthographicCameraControls>().minHeight;
        float pMax = orthographicCamera.GetComponent<orthographicCameraControls>().maxHeight;
        float pCurrent = orthographicCamera.GetComponent<orthographicCameraControls>().orthographicSize;

        pCurrent = pCurrent - pMin;
        pCurrent = pCurrent * ((maxHeight - minHeight) / (pMax - pMin)) + minHeight;

        pos = orthographicCamera.transform.position;
        pos.y = pCurrent;

        transform.position = pos;
    }
    
    void LateUpdate()
    {
        pos = transform.position;
        hightRatio = (pos.y - minHeight) / (maxHeight - minHeight);


        // rotation
        //scroll click rotation
        if (Input.GetMouseButtonDown(2))
        {
            mousePosAtClick = Input.mousePosition + mousePosDiffClickLast;
            midButtonDown = true;
        }

        if (Input.GetMouseButtonUp(2))
            midButtonDown = false;

        if (midButtonDown)
        {
            mousePosDiffClick = mousePosAtClick - Input.mousePosition;
            mousePosDiffClick.y = Mathf.Clamp(mousePosDiffClick.y, -100, 100);
            mousePosDiffClickLast = mousePosDiffClick;
        }

        rot = new Vector3(mousePosDiffClick.y * 0.01f * 90f + 90f, mousePosDiffClick.x *0.5f, 0f);

        rot.x = Mathf.Clamp(rot.x, 20, 90);


        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.x += Mathf.Sin(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
            pos.z += Mathf.Cos(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.x -= Mathf.Sin(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
            pos.z -= Mathf.Cos(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.z -= Mathf.Sin(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
            pos.x += Mathf.Cos(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.z += Mathf.Sin(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
            pos.x -= Mathf.Cos(rot.y * Mathf.Deg2Rad) * Mathf.Pow((hightRatio + 0.7f), 5f) * panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -(0 - planeborder), (len - planeborder));
        pos.z = Mathf.Clamp(pos.z, -(0 - planeborder), (len - planeborder));

        //
        scroll = -Input.GetAxis("Mouse ScrollWheel");

        pos.y += scroll * scrollSpeed * Time.deltaTime * 100f;
        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
        //
        transform.eulerAngles = rot;
        transform.position = pos;

    }
}
