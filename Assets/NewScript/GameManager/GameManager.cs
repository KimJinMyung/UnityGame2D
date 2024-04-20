using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameManager;

 [System.Serializable]
    public class SaveData
    {
        public bool isNewGame;
        public string stageName;
        public float playerCondition;
        public int PlayerInventory_Grip;
        public int[] PlayerInventory_Inven = new int[4];
        public int PlayerGun_EquipedMagazine;
        public bool PlayerGun_EquipedBullet;
        // Add other variables for inventory and gun status
    }

public class GameManager : singleTone1<GameManager>
{
   

    [SerializeField]
    private GameObject player;
    public GameObject GetPlayer
    {
        get { return player; }
    }

    private Player_Controller playerController;

    public bool isSaveGame {  get; private set; }

    public string SaveSceneName {  get; private set; }

    private string json;

    private string _path;

    private void Awake()
    {
        _path = Application.persistentDataPath + "/fileName.json";

        playerController = player.GetComponent<Player_Controller>();

        GameLoad();

        if (SceneManager.GetActiveScene().name != "Title")
        {
            GameSave();
        }        
    }

    

    //private void Start()
    //{
            
    //}
    

    public void GameSave()
    {
        isSaveGame = true;

        SaveData saveData = new SaveData();        

        //처음으로 만들어졌는지 판별문 저장
        saveData.isNewGame = isSaveGame;

        //현재 씬 이름 저장
        saveData.stageName = SceneManager.GetActiveScene().name;

        //체력 수치 저장
        saveData.playerCondition = player.GetComponent<Player_Controller>().condition * 1f;

        //인벤토리 저장
        if (player.GetComponent<Player_Controller>().playerInventory.grip != null)
        {
            saveData.PlayerInventory_Grip = player.GetComponent<Player_Controller>().playerInventory.grip.bulletCount;
        }
        else
        {
            saveData.PlayerInventory_Grip = -1;
        }
            

        for (int i = 0; i< 4; i++)
        {
            if (player.GetComponent<Player_Controller>().playerInventory.Inven[i] == null) 
            {
                saveData.PlayerInventory_Inven[i] = -1;
            }
            else
            {
                saveData.PlayerInventory_Inven[i] = player.GetComponent<Player_Controller>().playerInventory.Inven[i].bulletCount;
            }
        }

        //총 상태 저장
        if (player.GetComponent<Player_Controller>().playerGun.equipedMagazine != null) 
        {
            saveData.PlayerGun_EquipedMagazine = player.GetComponent<Player_Controller>().playerGun.equipedMagazine.bulletCount;
        }
        else
        {
            saveData.PlayerGun_EquipedMagazine = -1;
        }
            
        saveData.PlayerGun_EquipedBullet = player.GetComponent<Player_Controller>().playerGun.equipedBullet;

        json = JsonUtility.ToJson(saveData);
        File.WriteAllText(_path, json);

    }

    public void GameLoad()
    {
        //if (File.Exists(_path) == false)        
        //{
        //    int[] Inven_Bullets = new int[4];

        //    for (int i = 0; i < 4; i++)
        //    {
        //        Inven_Bullets[i] = -1;
        //    }

        //    player.GetComponent<Player_Controller>().LoadData(100, -1, Inven_Bullets, -1, false);
        //    return;
        //}


        string file = File.ReadAllText(_path);
        SaveData saveDatas = JsonUtility.FromJson<SaveData>(file);

        isSaveGame = saveDatas.isNewGame;
        SaveSceneName = saveDatas.stageName;

        float condition;
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            condition = 100f;
        }
        else
        {
            condition = saveDatas.playerCondition;
        }

        int grip_Bullet = saveDatas.PlayerInventory_Grip;

        int[] Inven_Bullet = new int[4];

        for (int i = 0; i < 4; i++)
        {
            Inven_Bullet[i] = saveDatas.PlayerInventory_Inven[i];
        }

        int EquipedMagazine_Bullet = saveDatas.PlayerGun_EquipedMagazine;

        bool EquipedBullet = saveDatas.PlayerGun_EquipedBullet;        

        GetPlayer.GetComponent<Player_Controller>().LoadData(condition, grip_Bullet, Inven_Bullet, EquipedMagazine_Bullet, EquipedBullet);

        //일시 정지 해제
        Time.timeScale = 1;
    }

    public void newGame()
    {
        isSaveGame = false;
        //string path = Path.Combine(Application.persistentDataPath, "GameData.json");

        if (File.Exists(_path))
        {
            File.Delete(_path);
        }           

        SaveSceneName = null;
    }
}
