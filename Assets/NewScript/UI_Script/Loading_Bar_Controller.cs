using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Loading_Bar_Controller : MonoBehaviour
{
    static string nextScene;

    [SerializeField]
    Image prograssBar;

    //private string SaveSceneName;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if(op.progress < 0.9f)
            {
                prograssBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                prograssBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if(prograssBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }  


}
