using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField]
    List<string> StageName;

    [SerializeField]
    private int StageCount = 3;

    private float currentSceneIndex;

    private bool isGoal = false;

    private GameObject player;

    private Vector2 playerMoveDir;

    private void Awake()
    {
        StageName = new List<string>();
        isGoal = false;

        for (int i = 1; i < StageCount+1; ++i)
        {
            StageName.Add($"Stage{i}");
        }

        player = GameManager.Instance.GetPlayer;
    }


    private void FixedUpdate()
    {
        if(isGoal) 
        {
            player.GetComponent<Player_Controller>().PortalMove(playerMoveDir);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.CompareTag("Player"))
        {
            isGoal = true;
            if(collision.transform.position.x - transform.position.x < 0f)
            {
                playerMoveDir = Vector2.left;
            }else
            {
                playerMoveDir = Vector2.right;
            }
            StartCoroutine(GoToNextScene(collision));                       
        }
    }

    private IEnumerator GoToNextScene(Collider2D collision)
    {
        yield return new WaitForSeconds(1f);
        NextScene();
        yield break;
    }

    private void NextScene()
    {
        GameManager.Instance.GameSave();
        Loading_Bar_Controller.LoadScene(StageName[(int)currentSceneIndex + 1]);
    }
}
