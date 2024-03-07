using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    public float detectionRadius = 20f; // 搜索半徑
    private MissileLauncher missileLauncher;
    public BoxCollider2D detectionCollider; // 搜索範圍的碰撞器
    private List<Transform> enemiesInRange = new List<Transform>(); // 保存當前範圍內的所有怪物

    public bool useColliderAsRange = true; // 使用半徑搜索還是 BoxCollider2D 搜索
    public bool useTopAttack = false;//使用對空攻擊模式
    public bool UseLockUi = false;

    public LineRenderer horizontalLineRenderer; // 水平 Line Renderer
    public LineRenderer verticalLineRenderer; // 垂直 Line Renderer




    void Start()
    {
        missileLauncher= GetComponent<MissileLauncher>();

        /*
        if (useRadiusSearch)
        {
            // 如果使用半徑搜索，添加 BoxCollider2D 組件
            detectionCollider = gameObject.AddComponent<BoxCollider2D>();
            // 設定碰撞器的大小
            detectionCollider.size = new Vector2(detectionRadius * 2, detectionRadius * 2);
            // 確保碰撞器是觸發型的
            detectionCollider.isTrigger = true;
        }
        */

        // 初始化 Line Renderer
        InitializeLineRenderer(horizontalLineRenderer);
        InitializeLineRenderer(verticalLineRenderer);
        horizontalLineRenderer.enabled = false;
        verticalLineRenderer.enabled = false;
    }


    void Update()
    {
        // 在 Update 中檢測敵人

        if(useTopAttack)//對空攻擊模式
        {

            SearchForEnemyBOSS();//BOSS半徑搜索

        }
        else
        {
            SearchForEnemy();
            DrawDetectionRadius();
        }

        if (UseLockUi)
        {
            // 更新 Line Renderer 的位置
            UpdateLineRendererPositions();

        }
        
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.useWorldSpace = false; // 使用本地坐标
    }



    void SearchForEnemy()//半徑搜索
    {


        // 找到所有標籤為 "MonsterColliders" 的物體
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("MonsterColliders");


        // 如果目標存在，檢查是否離開搜尋範圍
        if (missileLauncher.target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, missileLauncher.target.position);

            // 如果使用 detectionCollider 作為範圍
            if (useColliderAsRange)
            {
                if (distanceToTarget > detectionCollider.size.x / 2f)
                {
                    // 如果目標離開搜尋範圍，設為 null
                    missileLauncher.target = null;
                }
            }
            // 如果使用 detectionRadius 作為範圍
            else
            {
                if (distanceToTarget > detectionRadius)
                {
                    // 如果目標離開搜尋範圍，設為 null
                    missileLauncher.target = null;
                }
            }
        }

        // 遍歷所有敵人物體
        foreach (GameObject enemy in enemies )
        {
            // 計算與敵人的距離
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // 如果使用 detectionCollider 作為範圍
            if (useColliderAsRange)
            {
                if (distanceToEnemy <= detectionCollider.size.x / 2f)
                {
                    // 在這裡可以對找到的敵人進行相應的處理，比如設定為導彈的目標
                    Transform missileTarget = enemy.transform;

                    // 假設你有一個 TrackingMissile_NoRd 腳本，你可以根據實際情況進行修改
                    TrackingMissile_NoRd missileScript = GetComponent<TrackingMissile_NoRd>();

                    // 判斷目標是否已經設定
                    if (missileLauncher.target == null)
                    {
                        Debug.Log("目標 missileTarget");

                        // 將目標設為找到的敵人
                        missileLauncher.target = missileTarget;
                    }

                    // 停止搜索，因為已經找到敵人
                    break;
                }
            }
            // 如果使用 detectionRadius 作為範圍
            else
            {
                if (distanceToEnemy <= detectionRadius)
                {
                    // 在這裡可以對找到的敵人進行相應的處理，比如設定為導彈的目標
                    Transform missileTarget = enemy.transform;

                    // 假設你有一個 TrackingMissile_NoRd 腳本，你可以根據實際情況進行修改
                    TrackingMissile_NoRd missileScript = GetComponent<TrackingMissile_NoRd>();

                    // 判斷目標是否已經設定
                    if (missileLauncher.target == null)
                    {
                        Debug.Log("目標 missileTarget");

                        // 將目標設為找到的敵人
                        missileLauncher.target = missileTarget;
                    }

                    // 停止搜索，因為已經找到敵人
                    break;
                }
            }
        }
    }



    void SearchForEnemyBOSS()//BOSS半徑搜索
    {


        // 找到所有標籤為 "MonsterColliders" 的物體
        GameObject[] BOSSenemies = GameObject.FindGameObjectsWithTag("BOSS");


        // 如果目標存在，檢查是否離開搜尋範圍
        if (missileLauncher.target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, missileLauncher.target.position);

            // 如果使用 detectionCollider 作為範圍
            if (useColliderAsRange)
            {
                if (distanceToTarget > detectionCollider.size.x / 2f)
                {
                    // 如果目標離開搜尋範圍，設為 null
                    missileLauncher.target = null;
                }
            }
            // 如果使用 detectionRadius 作為範圍
            else
            {
                if (distanceToTarget > detectionRadius)
                {
                    // 如果目標離開搜尋範圍，設為 null
                    missileLauncher.target = null;
                }
            }
        }

        // 遍歷所有敵人物體
        foreach (GameObject enemy in BOSSenemies)
        {
            // 計算與敵人的距離
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // 如果使用 detectionCollider 作為範圍
            if (useColliderAsRange)
            {
                if (distanceToEnemy <= detectionCollider.size.x / 2f)
                {
                    // 在這裡可以對找到的敵人進行相應的處理，比如設定為導彈的目標
                    Transform missileTarget = enemy.transform;

                    // 假設你有一個 TrackingMissile_NoRd 腳本，你可以根據實際情況進行修改
                    TrackingMissile_NoRd missileScript = GetComponent<TrackingMissile_NoRd>();

                    // 判斷目標是否已經設定
                    if (missileLauncher.target == null)
                    {
                        Debug.Log("目標 missileTarget");

                        // 將目標設為找到的敵人
                        missileLauncher.target = missileTarget;
                    }

                    // 停止搜索，因為已經找到敵人
                    break;
                }
            }
            // 如果使用 detectionRadius 作為範圍
            else
            {
                if (distanceToEnemy <= detectionRadius)
                {
                    // 在這裡可以對找到的敵人進行相應的處理，比如設定為導彈的目標
                    Transform missileTarget = enemy.transform;

                    // 假設你有一個 TrackingMissile_NoRd 腳本，你可以根據實際情況進行修改
                    TrackingMissile_NoRd missileScript = GetComponent<TrackingMissile_NoRd>();

                    // 判斷目標是否已經設定
                    if (missileLauncher.target == null)
                    {
                        Debug.Log("目標 missileTarget");

                        // 將目標設為找到的敵人
                        missileLauncher.target = missileTarget;
                    }

                    // 停止搜索，因為已經找到敵人
                    break;
                }
            }
        }
    }




// 在 Scene 中绘制搜索范围


    void UpdateLineRendererPositions()
    {
        if (missileLauncher.target != null)
        {
            float offset = 4f; // 你希望的高度偏移量
            float lineLength = 600f; // 你希望的线的长度

            // 更新水平 Line Renderer 的位置
            horizontalLineRenderer.SetPosition(0, new Vector3(missileLauncher.target.position.x - lineLength / 2f, missileLauncher.target.position.y + offset, 0));
            horizontalLineRenderer.SetPosition(1, new Vector3(missileLauncher.target.position.x + lineLength / 2f, missileLauncher.target.position.y + offset, 0));

            // 更新垂直 Line Renderer 的位置
            verticalLineRenderer.SetPosition(0, new Vector3(missileLauncher.target.position.x, missileLauncher.target.position.y - lineLength / 2f + offset, 0));
            verticalLineRenderer.SetPosition(1, new Vector3(missileLauncher.target.position.x, missileLauncher.target.position.y + lineLength / 2f + offset, 0));

            // 啟用線條
            horizontalLineRenderer.enabled = true;
            verticalLineRenderer.enabled = true;
        }
        else
        {
            // 禁用線條
            horizontalLineRenderer.enabled = false;
            verticalLineRenderer.enabled = false;
        }
    }




    void DrawDetectionRadius()
    {
        // 計算圓形的點
        int segments = 36;
        float angleIncrement = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleIncrement;
            Vector3 startPoint = transform.position + new Vector3(Mathf.Cos(angle) * detectionRadius, Mathf.Sin(angle) * detectionRadius, 0f);
            angle = Mathf.Deg2Rad * (i + 1) * angleIncrement;
            Vector3 endPoint = transform.position + new Vector3(Mathf.Cos(angle) * detectionRadius, Mathf.Sin(angle) * detectionRadius, 0f);

            // 使用Debug.DrawLine绘制射线
            Debug.DrawLine(startPoint, endPoint, Color.red);
        }
    }
}
