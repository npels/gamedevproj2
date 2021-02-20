using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private AsyncOperation sceneAsync;

    public static GameManager instance;

    public bool inEditor;
    public bool inRespawn = false;

    public GameObject playerPrefab;

    public GameObject playerParentObject;

    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public List<GameObject> creatures;

    [HideInInspector]
    public int numRandomFood = 0;
    
    public static int currentDNA = 0;
    public static int totalCollectedDNA = 0;
    public TMPro.TMP_Text dnaText;

    public string gameSceneName;
    public string editorSceneName;
    public string respawnSceneName;

    public Vector3 editorPlayerLocation;
    public Vector3 gamePlayerLocation;

    public Image blackoutImage;
    public float fadeSpeed;

    public Slider healthBar;

    public Image blindMaskImage;

    private bool loading = false;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        if (inRespawn) {
            StartCoroutine(FadeIn());
            return;
        }

        player = GameObject.Find("Player(Clone)");
        
        dnaText.text = currentDNA.ToString();

        if (player == null) {
            player = Instantiate(playerPrefab);
        }

        if (inEditor) {
            player.transform.position = editorPlayerLocation;
            player.transform.rotation = Quaternion.Euler(Vector3.up);
        } else {
            player.transform.position = gamePlayerLocation;
            player.transform.parent = playerParentObject.transform;

            player.GetComponent<Creature>().CheckParts();
            blindMaskImage.enabled = player.GetComponent<Creature>().numEyes == 0;
        }

        creatures = new List<GameObject>();
        creatures.Add(player);

        StartCoroutine(FadeIn());
    }

    private void Update() {
        if (inRespawn) {
            if (Input.GetMouseButtonDown(0)) {
                Respawn();
            }
            return;
        }
        if (player == null) {
            player = GameObject.Find("Player(Clone)");

            if (inEditor) {
                player.transform.position = editorPlayerLocation;
                player.transform.rotation = Quaternion.Euler(Vector3.up);
                //player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            } else if (player != null) {
                player.transform.position = gamePlayerLocation;
                player.GetComponent<Creature>().CheckParts();
                blindMaskImage.enabled = player.GetComponent<Creature>().numEyes == 0;
            }
        }

        if (!inEditor && player != null && player.transform.parent == null && playerParentObject != null) {
            player.transform.parent = playerParentObject.transform;
        }
    }

    public void ChangeDNA(int amount) {
        currentDNA += amount;
        if (amount > 0) totalCollectedDNA += amount;
        dnaText.text = currentDNA.ToString();
    }

    public void ChangeHealthSlider(float healthFraction) {
        healthBar.value = healthFraction;
    }

    public void GoToGameScene() {
        StartCoroutine(LoadScene(editorSceneName, gameSceneName));
        inEditor = false;
    }

    public void GoToEditorScene() {
        StartCoroutine(LoadScene(gameSceneName, editorSceneName));
        inEditor = true;
    }

    public void GoToRespawnScreen() {
        StartCoroutine(LoadScene(gameSceneName, respawnSceneName));
        inRespawn = true;
    }

    public void Respawn() {
        StartCoroutine(LoadScene(respawnSceneName, gameSceneName));
        inRespawn = false;
    }

    public GameObject GetClosestCreature(GameObject source) {
        float closestDistance = float.MaxValue;
        GameObject closestObject = null;
        foreach (GameObject creature in creatures) {
            float dist = Vector3.Distance(source.transform.position, creature.transform.position);
            if (dist < closestDistance && source != creature) {
                closestObject = creature;
                closestDistance = dist;
            }
        }
        return closestObject;
    }

    private IEnumerator LoadScene(string fromScene, string toScene) {
        if (loading) yield break;
        loading = true;
        sceneAsync = SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
        sceneAsync.allowSceneActivation = false;

        Color color = Color.clear;

        while (color.a < 0.95f) {
            color.a = Mathf.Lerp(color.a, 1, fadeSpeed * Time.deltaTime);
            blackoutImage.color = color;
            yield return null;
        }

        blackoutImage.color = Color.black;

        while (sceneAsync.progress < 0.9f) {
            yield return null;
        }

        sceneAsync.allowSceneActivation = true;

        while (!sceneAsync.isDone) {
            yield return null;
        }

        FinishLoadingScenes(fromScene, toScene);
    }

    private IEnumerator FadeIn() {

        Color color = Color.black;

        while (color.a > 0.05f) {
            color.a = Mathf.Lerp(color.a, 0, fadeSpeed * Time.deltaTime);
            blackoutImage.color = color;
            yield return null;
        }

        blackoutImage.color = Color.clear;
    }

    private void FinishLoadingScenes(string fromScene, string toScene) {

        Scene scene = SceneManager.GetSceneByName(toScene);
        if (scene.IsValid()) {
            player.transform.parent = null;
            player.SetActive(!inRespawn);
            SceneManager.MoveGameObjectToScene(player, scene);
            SceneManager.SetActiveScene(scene);
            SceneManager.UnloadSceneAsync(fromScene);
        }
        loading = false;
    }

    
}
