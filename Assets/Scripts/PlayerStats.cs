using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats Settings")]
    public float startingVIT = 10f;
    public float startingSTR = 5f;
    public float startingDEF = 3f;
    public float startingSTA = 50f;

    // Dictionary เก็บค่า Stats ทั้ง 4 อ้างอิงด้วย Enum ทำให้ O(1) ในการค้นหา (เร็วสุดๆ)
    public Dictionary<StatType, CharacterStat> stats;

    [Header("Current Status")]
    public float currentHealth;
    public float currentStamina;

    [Header("Stamina Regen Settings")]
    public float staminaRegenRate = 5f; // รีเจนกี่หน่วยต่อวินาที
    public float staminaRegenDelay = 1.5f; // ต้องหยุดใช้กี่วินาทีถึงจะเริ่มรีเจน
    private float lastStaminaUseTime;

    private void Awake()
    {
        // สร้าง Dictionary และยัดค่าเริ่มต้นเข้าไป
        stats = new Dictionary<StatType, CharacterStat>()
        {
            { StatType.VIT, new CharacterStat(startingVIT) },
            { StatType.STR, new CharacterStat(startingSTR) },
            { StatType.DEF, new CharacterStat(startingDEF) },
            { StatType.STA, new CharacterStat(startingSTA) }
        };

        // ตั้งค่าเริ่มต้นเลือดและสตามิน่า
        // VIT 1 หน่วย = เลือด 10
        currentHealth = GetMaxHealth();
        currentStamina = GetMaxStamina();
    }

    private void Update()
    {
        HandleStaminaRegen();
    }

    // ฟังก์ชันแปลง Stat เป็นตัวเลขใช้งานจริง
    public float GetMaxHealth() => stats[StatType.VIT].GetValue() * 10f;
    public float GetMaxStamina() => stats[StatType.STA].GetValue();
    public float GetAttackDamage() => stats[StatType.STR].GetValue();
    public float GetDefense() => stats[StatType.DEF].GetValue();

    // ระบบใช้ Stamina
    public bool TryUseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            lastStaminaUseTime = Time.time; // จดเวลาล่าสุดที่ใช้
            return true; // มีพอและถูกตัดไปแล้ว
        }
        return false; // ไม่พอ
    }

    // ระบบ Regen แบบค่อยๆ เด้ง
    private void HandleStaminaRegen()
    {
        if (currentStamina < GetMaxStamina() && Time.time >= lastStaminaUseTime + staminaRegenDelay)
        {
            currentStamina += staminaRegenRate * Time.deltaTime; // ค่อยๆ เพิ่มตามเฟรมเรต
            if (currentStamina > GetMaxStamina()) currentStamina = GetMaxStamina(); // ป้องกันล้น
        }
    }
}