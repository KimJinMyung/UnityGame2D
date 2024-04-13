using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : singleTone<GameManager>
{
    [SerializeField]
    private GameObject player;
    public GameObject GetPlayer
    {
        get { return player; }
    }

    private Player_Controller playerController;


    [SerializeField]
    private Image[] Inven_Magazine_Img;
    [SerializeField]
    private Sprite Print_Magazine_Img_Exist;
    [SerializeField]
    private Sprite Print_Magazine_Img_Empty;
    [SerializeField]
    private Image Grip_Magazine_Img;

    private Animator UI_Grip_Aimator;
    private Animator UI_Grip_Panel_Aimator;

    private Animator[] UI_Inven_Aimator;
    private Animator[] UI_Inven_Panel_Aimator;

    [SerializeField]
    private Text player_Condition_Text;
    [SerializeField]
    private Text player_Focus_Text;

    private Image[] Inven_BulletGauge_Bar;
    private Image Grip_BulletGauge_Bar;

    private void Awake()
    {
        playerController = player.GetComponent<Player_Controller>();
        //Inven_Magazine_Img = new Image[4];
        UI_Grip_Aimator = Grip_Magazine_Img.gameObject.GetComponent<Animator>();
        UI_Grip_Panel_Aimator = Grip_Magazine_Img.gameObject.transform.parent.parent.parent.GetComponent<Animator>();

        UI_Inven_Aimator = new Animator[Inven_Magazine_Img.Length];
        UI_Inven_Panel_Aimator = new Animator[Inven_Magazine_Img.Length];

        Inven_BulletGauge_Bar = new Image[Inven_Magazine_Img.Length];
    }

    private void Start()
    {
        int index = 0;
        foreach (var item in Inven_Magazine_Img)
        {
            UI_Inven_Aimator[index] = item.gameObject.GetComponent<Animator>();
            UI_Inven_Panel_Aimator[index] = item.gameObject.transform.parent.parent.GetComponent<Animator>();

            Inven_BulletGauge_Bar[index] = item.gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            index++;
        }

        Grip_BulletGauge_Bar = Grip_Magazine_Img.gameObject.transform.GetChild(0).GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Print_Player_Status();
    }

    private void Print_Player_Status()
    {
        player_Condition_Text.text = $"Condition : {playerController.condition}%";
        player_Focus_Text.text = $"Focus : {playerController.focus}%";
    }

    public void Print_Player_InvenSlot_Up_Animation(int index)
    {
        //입력받은 번호의 인벤토리가 올라왔다가 1초 후에 다시 내려가는 애니메이션
        //UI_Grip_Panel_Aimator.SetTrigger("Up");
        UI_Inven_Panel_Aimator[index - 1].SetTrigger("Up");
        Inven_Magazine_Img[index - 1].gameObject.SetActive(true);
        UI_Inven_Aimator[index - 1].SetTrigger("Grip");

        //StartCoroutine(Print_Player_InvenSlot_Down_Animation(index));
    }
    

    public void Print_Player_InvenSlot(Item item, int index)
    {
        //Inven_Magazine_Img[index].gameObject.SetActive(true);
        if (item.bulletCount <= 0)
        {
            Inven_Magazine_Img[index].sprite = Print_Magazine_Img_Empty;
            Inven_BulletGauge_Bar[index].fillAmount = 0f;
        }
        else
        {
            Inven_Magazine_Img[index].sprite = Print_Magazine_Img_Exist;
            float bulletCountPer = (float)item.bulletCount / 15f;
            Inven_BulletGauge_Bar[index].fillAmount = bulletCountPer;
        }        
    }

    public void Print_Player_Grip(Item item)
    {
        if (item.bulletCount <= 0)
        {
            Grip_Magazine_Img.sprite = Print_Magazine_Img_Empty;
            Grip_BulletGauge_Bar.fillAmount = 0f;
        }
        else
        {
            Grip_Magazine_Img.sprite = Print_Magazine_Img_Exist;
            float bulletCountPer = (float)item.bulletCount / 15f;
            Grip_BulletGauge_Bar.fillAmount = bulletCountPer;
        }
    }

    public void Print_Player_InvenSlot(int index)
    {
        Inven_Magazine_Img[index].gameObject.SetActive(false);
    }

    public void Print_Player_Grip_Ainimation()
    {
        UI_Grip_Panel_Aimator.SetTrigger("Up");
        Grip_Magazine_Img.gameObject.SetActive(true);

        UI_Grip_Aimator.SetTrigger("Grip");
    }
    

    public void Init_Player_Grip()
    {
        Grip_Magazine_Img.gameObject.SetActive(false);
        Grip_Magazine_Img.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -320);
    }

    public void Init_Player_Inven()
    {
        foreach(var item in Inven_Magazine_Img)
        {
            item.gameObject.SetActive(false);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -320);
        }
    }   

    public void Drop_UI_Aimation()
    {
        UI_Grip_Aimator.SetTrigger("Drop");
        //UI_Grip_Panel_Aimator.SetTrigger("Down");
        UI_Grip_Panel_Aimator.SetTrigger("Down");

        StartCoroutine(SlowActiveOff_Grip());
        //StopCoroutine(Drop_Animation_UI());
    }

    private IEnumerator SlowActiveOff_Grip()
    {
        yield return new WaitForSeconds(0.3f);
        Grip_Magazine_Img.gameObject.SetActive(false);
        yield break;
    }
}
