using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponStats", menuName = "Custom/WeaponStats", order = 1)]
public class WeaponStats : ScriptableObject
{
    public WeaponLevelProperties GetWeaponLevelProperties(int level)
    {
        // 使用 LINQ 查找指定等级的屬性
        WeaponLevelProperties properties = weaponLevels.Find(prop => prop.level == level);

        if (properties == null)
        {
            Debug.LogError($"Weapon level properties for level {level} not found!");
        }

        return properties;
    }
    public WeaponLevelProperties GetCurrentLevelProperties(int currentLevel)
    {
        // 直接調用 GetWeaponLevelProperties 方法取得指定等級的屬性
        return GetWeaponLevelProperties(currentLevel);
    }

    [Header("武器等級屬性")]
    public List<WeaponLevelProperties> weaponLevels;

    [System.Serializable]
    public class WeaponLevelProperties
    {
        [Header("等級")]
        public int level;
        [Header("傷害")]
        public float damageMultiplier = 1f;
        [Header("爆擊率")]
        public float critRateMultiplier = 1f;
        [Header("射速")]
        public float attackSpeedMultiplier = 1f;
        [Header("最大彈藥值")]
        public int maxAmmo;
        [Header("武器裝填時間")]
        public float loadingWeaponTime;


    }
    // 定義一個方法，用於設定每把武器的基本屬性
    /*
    public void SetWeaponProperties(float damage, float critRate, float attackSpeed)
    {
        baseDamage = damage;
        baseCritRate = critRate;
        baseAttackSpeed = attackSpeed;
    }
    // 其他屬性...

    public float GetDamage(int weaponLevel)
    {
        // 根據武器等級計算實際傷害值
        return baseDamage + (weaponLevel * 5f); // 這裡是一個簡單的示例，實際上你可能需要更複雜的計算邏輯
    }

    public float GetCritRate(int weaponLevel)
    {
        // 根據武器等級計算實際爆擊率
        return baseCritRate + (weaponLevel * 0.01f); // 這裡是一個簡單的示例，實際上你可能需要更複雜的計算邏輯
    }
    */
    /*public float GetAttackSpeed(int weaponLevel)
    {
        // 根據武器等級計算實際攻擊速度
        return baseAttackSpeed + (weaponLevel * 0.1f); // 這裡是一個簡單的示例，實際上你可能需要更複雜的計算邏輯
    }
    // Start is called before the first frame update
    */

}
