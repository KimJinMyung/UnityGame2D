using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArmScripts : MonoBehaviour
{
    private Player_Controller player;

    private Animator bodyAnimator;
    private Animator RightArmAimator;
    private Animator LeftArmAimator;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player_Controller>();
    }

    void Start()
    {
        bodyAnimator = GetComponent<Animator>();
        RightArmAimator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        LeftArmAimator = transform.GetChild(0).GetChild(1).GetComponent<Animator>();
    }

    void ArmActive()
    {
        RightArmAimator.gameObject.SetActive(true);
        LeftArmAimator.gameObject.SetActive(true);
        bodyAnimator.SetBool("PickUp", false);
        transform.GetChild(0).transform.localPosition = new Vector2(-0.15f, 0.15f);

        //player.state = PlayerState.Idle;
    }

    void ArmUnActive()
    {
        LeftArmAimator.gameObject.SetActive(false);
        RightArmAimator.gameObject.SetActive(false);
    }   

    void Hurt_After()
    {
        player.state = PlayerState.Idle;
    }    

    void ReCharge_Bullet()
    {
        GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().ReCharge_Inventory_Magazine();
    }
}
