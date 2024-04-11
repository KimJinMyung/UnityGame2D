using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackTiming : MonoBehaviour
{
    private Player_Controller player;

    private void Start()
    {
        player = transform.parent.parent.parent.GetComponent<Player_Controller>();
    }
    void Attack()
    {
        if (player.target != null)
        {
            player.target.Hurt(player.AttackDamage);
            Debug.Log("АјАн ");
        }
    }
}
