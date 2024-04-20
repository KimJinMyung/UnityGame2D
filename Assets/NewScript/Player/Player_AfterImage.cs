using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_AfterImage : MonoBehaviour
{
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    [SerializeField]
    private float alpha;    // ≈ı∏Ìµµ
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;


    private Transform player;

    private SpriteRenderer[] SR;
    private SpriteRenderer Player_BodySR;
    private SpriteRenderer[] Player_ArmsSR;

    private Color color;

    private void OnEnable()
    {
        SR = new SpriteRenderer[3];

        SR[0] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SR[1] = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SR[2] = transform.GetChild(0).GetComponent<SpriteRenderer>();

        player = GameManager.Instance.GetPlayer.transform;
        Player_BodySR = player.GetChild(0).GetComponent<SpriteRenderer>();

        Player_ArmsSR = new SpriteRenderer[2];

        Player_ArmsSR[0] = player.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        Player_ArmsSR[1] = player.GetChild(0).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
    
        alpha = alphaSet;
        SR[0].sprite = Player_BodySR.sprite;

        transform.position = player.GetChild(0).transform.position;
        transform.rotation = player.GetChild(0).transform.rotation;

        timeActivated = Time.time;
    }

    private void Update()
    { 
        alpha *= alphaMultiplier;
        color = new Color(255f, 255f, 255f, alpha);
        foreach(var item in SR)
        {
            item.color = color;
        }

        if(Time.time > (timeActivated + activeTime))
        {
            //Add back to Pool
            ImagePoolManager.Instance.OnRelease(gameObject);
        }
    }
}
