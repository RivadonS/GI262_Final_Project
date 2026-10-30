using System.Collections.Generic;
using UnityEngine;

// กำหนด Enum ให้เรียกใช้เป็น Key ได้ง่ายๆ และไม่พิมพ์ผิด
public enum StatType
{
    VIT, // Vitality (Health)
    STR, // Strength (Damage)
    DEF, // Defense (Damage Reduction)
    STA  // Stamina (Action Points)
}

[System.Serializable] // ทำให้คลาสนี้โชว์ใน Inspector ของ Unity ได้
public class CharacterStat
{
    public float baseValue; // ค่าพื้นฐานตอนเลเวลอัพ
    private List<float> modifiers = new List<float>(); // ลิสต์เก็บค่าบัฟจาก Passive Skill / ไอเทม

    public CharacterStat(float baseValue)
    {
        this.baseValue = baseValue;
    }

    // ฟังก์ชันคำนวณค่าผลลัพธ์สุดท้าย (รวมบัฟทั้งหมด)
    public float GetValue()
    {
        float finalValue = baseValue;
        foreach (float modifier in modifiers)
        {
            finalValue += modifier;
        }
        return finalValue;
    }

    // เอาไว้ให้ Passive Skill เรียกใช้เวลาจะเพิ่ม/ลด บัฟ
    public void AddModifier(float modifier) => modifiers.Add(modifier);
    public void RemoveModifier(float modifier) => modifiers.Remove(modifier);
}