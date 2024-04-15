using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    float camSize;
 
    float camSpeed = 10;
    float camRange = 10;

    Vector3 camPosition = Vector3.zero;

    Camera cam;

    private void Awake()
    {
        camSize = 6.0f;
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Vector3 ScreenAimPoint = /*Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));*/ Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
        Vector3 offset = ScreenAimPoint - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f,0) * 0.0001f * camSize; //화면 중심 계산
        camPosition = GameManager.Instance.GetPlayer.transform.position + new Vector3(0, 0, -10) + offset * camRange;
        this.transform.position = Vector3.Lerp(this.transform.position, camPosition, camSpeed * Time.deltaTime);
        cam.orthographicSize = camSize;
    }
}
