using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject recordTemplate;    
    public GameObject recordPanel;    
    public static Menu Instance { get; private set; }

    private List<GameObject> loadedRecordsList = new List<GameObject>();

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

        loadedRecordsList = new List<GameObject>();
    }
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void ShowRecords()
    {
        foreach(var r in loadedRecordsList)
        {
            GameObject.Destroy(r);
        }
        loadedRecordsList.Clear();
        var loadedRecords = LoadRecords();
        foreach (var r in loadedRecords.Select((value, i) => new { i, value }))
        {
            var recordInst = Instantiate(recordTemplate, recordPanel.transform);
            recordInst.transform.position = new Vector3(recordInst.transform.position.x, recordInst.transform.position.y - r.i*40, 0);

            recordInst.transform.Find("ScoreText").GetComponent<Text>().text = r.value.score.ToString();
            recordInst.transform.Find("TimeText").GetComponent<Text>().text = r.value.time.ToString("0.00");
            recordInst.transform.Find("NameText").GetComponent<Text>().text = r.value.name.ToString();
            loadedRecordsList.Add(recordInst);
        }
    }

    private void SaveRecord(List<RecordEntity> re)
    {
        PlayerPrefs.SetString("Records", JsonUtility.ToJson(new records() { recordsList = re.GetRange(0, Mathf.Min(10, re.Count)) }));
    }

    private List<RecordEntity> LoadRecords()
    {
        var loadedlist = JsonUtility.FromJson<records>(PlayerPrefs.GetString("Records"));
            return (loadedlist != null)? loadedlist.recordsList: new List<RecordEntity>();
    }

    public void UpdateRecords(float t, int score, string name)
    {
        var rec = new RecordEntity() { name = name, score = score, time = t };
        var loadedRecords = LoadRecords();
        loadedRecords.Add(rec);
        var resultRecords = loadedRecords.OrderByDescending(x => x.score);
        SaveRecord(resultRecords.ToList());
    }
    private class records
    {
        public List<RecordEntity> recordsList;
    }
    [System.Serializable]
    private class RecordEntity
    {
        public float time;
        public int score;
        public string name;
    }
}

