using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround_Scroll : MonoBehaviour
{
    Vector2 PlayerVelocity;

    [SerializeField] private float MoveSpeed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        GetPlayerVelcity();
        Scrolling();
    }

    private void GetPlayerVelcity()
    {
        PlayerVelocity = GameManager.Instance.GetPlayer.GetComponent<Rigidbody2D>().velocity;
    }

    private void Scrolling()
    {
        if(PlayerVelocity.x > 0f)
        {
            transform.Translate(Vector2.left * MoveSpeed * Time.deltaTime);
        }else if(PlayerVelocity.x < 0f)
        {
            transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);
        }
    }
}
