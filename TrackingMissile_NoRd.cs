using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingMissile_NoRd : MonoBehaviour
{
    public Transform target; // 導彈的目標
    public float initialMissileSpeed = 5f;  // 導彈速度
    public float rotationSpeed = 1f;        // 轉向速度
    public float decelerationRate = 0.8f;   // 減速比例
    //public float forwardForce = 10f;
    public float angleThreshold = 45f;      // 角度閾值

    public List<ParticleSystem> RocketEffects = new List<ParticleSystem>();//特效
    public List<GameObject> LightEffects = new List<GameObject>();//特效


    [Header("導彈跟蹤啟動時間")]
    public float currentLaunchDelay;  // 當前延遲時間
    private bool hasStartedTracking = false;//轉向啟動bool

    public GameObject Explode;//爆炸

    void Start()
    {
        Destroy(gameObject, 10f);

    }
    void FixedUpdate()
    {
        if (target != null)
        {
            // 計算朝向目標的方向
            Vector3 direction = target.position - transform.position;
            direction.Normalize();


            if(hasStartedTracking)
            {
                // 使用 Quaternion.Slerp 實現緩慢轉向
                Quaternion desiredRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // 如果還沒開始追蹤，開始倒數
                currentLaunchDelay -= Time.deltaTime;

                // 如果倒數時間到，開始追蹤
                if (currentLaunchDelay <= 0f)
                {
                    hasStartedTracking = true;
                }
            }
            // 計算前進方向和目標方向的角度
            float angleDifference = Vector3.Angle(transform.up, direction);

            /*
            // 根據角度差距判斷是否要減速
            if (angleDifference > angleThreshold)
            {
                // 根據減速比例減小速度
                initialMissileSpeed *= decelerationRate;
            }
            */
            // 直接移動導彈
            transform.Translate(Vector3.up * initialMissileSpeed * Time.fixedDeltaTime);

            // 追擊目標，可以根據需要調整追擊邏輯
            ChaseTarget();
        }
        else
        {
            transform.Translate(Vector3.up * initialMissileSpeed * Time.fixedDeltaTime);

        }
    }

    void ChaseTarget()
    {
        // 可以根據需要實現追擊邏輯，例如改變朝向、速度等
        // 這裡使用簡單的方法，直接將目標的位置設為導彈的目標位置
        //target = GameObject.FindWithTag("MonsterColliders").transform;
    }

    private bool isCollided = false;
    private void OnTriggerEnter2D(Collider2D collision)//觸碰到別的碰撞器的時候
    {


        if (!isCollided)
        {
            isCollided = true;
            Instantiate(Explode, this.transform.position, this.transform.rotation);

            // 停止導彈的碰撞檢測
            GetComponent<Collider2D>().enabled = false;

            DisableLaser();

            initialMissileSpeed = 0f;

            rotationSpeed = 0f;


            // 遍歷導彈的子物體找到名稱為 "missile" 的 SpriteRenderer 並關閉
            foreach (Transform child in transform)
            {
                if (child.name == "missile")
                {
                    SpriteRenderer missileSprite = child.GetComponent<SpriteRenderer>();//關閉導彈圖片
                    if (missileSprite != null)
                    {
                        missileSprite.enabled = false;
                    }
                }
            }

            Destroy(gameObject,3f);
        }

    }
    void DisableLaser()//特效關閉
    {

        for (int i = 0; i < RocketEffects.Count; i++)
            RocketEffects[i].Stop();

        for (int i = 0; i < LightEffects.Count; i++)
            LightEffects[i].gameObject.SetActive(false);


    }

    //
}
