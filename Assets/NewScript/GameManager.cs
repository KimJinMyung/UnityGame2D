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

    [SerializeField]
    private Text player_Condition_Text;
    [SerializeField]
    private Text player_Focus_Text;


    private void Awake()
    {
        playerController = player.GetComponent<Player_Controller>();
        Inven_Magazine_Img = new Image[4];
        UI_Grip_Aimator = Grip_Magazine_Img.gameObject.GetComponent<Animator>();
        UI_Grip_Panel_Aimator = Grip_Magazine_Img.gameObject.transform.parent.parent.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Print_Player_Status();
    }

    public void Print_Player_Status()
    {
        player_Condition_Text.text = $"Condition : {playerController.condition}%";
        player_Focus_Text.text = $"Focus : {playerController.forcus}%";
    }

    public void Print_Player_InvenSlot(Item item, int index)
    {
        Inven_Magazine_Img[index].gameObject.SetActive(true);
        if (item.bulletCount <= 0)
        {
            Inven_Magazine_Img[index].sprite = Print_Magazine_Img_Empty;
        }
        else
        {
            Inven_Magazine_Img[index].sprite = Print_Magazine_Img_Exist;
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

    public void Print_Player_Grip(Item item)
    {
        if (item.bulletCount <= 0)
        {
            Grip_Magazine_Img.sprite = Print_Magazine_Img_Empty;
        }
        else
        {
            Grip_Magazine_Img.sprite = Print_Magazine_Img_Exist;
        }
    }

    public void Init_Player_Grip()
    {
        Grip_Magazine_Img.gameObject.SetActive(false);
        Grip_Magazine_Img.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -320);
    }

    public void Drop_UI_Aimation()
    {
        UI_Grip_Aimator.SetTrigger("Drop");
        //UI_Grip_Panel_Aimator.SetTrigger("Down");
        UI_Grip_Panel_Aimator.SetTrigger("Down");

        StartCoroutine(SlowActiveOff());
        //StopCoroutine(Drop_Animation_UI());
    }

    private IEnumerator SlowActiveOff()
    {
        yield return new WaitForSeconds(0.3f);
        Grip_Magazine_Img.gameObject.SetActive(false);
    }
}
