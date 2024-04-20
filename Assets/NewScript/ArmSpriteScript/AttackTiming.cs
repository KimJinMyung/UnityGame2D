using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackTiming : MonoBehaviour
{
    private Player_Controller player;
    private float Accuracy;

    private bool isIncresased = true;

    private void Start()
    {
        player = transform.parent.parent.parent.GetComponent<Player_Controller>();

        Accuracy = 100;
    }
    void Attack()
    {
        if(player.Lights_target != null)
        {
            player.Lights_target.Hurt();
        }
        else if (player.Monster_target != null)
        {
            if(player.focus > 50f)
            {
                Accuracy = 100f;
            }
            else
            {
                Accuracy = Mathf.Clamp(player.focus * 1.5f, 10f, 100f);
            }

            float AttackSuccess = Random.Range(0, 100f);
            if(AttackSuccess <=  Accuracy)
            {
                player.Monster_target.Hurt(player.AttackDamage);
            }            
        }
        
        player.AttackBackSlide();
    }

    void AttackDelay()
    {
        player.state = PlayerState.Idle;
    }

    private void Under_Light_Accuracy()
    {
        if (player.isLightUnder && !isIncresased)
        {
            isIncresased = true;
            Accuracy = Mathf.Clamp(Accuracy + 15, 0f, 100f);
        }else if(!player.isLightUnder && isIncresased)
        {
            isIncresased = false;
            Accuracy = Mathf.Clamp(Accuracy - 15, 0f, 100f);
        }
    }

    private void Update()
    {
        Under_Light_Accuracy();
    }    
}
