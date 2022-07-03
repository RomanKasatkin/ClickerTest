using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject loseMenu;
    public InputField nameInput;
    public AudioSource tapSound;
    public bool isDead { get; private set; }

    public float camOffsetX = 1;
    public float camOffsetY = 1;
    private int score = 0;    
    public Text timeTextField;
    public Text scoreTextField;
    public Camera mainCamera;
    public float cameraSpeed = 1f;

    private float gameTime = 0;

    public GameObject URIndicator;
    public GameObject ULIndicator;
    public GameObject DRIndicator;
    public GameObject DLIndicator;

    private Ray ray;
    private RaycastHit hit; 
    private void Awake()
    {        
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        isDead = false;
    }
    void Start()
    {
        score = 0;
        gameTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        URIndicator.SetActive(false);
        ULIndicator.SetActive(false);
        DRIndicator.SetActive(false);
        DLIndicator.SetActive(false);
        CheckEnemiesOutView();
        if (!isDead)
        {
            gameTime += Time.deltaTime;
            timeTextField.text = gameTime.ToString("0.00");
            scoreTextField.text = score.ToString();
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    float x = mainCamera.transform.position.x - Input.GetTouch(0).deltaPosition.x;
                    float z = mainCamera.transform.position.z - Input.GetTouch(0).deltaPosition.y;

                    //Liming camera over field
                    if (x < -90)
                    {
                        x = -90;
                    }
                    else if (x > 90)
                    {
                        x = 90;
                    }

                    if (z < -103)
                    {
                        z = -103;
                    }
                    else if (z > 80)
                    {
                        z = 80;
                    }

                    var resultCameraPosition = new Vector3(x, mainCamera.transform.position.y, z);
                        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, resultCameraPosition, Time.deltaTime * cameraSpeed);
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        foreach (var mb in hit.collider.gameObject.GetComponents<MonoBehaviour>())
                        {
                            if (mb is IClickable)
                            {
                                Debug.Log("Touch");
                                ((IClickable)mb).DoClickAction();
                            }                                
                        }
                    }
                }
            }
        }        
    }

    public float GetGameTime()
    {
        return gameTime;
    }

    public float GetScore()
    {
        return score;
    }

    private bool CheckEnemiesOutView()
    {
        var cRay = Camera.main.ScreenPointToRay(Camera.main.transform.forward);
        RaycastHit cHit;
        Physics.Raycast(cRay, out cHit);
        
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var e in allEnemies)
        {
            if (e.transform.position.x > cHit.point.x + camOffsetX && e.transform.position.z > cHit.point.z + camOffsetY)
            {
                if (CheckObjectVisibility(e.transform.position))
                {
                    URIndicator.SetActive(true);
                }
            }
            if (e.transform.position.x < cHit.point.x + camOffsetX && e.transform.position.z > cHit.point.z + camOffsetY)
            {
                if (CheckObjectVisibility(e.transform.position))
                {
                    ULIndicator.SetActive(true);
                }
            }
            if (e.transform.position.x > cHit.point.x + camOffsetX && e.transform.position.z < cHit.point.z + camOffsetY)
            {
                if (CheckObjectVisibility(e.transform.position))
                {
                    DRIndicator.SetActive(true);
                }
            }
            if (e.transform.position.x < cHit.point.x + camOffsetX && e.transform.position.z < cHit.point.z + camOffsetY)
            {
                if (CheckObjectVisibility(e.transform.position))
                {
                    DLIndicator.SetActive(true);
                }
            }
        }
        return false;
    }
    public bool CheckObjectVisibility(Vector3 v)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(v);
        return !(viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0);
    }

    public void GainScore(int s)
    {
        score += s;
    }

    public void CommitDefeat()
    {
        isDead = true;
        loseMenu.SetActive(true);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveResult()
    {
        Menu.Instance.UpdateRecords(gameTime, score, nameInput.text);
        SceneManager.LoadScene("Menu");
    }
}
