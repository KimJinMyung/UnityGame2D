using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField]
    List<string> StageName;

    [SerializeField]
    private int StageCount = 2;

    private float currentSceneIndex;

    private Vector2 playerMoveVelocity;

    private void Awake()
    {
        StageName = new List<string>();

        for (int i = 1; i < StageCount+1; ++i)
        {
            StageName.Add($"Stage{i}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.CompareTag("Player"))
        {
            playerMoveVelocity = collision.transform.parent.parent.GetComponent<Rigidbody2D>().velocity;
            currentSceneIndex = StageName.IndexOf(SceneManager.GetActiveScene().name);
            
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
        Loading_Bar_Controller.LoadScene(StageName[(int)currentSceneIndex + 1]);
    }
}
