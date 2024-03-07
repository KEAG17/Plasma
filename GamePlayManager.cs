using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayManager : MonoBehaviour
{

    public static GamePlayManager Instance;  // 靜態的單例實例
    [System.Serializable]
    public class MonsterOption
    {
        public IEnumerator SpawnMonsters(Transform[] spawnPoints)
        {


            // 在這裡使用 Instance.StartCoroutine
            yield return Instance.StartCoroutine(SpawnMonstersWithInterval(spawnPoints));
            //Debug.Log("調用生成SpawnMonsters");


        }



        [HideInInspector] public string displayName;  // 用來表示波次編號的名稱



        [Header("怪物預置物")]     public GameObject monsterPrefab;     // 怪物預置物
        [Header("生成間隔時間")]   public float spawnInterval;        // 生成間隔時間
        [Header("怪物生命值")]     public float monsterHealth;          // 怪物生命值
        [Header("怪物掉落物品")]   public GameObject dropItem;        // 怪物掉落物品
        [Header("怪物的生成數量")] public int monsterCounts;      // 對應每個怪物的生成數量
        [Header("怪物的生成位置")] public Transform[] spawnPoints;  // 對應每個怪物的生成位置



        [HideInInspector] public int waveIndex;  // 用來記錄怪物屬於哪個波次



        private IEnumerator SpawnMonstersWithInterval(Transform[] spawnPoints)
        {
            float timer = 0f;
            //Debug.Log("調用生成SpawnMonstersWithInterval");

            // 在每個生成點生成怪物
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                for (int j = 0; j < monsterCounts; j++)
                {
                    GameObject newMonster = Instantiate(monsterPrefab, spawnPoints[i].position, spawnPoints[i].rotation);

                    // 可以在這裡設定怪物的其他屬性，例如生命值等
                    //newMonster.GetComponent<YourMonsterScript>().Initialize(monsterHealth, dropItem);

                    // 如果有需要，可以將生成的怪物加入列表或追蹤它們的其他邏輯

                    // 等待生成間隔時間

                    while (timer < spawnInterval)
                    {
                        timer += Time.deltaTime;
                        yield return null; // 讓這個迴圈在每一幀都等待
                    }

                    // 重置計時器
                    timer = 0f;
                }
            }
        }
    }



    [System.Serializable]
    public class MonsterWave
    {
        [HideInInspector] public string displayName;  // 用來表示波次編號的名稱

        public List<MonsterOption> monsterOptionsList = new List<MonsterOption>();  // 怪物配置

        public int expectedTotalMonsters;  // 預計生成的怪物總數
       
        
    }

    [Header("波次配置")] public List<MonsterWave> waves = new List<MonsterWave>();


    private int currentWaveIndex = 0;          // 紀錄當前波次的索引
    private int remainingMonsters;             // 紀錄當前波次的存活怪物數量
    public Text roundText;                     // 這是你的UI Text組件
    public Text NumberOfMonstersText;          // 這是你的UI Text組件
    public Text TextPoints;                    // 點數UI Text組件


    [Header("點數")] public float Points;      // 點數


    private bool isStartingNextWave = false;



    void Start()
    {
        // 遊戲開始時，初始化當前波次
        currentWaveIndex = 0;
        StartWave();


    }



    void Update()
    {
        // 檢查是否有存活的怪物
        if (remainingMonsters == 0 && !isStartingNextWave)
        {
            // 避免重複啟動協程
            isStartingNextWave = true;

            // 當前波次結束，可以開始下一波
            StartCoroutine(DelayedStartNextWave());
            //StartNextWave();
        }
    }



    void OnValidate()
    {
        UpdateDisplayNames();
    }



    private void UpdateDisplayNames() //編號命名
    {
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].displayName = "--波次--" + (i + 1).ToString();

            for (int j = 0; j < waves[i].monsterOptionsList.Count; j++)
            {
                waves[i].monsterOptionsList[j].displayName = "----怪物----" + (j + 1).ToString();
            }
        }
    }

    private void Awake()
    {
        // 設置 Instance 為這個實例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // 避免多個實例存在
        }
    }


    // 開始波次
    private void StartWave()
    {
        remainingMonsters = 0;
        waves[currentWaveIndex].expectedTotalMonsters = 0;

        foreach (var monsterOption in waves[currentWaveIndex].monsterOptionsList)
        {
            //remainingMonsters += monsterOption.monsterCounts;

            monsterOption.waveIndex = currentWaveIndex;

            /*
            waves[currentWaveIndex].expectedTotalMonsters += monsterOption.monsterCounts;
            monsterOption.waveIndex = currentWaveIndex;
            */

            // 調用生成怪物的方法
            StartCoroutine(monsterOption.SpawnMonsters(monsterOption.spawnPoints));


            // 計算預計生成的怪物總數，考慮每個怪物的生成數量和生成位置的組合
            waves[currentWaveIndex].expectedTotalMonsters += monsterOption.monsterCounts;

            // 計算剩餘怪物數量，考慮每個怪物的生成數量和生成位置的組合
            remainingMonsters += monsterOption.monsterCounts * monsterOption.spawnPoints.Length;
        }

        // 在這裡，你可以執行開始波次的相關邏輯
        Debug.Log("開始波次：" + waves[currentWaveIndex].displayName);

        // 更新UI Text顯示當前回合數

        UpdateRoundText();
    }



    // 開始下一波次的方法
    private void StartNextWave()
    {
        if (currentWaveIndex < waves.Count - 1)
        {
            // 開始下一波
            currentWaveIndex++;
            StartWave();
        }
        else
        {
            // 已經到達最後一波，可能執行遊戲結束的相關邏輯
            Debug.Log("遊戲結束！");
            roundText.text = "回合結束：" + (currentWaveIndex + 1).ToString();
            NumberOfMonstersText.text= "  存活怪物數量：" + remainingMonsters.ToString();
        }
    }
    // 更新UI Text的方法
    public void UpdateRoundText()
    {
        roundText.text = "回合：" + (currentWaveIndex + 1).ToString();
        NumberOfMonstersText.text = "  存活怪物數量：" + remainingMonsters.ToString();
        TextPoints.text = "點數" + Points.ToString();

    }


    // 延遲啟動下一波
    IEnumerator DelayedStartNextWave()
    {
        yield return new WaitForSeconds(3f);  // 這裡可以調整延遲的秒數
        StartNextWave();

        // 重置控制變數
        isStartingNextWave = false;
    }



    // 當怪物死亡時呼叫，減少存活怪物數量
    public void MonsterDied()
    {
        remainingMonsters--;

        Points++;

        // 如果存活怪物數量為零，波次結束
        if (remainingMonsters == 0 && AreAllMonstersDefeated())
        {
            StartCoroutine(DelayedStartNextWave());
        }
        UpdateRoundText();
    }



    // 新增檢查當前波次的所有怪物是否已解決的方法
    private bool AreAllMonstersDefeated()
    {
        return remainingMonsters == 0 && waves[currentWaveIndex].expectedTotalMonsters == 0;
    }



}