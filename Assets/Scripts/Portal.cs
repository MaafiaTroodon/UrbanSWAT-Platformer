
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Load Target")]
    public string sceneName = "";   // if empty, uses build index
    public int sceneBuildIndex = -1; // -1 = use next index (current+1)

    [Header("Who can use it")]
    public string requiredTag = "Player"; // ensure your player GameObject has this Tag

    [Header("Loading")]
    public bool asyncLoad = true;   // smoother loading
    public float delayBeforeLoad = 0.2f;

    void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        if (asyncLoad) StartCoroutine(LoadRoutine());
        else LoadNow();
    }

    System.Collections.IEnumerator LoadRoutine()
    {
        if (delayBeforeLoad > 0f) yield return new WaitForSeconds(delayBeforeLoad);

        if (!string.IsNullOrEmpty(sceneName))
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            yield return op;
        }
        else
        {
            int target = sceneBuildIndex >= 0
                ? sceneBuildIndex
                : SceneManager.GetActiveScene().buildIndex + 1;
            var op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Single);
            yield return op;
        }
    }

    void LoadNow()
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
        else
        {
            int target = sceneBuildIndex >= 0
                ? sceneBuildIndex
                : SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(target);
        }
    }
}
