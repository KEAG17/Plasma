using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_RemoteControl : MonoBehaviour
{

    // 定義槍口位置的 Transform
    public Transform muzzle;
    public Transform Gameobject;

    // 參考角色翻轉腳本
    public PlayerControler playercontroler;
    static public int GunRotate;//槍枝旋轉判斷用於槍械彈殼位置

    // 儲存上一次的旋轉方向，用於檢測旋轉方向的改變
    private float previousAngle = 0f;

    // 控制槍械旋轉速度的變數
    public float rotationSpeed = 80f; //調整槍械旋轉速度


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 獲取滑鼠在畫面上的位置
        Vector3 mousePos = Input.mousePosition;
        // 將滑鼠位置轉換為世界座標
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.z - muzzle.position.z));
        // 計算槍口指向鼠標的方向
        Vector3 direction = mousePos - muzzle.position;
        // 使用 Atan2 函數計算角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        // 计算角度差异
        float angleDifference = angle - previousAngle;

        // 当角度差异大于2度时执行旋转
        if (Mathf.Abs(angleDifference) >= 0.7f)
        {
            // 判斷角度差異的正負
            //float rotationAmount = Mathf.Sign(angleDifference) * 2f;

            // 旋转槍口使其指向鼠标
            //muzzle.Rotate(0f, 0f, rotationAmount);
            muzzle.rotation = Quaternion.Euler(new Vector3(0, 0, angle));//即時
            // 更新上一次的旋转角度
            previousAngle = angle;
        }





        // 根據槍械旋轉速度平滑地旋轉槍械
        float step = rotationSpeed * Time.deltaTime;
        //muzzle.rotation = Quaternion.RotateTowards(muzzle.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), step);




        // 判斷玩家的 Scale X 是正數還是負數
        if (Mathf.Sign(playercontroler.PlayerTransform.localScale.x) > 0)
        {
            // 如果玩家的 Scale X 是正數，並且槍枝的旋轉角度在 90 度和 -90 度之間
            if (angle > 90f || angle < -90f)
            {
                GunRotate = 0;
                // 設置槍枝的 Scale Y 為負值
                Vector3 scale = muzzle.localScale;
                scale.y = -Mathf.Abs(scale.y);
                muzzle.localScale = scale;
            }
            else
            {
                GunRotate = 1;
                // 其他情況下，恢復槍枝的 Scale Y 為正值
                Vector3 scale = muzzle.localScale;
                scale.y = Mathf.Abs(scale.y);
                muzzle.localScale = scale;
            }
        }
        else
        {
            // 如果玩家的 Scale X 是負數，並且槍枝的旋轉角度在 90 度和 -90 度之間
            if (angle > 90f || angle < -90f)
            {
                // 設置槍枝的 Scale Y 為負值
                Vector3 scale = muzzle.localScale;
                scale.y = Mathf.Abs(scale.y);
                muzzle.localScale = scale;
            }
            else
            {
                // 其他情況下，恢復槍枝的 Scale Y 為正值
                Vector3 scale = muzzle.localScale;
                scale.y = -Mathf.Abs(scale.y);
                muzzle.localScale = scale;
            }
        }

        // 旋轉槍口使其指向鼠標

        //muzzle.rotation = Quaternion.Euler(new Vector3(0, 0, angle));//即時

        Gameobject.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // 檢查角色是否被翻轉，如果是，校正槍口方向
        /*
        if (playercontroler.IsFlipped())
        {
            // 將角度補偿 180 度
            angle += 180f;
            // 旋轉槍口使其指向鼠標
            muzzle.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        */
    }
}

