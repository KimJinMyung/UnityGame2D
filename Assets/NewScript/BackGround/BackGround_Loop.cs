using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround_Loop : MonoBehaviour
{
    BoxCollider2D BoxCollider;

    Transform BackGround;

    private void Awake()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
        BackGround = this.transform.parent;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision.transform.position.x < (BoxCollider.transform.position.x - BoxCollider.size.x * 3 / 2))
            {
                BackGround.position = BackGround.position - new Vector3(25.8f, 0, 0);
            }else if(collision.transform.position.x > (BoxCollider.transform.position.x + BoxCollider.size.x * 3 / 2 / 2))
            {
                BackGround.position = BackGround.position + new Vector3(25.8f, 0, 0);
            }
        }
    }
}
