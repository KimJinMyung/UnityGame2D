using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackTiming : MonoBehaviour
{
    private Player_Controller player;
    private float Accuracy;

    private void Start()
    {
        player = transform.parent.parent.parent.GetComponent<Player_Controller>();
    }
    void Attack()
    {
        if (player.target != null)
        {
            if(player.focus > 30f)
            {
                Accuracy = 100f;
            }
            else
            {
                Accuracy = player.focus * 1.5f;
            }

            float AttackSuccess = Random.Range(0, 100f);
            if(AttackSuccess <=  Accuracy)
            {
                player.target.Hurt(player.AttackDamage);
            }
            else
            {
                Debug.Log("ºø³ª°¨...");
            }
        }

        player.AttackBackSlide();
    }

    void AttackDelay()
    {
        player.state = PlayerState.Idle;
    }
}
