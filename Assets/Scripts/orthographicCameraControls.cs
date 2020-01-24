using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orthographicCameraControls : MonoBehaviour
{
    public float panSpeed = 5f;
    public float panHeightSpeed = 20f;
    public float panBorderThickness = 10.0f;
    public Camera projectiveCamera;
    public float scrollSpeed = 5.0f;
    public float minHeight = 5.0f;
    public float maxHeight = 30.0f;
    public float orthographicSize;
    private float scroll;
    private Vector3 pos;

    float mapX = 128f;
    float mapY = 128f;

    private float vertExtent;
    private float horzExtent;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void ResetOrthographicSize()
    {
        float pMin = projectiveCamera.GetComponent<cameraControls>().minHeight;
        float pMax = projectiveCamera.GetComponent<cameraControls>().maxHeight;
        float pCurrent = projectiveCamera.GetComponent<cameraControls>().pos.y;

        pCurrent = pCurrent - pMin;
        pCurrent = pCurrent * ((maxHeight - minHeight) / (pMax - pMin)) + minHeight;

        Camera.main.orthographicSize = pCurrent;
    }
void LateUpdate()
    {
        pos = transform.position;
        orthographicSize = Camera.main.orthographicSize;
        panHeightSpeed = orthographicSize;

        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * panHeightSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * panHeightSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * panHeightSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * panHeightSpeed * Time.deltaTime;
        }



        scroll = -Input.GetAxis("Mouse ScrollWheel");

        orthographicSize += scroll * scrollSpeed * Time.deltaTime * 100f;
        orthographicSize = Mathf.Clamp(orthographicSize, minHeight, maxHeight);


        vertExtent = orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;

        // Calculations assume map is position at the origin
        minX = horzExtent - 0.5f;
        maxX = mapX - horzExtent - 0.5f;
        minY = vertExtent - 0.5f;
        maxY = mapY - vertExtent - 0.5f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minY, maxY);

        Camera.main.orthographicSize = orthographicSize;
        transform.position = pos;
    }
}
