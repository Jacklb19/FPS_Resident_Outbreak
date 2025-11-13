using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

using TMPro; 

public class LoadingScreenController : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingText; 
    void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(GameManager.targetScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (loadingText != null)
                loadingText.text = $"Loading... {(progress * 100):0}%";

            if (operation.progress >= 0.9f)
            {
                if (loadingText != null)
                    loadingText.text = "Press any key to continue";

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}

