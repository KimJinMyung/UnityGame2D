using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Depot : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isContectDepot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isContectDepot = false;
        }
    }
}
