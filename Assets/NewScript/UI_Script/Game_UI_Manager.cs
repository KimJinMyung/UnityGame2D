using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Game_UI_Manager : singleTone<Game_UI_Manager>
{
    [Header("인벤토리 UI")]
    [SerializeField]
    private Image[] Inven_Magazine_Img;

    [Header("소지 탄창 UI")]
    [SerializeField]
    private Image Grip_Magazine_Img;

    [Header("탄창 이미지")]
    [SerializeField]
    private Sprite Print_Magazine_Img_Exist;
    [SerializeField]
    private Sprite Print_Magazine_Img_Empty;    

    private Animator UI_Grip_Aimator;
    private Animator UI_Grip_Panel_Aimator;

    private Animator[] UI_Inven_Aimator;
    private Animator[] UI_Inven_Panel_Aimator;

    [Header("플레이어 상태 표시")]
    [SerializeField]
    private Text player_Condition_Text;
    [SerializeField]
    private Text player_Focus_Text;

    [Header("튜토리얼 텍스트")]
    [SerializeField]
    private GameObject Turotial_Text;

    private Image[] Inven_BulletGauge_Bar;
    private Image Grip_BulletGauge_Bar;

    [Header("서브 메뉴 UI")]
    public GameObject SubMenu;

    [Header("총 애니메이션 UI")]
    [SerializeField]
    private GameObject Gun_UI;

    private Animator Gun_Animation;

    private bool isSubMenuPressed = false;
    private bool isTutorialPressed = false;

    private TMP_Text CrouchText;
    private TMP_Text AttackText;
    private TMP_Text HeadAiming;
    private TMP_Text Inven_Text;
    private TMP_Text PickUp_Text;
    private TMP_Text Equip_Text;
    private TMP_Text UnEquip_Text;
    private TMP_Text BackSlideText;
    private TMP_Text LegAiming;
    private TMP_Text LightAiming;
    private TMP_Text DashText;

    private Color Activecolor;
    private Color Unactivecolor;

    private bool isAimingAble;
    private bool isHaveItem;
    public bool isContactMagazine = false;

    private Player_Controller player;

    private void Awake()
    {
        UI_Grip_Aimator = Grip_Magazine_Img.gameObject.GetComponent<Animator>();
        UI_Grip_Panel_Aimator = Grip_Magazine_Img.gameObject.transform.parent.parent.parent.GetComponent<Animator>();

        UI_Inven_Aimator = new Animator[Inven_Magazine_Img.Length];
        UI_Inven_Panel_Aimator = new Animator[Inven_Magazine_Img.Length];

        Inven_BulletGauge_Bar = new Image[Inven_Magazine_Img.Length];

        CrouchText = Turotial_Text.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        AttackText = Turotial_Text.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        HeadAiming = Turotial_Text.transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        Inven_Text = Turotial_Text.transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>();
        PickUp_Text = Turotial_Text.transform.GetChild(4).GetChild(0).GetComponent<TMP_Text>();
        Equip_Text = Turotial_Text.transform.GetChild(5).GetChild(0).GetComponent<TMP_Text>();
        UnEquip_Text = Turotial_Text.transform.GetChild(6).GetChild(0).GetComponent<TMP_Text>();
        BackSlideText = Turotial_Text.transform.GetChild(8).GetChild(0).GetComponent<TMP_Text>();
        LegAiming = Turotial_Text.transform.GetChild(9).GetChild(0).GetComponent<TMP_Text>();
        LightAiming = Turotial_Text.transform.GetChild(10).GetChild(0).GetComponent<TMP_Text>();
        DashText = Turotial_Text.transform.GetChild(11).GetChild(0).GetComponent<TMP_Text>();

        Gun_Animation = Gun_UI.GetComponent<Animator>();

        player = GameManager.Instance.GetPlayer.GetComponent<Player_Controller>();
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

        SubMenu.SetActive(false);
        

        Activecolor = new Color(255f, 159f, 0f);
        Unactivecolor = new Color(0, 0, 0, 150f);

        Default_LightAiming_Color();
        Default_MonsterAiming_Color();

        //if(player.playerInventory.grip != null)
        //    Print_Player_Grip_Ainimation();
    }

    private void Change_DashText_Text()
    {
        if (GameManager.Instance.GetPlayer.GetComponent<Player_Controller>().isDashing)
        {
            DashText.color = Unactivecolor;
        }
        else
        {
            DashText.color = Color.white;
        }
    }

    private void Change_AttackText_Color()
    {        
        AttackText.color = Activecolor;
    }
    public void Change_CrouchText_Color()
    {
        CrouchText.color = Activecolor;
    }

    private void Change_CrouchText()
    {
        if (player.isBleeding)
        {
            CrouchText.text = "지혈하기 : ";
            Change_CrouchText_Color();
        }
        else if(player.isContactCover && player.state != PlayerState.Crouch)
        {
            CrouchText.text = "엄폐하기 : ";
            Change_CrouchText_Color();
        }
        else if(player.state == PlayerState.Crouch)
        {
            CrouchText.text = "일어나기 : ";
            Change_CrouchText_Color();
        }
        else 
        {
            CrouchText.text = "웅크리기 : ";
            CrouchText.color = Color.white;
        }  
    }


    public void Change_MonsterAiming_Color()
    {
        if (isAimingAble)
        {
            if(player.focus > 50f)
            {
                HeadAiming.color = Activecolor;
                LegAiming.color = Activecolor;
            }
            else
            {
                HeadAiming.color = Color.white;
                LegAiming.color = Color.white;
            }
        }              
    }

    public void Default_MonsterAiming_Color()
    {
        HeadAiming.color = Unactivecolor;
        LegAiming.color = Unactivecolor;
    }

    public void Change_LightAiming_Color()
    {
        if (isAimingAble)
        {
            LightAiming.color = Activecolor;
        }
    }
    public void Default_LightAiming_Color()
    {
        LightAiming.color = Unactivecolor;
    }

    public void Change_Inven_Text_Color()
    {
        Inven_Text.color = Activecolor;
    }

    private void Change_PickUpText_Color()
    {
        if(player.playerInventory.grip != null)
        {
            PickUp_Text.text = "탄창 버리기 : ";
            PickUp_Text.color = Activecolor;
        }
        else if (isContactMagazine)
        {
            PickUp_Text.text = "탄창 줍기 : ";
            PickUp_Text.color = Activecolor;
        }else if (player.isContectDepot && Array.Exists(player.playerInventory.Inven, item => item != null))
        {           
            PickUp_Text.text = "탄알 충전 : ";
            PickUp_Text.color = Activecolor;
        }
        else
        {
            PickUp_Text.text = "탄창 줍기 : ";
            PickUp_Text.color = Unactivecolor;
        }        
    }

    private void Change_EquipText_Color()
    {
        Equip_Text.color = Activecolor;
    }

    void FixedUpdate()
    {
        Print_Player_Status();

        Change_CrouchText();
        ChangeAttackTextColor();
        ChangeInvenTextColor();
        Change_PickUpText_Color();
        ChangeEqiupText();
        ChangeUnEquipText();
        ChangeBackSlideText();
        Change_DashText_Text();

        if (player.playerInventory.grip != null)
        {
            Print_Player_Grip_Ainimation();
        }
        else
        {
            Drop_UI_Aimation();
        }
    }

    private void ChangeBackSlideText()
    {
        if (!player.playerGun.equipedBullet)
        {
            BackSlideText.color = Activecolor;
        }
        else
        {
            BackSlideText.color = Unactivecolor;
        }        
    }

    private void ChangeUnEquipText()
    {
        if(player.playerInventory.grip != null)
        {
            UnEquip_Text.text = "탄창 버리기 : ";
            UnEquip_Text.color = Activecolor;
        }
        else if(player.playerGun.equipedMagazine != null)
        {
            if(player.playerGun.equipedMagazine.bulletCount > 0)
            {
                UnEquip_Text.text = "탄창 제거 : ";
                UnEquip_Text.color = Activecolor;
            }
            else
            {
                UnEquip_Text.color = Color.white;
            }
        }
        else
        {
            UnEquip_Text.color = Unactivecolor;
        }        
    }

    private void ChangeEqiupText()
    {
        if(player.playerInventory.grip != null)
        {
            Change_EquipText_Color();
        }
        else
        {
            Equip_Text.color = Unactivecolor;
        }
    }

    private void ChangeAttackTextColor()
    {
        if (player.playerGun.equipedBullet)
        {
            Change_AttackText_Color();
            isAimingAble = true;
        }
        else
        {
            AttackText.color = Unactivecolor;
            isAimingAble = false;
        }
    }

    private void ChangeInvenTextColor()
    {
        var inven = player.playerInventory.Inven;
        for (int i = 0; i< inven.Length; i++)
        {
            if (inven[i] != null && inven[i].bulletCount > 0f)
            {
                isHaveItem = true;
                break;
            }
            else
            {
                isHaveItem = false;
            }
        }

        if (isHaveItem)
        {
            Change_Inven_Text_Color();
        }
        else
        {
            Inven_Text.color = Unactivecolor;
        }
    }       
        private void Print_Player_Status()
    {
        player_Condition_Text.text = $"Condition : {player.condition}%";
        player_Focus_Text.text = $"Focus : {player.focus}%";
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
        Grip_BulletGauge_Bar.gameObject.SetActive(true);

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

    public void Print_Player_Grip()
    {
        Grip_BulletGauge_Bar.gameObject.SetActive(false);
    }

    public void Print_Player_InvenSlot(int index)
    {
        Inven_Magazine_Img[index].gameObject.SetActive(false);
    }

    public void Print_Player_Grip_Ainimation()
    {        
        if (!UI_Grip_Aimator.GetBool("Grip_Pressed"))
        {
            UI_Grip_Panel_Aimator.SetTrigger("Up");
            Grip_Magazine_Img.gameObject.SetActive(true);

            UI_Grip_Aimator.SetTrigger("Grip");
            UI_Grip_Aimator.SetBool("Grip_Pressed", true);
        }              
    }


    public void Init_Player_Grip()
    {
        Grip_Magazine_Img.gameObject.SetActive(false);
        Grip_Magazine_Img.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -320);
    }

    public void Init_Player_Inven()
    {
        foreach (var item in Inven_Magazine_Img)
        {
            item.gameObject.SetActive(false);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -320);
        }
    }

    public void Drop_UI_Aimation()
    {
        if (UI_Grip_Aimator.GetBool("Grip_Pressed"))
        {
            UI_Grip_Aimator.SetTrigger("Drop");
            UI_Grip_Aimator.SetBool("Grip_Pressed", false);

            //UI_Grip_Panel_Aimator.SetTrigger("Down");
            UI_Grip_Panel_Aimator.SetTrigger("Down");

            StartCoroutine(SlowActiveOff_Grip());
            //StopCoroutine(Drop_Animation_UI());
        }
    }

    private IEnumerator SlowActiveOff_Grip()
    {
        yield return new WaitForSeconds(0.3f);
        Grip_Magazine_Img.gameObject.SetActive(false);
        yield break;
    }

    public void OnSubMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            subMenu();
        }
    }

    public void subMenu()
    {
        if (isSubMenuPressed)
        {
            //일시 정지 해제
            Time.timeScale = 1;
            //눌린적 있는지 판별
            isSubMenuPressed = false;
            //sub 메뉴 off
            SubMenu.SetActive(false);
        }
        else
        {
            //일시 정지
            Time.timeScale = 0;
            isSubMenuPressed = true;
            //sub 메뉴 on
            SubMenu.SetActive(true);
        }
    }

    public void OnSet_ShowTextUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetTutorial_TextUI();
        }
    }

    private void SetTutorial_TextUI()
    {
        if (isTutorialPressed)
        {
            isTutorialPressed = false;
            Turotial_Text.SetActive(true);
        }
        else
        {
            isTutorialPressed = true;
            Turotial_Text.SetActive(false);
        }
    }

    public void BackSlide_Animation(bool fail, bool equipedBullet)
    {
        if (equipedBullet)
        {
            Gun_Animation.SetBool("Empty", false);
            if (fail)
            {
                Gun_Animation.SetBool("Success", false);
            }
            else
            {
                Gun_Animation.SetBool("Success", true);
            }            
        }
        else
        {
            Gun_Animation.SetBool("Empty", true);
        }

        Gun_Animation.SetTrigger("BackSlide");
    }    

    public void Check_EquipedBullet_Animation(bool equipedBullet)
    {
        if(equipedBullet)
        {
            Gun_Animation.SetBool("Empty", false);
        }
        else
        {
            Gun_Animation.SetBool("Empty", true);
        }
        Gun_Animation.SetBool("Checking", true);
    }

    public void End_Check_EquipedBullet_Animation()
    {
        Gun_Animation.SetBool("Checking", false);
    }
    
}
