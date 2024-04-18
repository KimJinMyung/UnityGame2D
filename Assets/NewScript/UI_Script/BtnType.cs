using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BTNType
{
    New,
    Continue,
    Restart,
    MainMenu,
    Back,
    Exit
}

public class BtnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void NewGameStart()
    {
        GameManager.Instance.newGame();
        //SceneManager.LoadScene("Stage1");
        Loading_Bar_Controller.LoadScene("Stage1");
    }

    public void Loading_SaveGame()
    {
        GameManager.Instance.GameLoad();
        if(GameManager.Instance.SaveSceneName !=null)
            Loading_Bar_Controller.LoadScene(GameManager.Instance.SaveSceneName);
        else
            Loading_Bar_Controller.LoadScene("Stage1");
    }

    [SerializeField]
    private BTNType currentType;

    public Transform buttonScale;
    Vector3 defaultScale;

    private void Start()
    {
        buttonScale = GetComponent<RectTransform>();
        defaultScale = buttonScale.GetComponent<RectTransform>().localScale;
    }

    public void OnBtnClick()
    {
        switch (currentType)
        {
            case BTNType.New:
                NewGameStart();
                break;
            case BTNType.Continue:
                Loading_SaveGame();
                break;
            case BTNType.Exit:
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                break;
            case BTNType.Back:
                Game_UI_Manager.Instance.subMenu();
                break;
            case BTNType.Restart:
                Loading_SaveGame();
                //GameManager.Instance.GameSave();
                //Game_UI_Manager.Instance.subMenu();
                break;
            case BTNType.MainMenu:
                Loading_Bar_Controller.LoadScene("Title");
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
