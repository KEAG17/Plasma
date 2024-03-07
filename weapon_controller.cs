using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class weapon_controller : MonoBehaviour
{
    //public WeaponStats[] weaponStatsArray; // 在 Unity 編輯器中指定每把武器的 WeaponStats

    public Laser_gun laserGun;
    public Plasma_Gun_mk1 Plasma;
    public Laser_gun laser_machine_gun;
    public MissileLauncher missileLauncher;
    public AutomaticGun automaticGun;
    public GLOCK_Pistol Glock_Pistol;
    public int OverheatSize;//槍枝發射過熱
    private Coroutine reloadCoroutine; // 儲存重新裝填的協程
    private bool canShoot = true; // 控制是否可以射擊的變量
    private bool isReloading = false; // 跟蹤是否正在重新裝填中

    [System.Serializable]
    public class WeaponInfo
    {
        public GameObject weaponObject; // 武器的遊戲物件
        public WeaponStats weaponStats; // 引用相应的WeaponStats



        [Header("武器等級")] public int currentWeaponLevel; // 根據武器等級管理數值
        [Header("武器的彈藥數量")] public int currentAmmo;      // 當前武器的彈藥數量
        [Header("最大彈藥值")] public int maxAmmo;          // 武器最大彈藥值
        [Header("彈藥類型")] public AmmoType ammoType;    // 武器的彈藥類型
        [Header("武器名稱")] public string weaponName;    // 武器名稱
        [Header("武器圖片")] public Sprite weaponImage;   // 武器圖片
        [Header("武器傷害")] public float weaponDamages;  // 武器傷害
        [Header("武器爆擊率")] public float critRate;       // 武器爆擊率
        [Header("武器發射冷卻時間")] public float shootCooldown;  // 發射冷卻時間
        [Header("彈匣裝填時間")] public float loadingWeaponTime;  // 彈匣裝填時間
        [HideInInspector] public float loadingTime;       // 彈匣裝填冷卻時間


        /// 過熱類 

        [Header("武器保險")] public bool WeaponInsurance = false;//武器保險
        [Header("武器過熱")] public bool OverheatSizeyes = false;//武器會過熱是否
        [Header("武器過熱上限")] public int OverheatSize = 0;        //武器過熱上限
        [Header("武器射擊次數")] public int NumberOfShots;           //槍枝射擊次數
        [HideInInspector] public float currentCooldown;       // 目前的冷卻時間


        //public int CurrentWeaponLevel;

        public AmmoType AmmoType
        {
            get { return ammoType; }
        }

    }


    public WeaponInfo[] weapons;        // 存儲所有武器的資訊陣列
    private int currentWeaponIndex = 5; // 當前武器的索引
    [Header("電能彈藥")] public int Battery_Ammunition; // 玩家身上所攜帶的Battery彈藥
    [Header("能源彈藥")] public int Energy_Ammunition;  // 玩家身上所攜帶的Energy彈藥
    [Header("爆裂彈藥")] public int Burst_Ammunition;  // 玩家身上所攜帶的Burst彈藥
    [Header("常規彈藥")] public int Bullet_Ammunition;  // 玩家身上所攜帶的Bullet彈藥
    [Header("手槍彈藥")] public int Unlimited_Bullets;  // 玩家身上所攜帶的Bullet彈藥


    // UI 元素
    public Text ammoText;       // 連接到 UI 子彈
    public Text ammoText02;       // 連接到 UI 子彈
    public Text weaponNameText; // 連接到武器名稱的 UI 文本元素
    public Image weaponImage;   // 連接到武器圖片的 UI 圖片元素
    public GameObject FGM148UI; // FGM148武器介面UI
    public GameObject FGM148Line; // FGM148武器Line
    public Text[] FGM148Text;     // FGM148武器介面UI
    public Image loadingWeaponImage;//裝填彈匣讀條

    // Start is called before the first frame update
    public int CurrentWeaponIndex
    {
        get { return currentWeaponIndex; }
    }
    public int CurrentWeaponIndexLevel { get; private set; }

    void Start()
    {
        // 初始化武器
        foreach (var weapon in weapons)
        {
            weapon.weaponObject.SetActive(false); // 初始時關閉所有武器
            weapon.currentCooldown = 0; // 初始化冷卻時間        
            weapon.loadingTime = 0;
            weapons[currentWeaponIndex].weaponObject.SetActive(true); // 啟用初始武器
            loadingWeaponImage.fillAmount = 0f;
            isReloading = false;
        }
        UpdateWeaponStats();
    }

    public void UpdateWeaponStats()//武器數值Level管理
    {
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length)
        {
            WeaponInfo currentWeapon = weapons[currentWeaponIndex];

            if (currentWeapon.weaponStats != null)
            {
                // 獲取當前等級的武器屬性
                WeaponStats.WeaponLevelProperties currentLevelProperties = currentWeapon.weaponStats.GetCurrentLevelProperties(currentWeapon.currentWeaponLevel);

                // 將屬性應用到 WeaponInfo 中
                currentWeapon.weaponDamages = currentLevelProperties.damageMultiplier;
                currentWeapon.critRate = currentLevelProperties.critRateMultiplier;
                currentWeapon.shootCooldown = currentLevelProperties.attackSpeedMultiplier;
                currentWeapon.maxAmmo = currentLevelProperties.maxAmmo;
                currentWeapon.loadingWeaponTime = currentLevelProperties.loadingWeaponTime;


                // 在這裡使用 currentLevelProperties 進行相應的操作

                Debug.Log("Current Damage: " + currentLevelProperties.damageMultiplier + "傷害");
                Debug.Log("Current Crit Rate: " + currentLevelProperties.critRateMultiplier + "爆擊");
                Debug.Log("Current Attack Speed: " + currentLevelProperties + currentLevelProperties.attackSpeedMultiplier + "射速");
                Debug.Log("Current Max Ammo: " + currentLevelProperties.maxAmmo + "最大彈藥");

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (weapons[currentWeaponIndex].OverheatSize != 0 && weapons[currentWeaponIndex].NumberOfShots >= weapons[currentWeaponIndex].OverheatSize)
        {

            StartCoroutine(Overheat());

        }
        if (Input.GetKeyDown("j"))
        {
            WeaponInfo currentWeapon = weapons[currentWeaponIndex];
            currentWeapon.currentWeaponLevel = 2;
        }

        // 更新武器的 UI

        UpdateAmmoUI();
        HandleWeaponInput();



        if (Input.GetKeyDown("r"))
        {
            // 如果之前有重新裝填的協程，則取消它
            /*
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            */

            // 獲取當前武器
            WeaponInfo currentWeapon = weapons[currentWeaponIndex];


            // 啟動重新裝填協程
            if (currentWeapon.currentAmmo < currentWeapon.maxAmmo && !isReloading)
            {
                // 根據 loadingWeaponTime 開始倒數計時
                currentWeapon.loadingTime = currentWeapon.loadingWeaponTime;


                reloadCoroutine = StartCoroutine(CountdownToReload());

                isReloading = true; // 標記為正在重新裝填中

            }

        }

        foreach (var weapon in weapons)
        {
            weapon.currentCooldown = Mathf.Max(0, weapon.currentCooldown - Time.deltaTime);
        }

        if (Input.GetKey("k") || Input.GetMouseButton(1) && weapons[currentWeaponIndex].currentAmmo > 0 && weapons[currentWeaponIndex].WeaponInsurance == false && canShoot == true)
        {
            StartCoroutine(ShootCurrentWeapon());
        }
    }
    IEnumerator ShootCurrentWeapon()//對應當前武器的射擊呼叫
    {
        WeaponInfo currentWeapon = weapons[currentWeaponIndex];

        if (currentWeapon.currentAmmo > 0 && currentWeapon.currentCooldown <= 0)
        {
            // 檢查冷卻時間
            // 進行射擊
            switch (currentWeaponIndex)
            {
                case 0:
                    StartCoroutine(laserGun.Shoot());
                    break;
                case 1:
                    Plasma.Shoot();
                    break;
                case 2:
                    StartCoroutine(laser_machine_gun.Shoot());
                    break;
                case 3:
                    missileLauncher.LaunchMissile();
                    break;
                case 4:
                    StartCoroutine(automaticGun.Shoot());
                    break;
                case 5:
                    StartCoroutine(Glock_Pistol.Shoot());
                    break;

                // 添加其他武器的處理...
                default:
                    // 根據你的需求進行默認處理，可能是顯示錯誤訊息或者不做任何事情
                    Debug.LogError("Unknown weapon index!");
                    break;
            }


            currentWeapon.NumberOfShots++;
            currentWeapon.currentAmmo--;
            // 設定冷卻時間
            currentWeapon.currentCooldown = currentWeapon.shootCooldown;

            // 等待冷卻時間結束
            yield return null;

        }
        else
        {
            // 彈藥不足的處理...
        }

        yield return null;
    }

    void HandleWeaponInput()
    {
        // 處理玩家輸入，例如按鍵觸發射擊或切換武器
        // 可以使用 Input.GetKey 或 Input.GetButtonDown 等方法

        // 例如，按1切換到第一把武器
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FGM148UI.SetActive(false);
            FGM148Line.SetActive(false);

            SwitchWeapon(5);//G17
        }

        // 例如，按2切換到第二把武器
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FGM148UI.SetActive(false);
            FGM148Line.SetActive(false);

            SwitchWeapon(0);//LAR
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FGM148UI.SetActive(false);
            FGM148Line.SetActive(false);

            SwitchWeapon(4);//
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FGM148UI.SetActive(false);
            FGM148Line.SetActive(false);

            SwitchWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            FGM148UI.SetActive(false);
            FGM148Line.SetActive(false);

            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            FGM148UI.SetActive(true);
            FGM148Line.SetActive(true);

            SwitchWeapon(3);
        }

        // 其他按鍵切換到相應的武器
    }

    void SwitchWeapon(int newIndex)
    {
        // 切換武器的方法
        if (newIndex >= 0 && newIndex < weapons.Length)
        {
            weapons[currentWeaponIndex].weaponObject.SetActive(false); // 關閉當前武器
            currentWeaponIndex = newIndex;
            weapons[currentWeaponIndex].weaponObject.SetActive(true); // 啟用新武器
            UpdateAmmoUI(); // 切換武器時更新 UI

            // 更新武器數值
            UpdateWeaponStats();

        }
    }

    void Change_Magazine()
    {
        //Debug.Log("Changing magazine");
        isReloading = false;

        // 當前武器的彈藥資訊
        WeaponInfo currentWeapon = weapons[currentWeaponIndex];

        // 如果當前武器的彈藥數量小於最大彈藥值，進行補充
        if (currentWeapon.currentAmmo < currentWeapon.maxAmmo)
        {
            //Debug.Log("Reloading...");

            // 計算需要補充的彈藥數量，限制補充量不超過武器最大彈藥值
            int neededAmmo = Mathf.Min(currentWeapon.maxAmmo - currentWeapon.currentAmmo, GetAmmoType());

            // 更新武器的彈藥數量
            currentWeapon.currentAmmo += neededAmmo;

            // 更新玩家身上所攜帶的彈藥
            SubtractAmmo(neededAmmo);

            isReloading = false;

        }
        else
        {
            //Debug.Log("Weapon already fully loaded");
        }
    }
    int GetAmmoType()//新更新彈藥需添加此處
    {
        /*// 根據當前武器索引，返回相應的彈藥數量
        return (currentWeaponIndex == 0) ? Battery_Ammunition : Energy_Ammunition;*/

        AmmoType currentAmmoType = weapons[currentWeaponIndex].AmmoType;

        switch (currentAmmoType)
        {
            case AmmoType.Battery:
                return Battery_Ammunition;
            case AmmoType.Energy:
                return Energy_Ammunition;
            case AmmoType.Burst:
                return Burst_Ammunition;
            case AmmoType.Bullet:
                return Bullet_Ammunition;
            case AmmoType.Unlimited:
                return Unlimited_Bullets;



            // 添加其他彈藥類型的處理...
            default:
                return 0; // 或者根據你的需求返回默認值
        }
    }

    void SubtractAmmo(int amount)//新更新彈藥需添加此處
    {

        // 根據當前武器的彈藥類型，減去相應的彈藥數量
        AmmoType currentAmmoType = weapons[currentWeaponIndex].AmmoType;

        // 當前武器的彈藥資訊
        WeaponInfo currentWeapon = weapons[currentWeaponIndex];


        switch (currentAmmoType)
        {
            case AmmoType.Battery:
                Battery_Ammunition -= amount;
                break;
            case AmmoType.Energy:
                Energy_Ammunition -= amount;
                break;
            case AmmoType.Burst:
                Burst_Ammunition -= amount;
                break;
            case AmmoType.Bullet:
                Bullet_Ammunition -= amount;
                break;
            case AmmoType.Unlimited:
                Unlimited_Bullets = currentWeapon.maxAmmo;
                break;



            // 添加其他彈藥類型的處理...
            default:
                // 根據你的需求進行默認處理，可能是顯示錯誤訊息或者不做任何事情
                Debug.LogError("Unknown ammo type!");
                break;
        }
    }

    public enum AmmoType//新更新彈藥需添加此處
    {

        Battery,
        Energy,
        Burst,
        Bullet,
        Unlimited,
        // 添加其他彈藥類型
    }


    void UpdateAmmoUI()//新更新彈藥需添加此處
    {
        if (ammoText != null && currentWeaponIndex < weapons.Length)
        {
            // 根據當前武器的彈藥類型更新 UI
            //string ammoTypeText = (weapons[currentWeaponIndex].ammoType == AmmoType.Battery) ? "電能" : "能源" ;

            string ammoTypeText;

            switch (weapons[currentWeaponIndex].ammoType)
            {
                case AmmoType.Battery:
                    ammoTypeText = "電能";
                    break;
                case AmmoType.Energy:
                    ammoTypeText = "能源";
                    break;
                case AmmoType.Burst:
                    ammoTypeText = "爆裂";
                    break;
                case AmmoType.Bullet:
                    ammoTypeText = "實彈";
                    break;
                case AmmoType.Unlimited:
                    ammoTypeText = "手槍彈";
                    break;


                default:
                    ammoTypeText = "未知"; // Handle any other cases if necessary
                    break;
            }



            ammoText.text = "類型" + ammoTypeText + ":" + GetAmmoType() + "\n" +
                            "彈藥: " + weapons[currentWeaponIndex].currentAmmo + " / " + weapons[currentWeaponIndex].maxAmmo;
            //"\n類型彈藥數量: " + GetAmmoType();
            ammoText02.text =weapons[currentWeaponIndex].currentAmmo.ToString();




            // 更新武器名稱
            weaponNameText.text = "武器名稱: " + weapons[currentWeaponIndex].weaponName;

            // 更新武器圖片
            weaponImage.sprite = weapons[currentWeaponIndex].weaponImage;

            if (CurrentWeaponIndex == 3)//FGM148UI
            {
                if (weapons[3].currentAmmo < weapons[3].maxAmmo)
                {
                    FGM148Text[0].text = "TOP:NO" + "\n" + "\n" + "DIR:OK" + "\n" + "\n" + "導彈未連線" + "\n" + weapons[currentWeaponIndex].currentAmmo + " / " + weapons[currentWeaponIndex].maxAmmo;

                }
                else
                {
                    FGM148Text[0].text = "TOP:NO" + "\n" + "\n" + "DIR:OK" + "\n" + "\n" + "導彈就緒" + "\n" + weapons[currentWeaponIndex].currentAmmo + " / " + weapons[currentWeaponIndex].maxAmmo;

                }
            }
        }
    }
    // 其他方法...

    public IEnumerator Overheat()
    {


        if (currentWeaponIndex == 2)//武器序列-2
        {
            if (!weapons[currentWeaponIndex].OverheatSizeyes)
            {

                weapons[currentWeaponIndex].OverheatSizeyes = true;
                float CD1 = weapons[2].shootCooldown;
                laser_machine_gun.GunOverheat();
                weapons[2].shootCooldown *= 2F;


                yield return new WaitForSeconds(6.5F);

                weapons[2].NumberOfShots = 0;
                weapons[2].shootCooldown = CD1;
                weapons[2].OverheatSizeyes = false;
                weapons[2].WeaponInsurance = false;
            }
        }

        if (currentWeaponIndex == 0)//武器序列-1
        {
            if (!weapons[currentWeaponIndex].OverheatSizeyes)
            {

                weapons[currentWeaponIndex].OverheatSizeyes = true;
                float CD2 = weapons[0].shootCooldown;
                weapons[currentWeaponIndex].shootCooldown *= 1.5F;


                yield return new WaitForSeconds(3.5F);

                weapons[0].shootCooldown = CD2;
                weapons[0].WeaponInsurance = false;
                weapons[0].OverheatSizeyes = false;
                weapons[0].NumberOfShots = 0;

            }
        }
    }
    public void UpWeapon()//呼叫武器數值更新
    {
        UpdateWeaponStats();
    }


    IEnumerator CountdownToReload()//裝填彈匣冷卻
    {
        // 獲取當前武器
        WeaponInfo currentWeapon = weapons[currentWeaponIndex];


        // 在開始裝填時將進度條填充比例設置為 0 UI
        loadingWeaponImage.fillAmount = 0f;

        // 紀錄開始裝填的時候的武器索引
        int startingWeaponIndex = currentWeaponIndex;

        // 暫停射擊
        canShoot = false;

        while (currentWeapon.loadingTime > 0)
        {
            // 如果在重新裝填的過程中切換了武器，則取消重新裝填
            if (startingWeaponIndex != currentWeaponIndex)
            {
                loadingWeaponImage.fillAmount = 0f;
                canShoot = true; // 重新啟用射擊
                isReloading = false; // 取消重新裝填標記
                yield break; // 結束協程
            }

            // 更新進度條的填充比例，表示裝填進度
            float progress = 1f - (currentWeapon.loadingTime / currentWeapon.loadingWeaponTime); // 計算裝填進度
            loadingWeaponImage.fillAmount = progress;

            // 倒數計時
            currentWeapon.loadingTime -= Time.deltaTime;
            yield return null;
        }
        loadingWeaponImage.fillAmount = 0f;
        canShoot = true; // 重新啟用射擊
        isReloading = false; // 取消重新裝填標記

        // 倒數計時結束，呼叫彈匣裝填方法
        Change_Magazine();
    }


}
