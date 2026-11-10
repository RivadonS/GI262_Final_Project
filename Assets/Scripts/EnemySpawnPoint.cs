using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] possibleEnemies;
    
    [Header("Respawn Rules")]
    public int maxSpawns = 3;       // โควต้าสูงสุดที่จะเสกได้ในจุดนี้ (เช่น เสกได้แค่ 3 ครั้ง)
    public float respawnDelay = 2f; // หน่วงเวลากี่วินาทีหลังจากตัวเก่าตาย ถึงจะเสกตัวใหม่

    private int spawnCount = 0;     // นับว่าเสกไปแล้วกี่ตัว
    private float timer = 0f;
    private GameObject currentActiveEnemy; // เก็บตัวที่กำลังมีชีวิตอยู่

    private void Start()
    {
        // เซ็ตให้ตัวแรกเกิดทันทีเมื่อเริ่มด่าน (ไม่ต้องรอ Delay)
        timer = respawnDelay; 
    }

    private void Update()
    {
        // 1. ถ้าเสกครบโควต้าแล้ว ก็ปิดการทำงานของ Node นี้ไปเลย
        if (spawnCount >= maxSpawns) return;

        // 2. ถ้าศัตรูตัวปัจจุบันตายไปแล้ว (โดน Destroy จนกลายเป็น null) หรือยังไม่ได้เสก
        if (currentActiveEnemy == null)
        {
            timer += Time.deltaTime; // เริ่มนับเวลาถอยหลัง

            // 3. เมื่อครบเวลาดีเลย์ ให้เสกตัวใหม่
            if (timer >= respawnDelay)
            {
                SpawnEnemy();
                timer = 0f; // รีเซ็ตเวลาเพื่อรอรอบถัดไป
            }
        }
    }

    private void SpawnEnemy()
    {
        if (possibleEnemies.Length == 0) return;

        int randomIndex = Random.Range(0, possibleEnemies.Length);
        GameObject selectedEnemy = possibleEnemies[randomIndex];

        // เสกพร้อมกับเก็บ Reference ไว้เช็คสถานะการตาย
        currentActiveEnemy = Instantiate(selectedEnemy, transform.position, Quaternion.identity);
        
        spawnCount++; // หักโควต้า
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}