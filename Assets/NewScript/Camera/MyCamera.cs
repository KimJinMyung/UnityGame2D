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

    [SerializeField]
    private GameObject Ground;

    private float Min_Carmera_Pos;
    private float Max_Carmera_Pos;

    private void Awake()
    {
        camSize = 4.5f;
        cam = GetComponent<Camera>();

        //transform.position = GameManager.Instance.GetPlayer.transform.position;
    }

    private void Start()
    {
        Min_Carmera_Pos = -(Ground.transform.localScale.x / 2) + Ground.transform.position.x + 9.5f;
        Max_Carmera_Pos = (Ground.transform.localScale.x / 2) + Ground.transform.position.x - 9.5f;
    }

    private void FixedUpdate()
    {
        Vector3 ScreenAimPoint = Input.mousePosition;
       
        Vector3 offset = (ScreenAimPoint - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f,0)) * 0.0001f * camSize; //화면 중심 계산
        camPosition = GameManager.Instance.GetPlayer.transform.position + new Vector3(0, 2f, -10) + offset * camRange;
        Vector3 MoveScreen = Vector3.Lerp(this.transform.position, camPosition, camSpeed * Time.deltaTime);
        this.transform.position = new Vector3(Mathf.Clamp(MoveScreen.x, Min_Carmera_Pos, Max_Carmera_Pos), MoveScreen.y, MoveScreen.z);
        cam.orthographicSize = camSize;
    }
}
