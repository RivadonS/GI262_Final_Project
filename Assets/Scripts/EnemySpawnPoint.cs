using UnityEngine;

public enum SpawnNodeType { Ground, Flying }

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Node Configuration")]
    public SpawnNodeType nodeType = SpawnNodeType.Ground; // เลือกใน Inspector ของแต่ละ Prefab ได้เลย
    public int maxSpawns = 3;       // โควต้าสูงสุดของจุดนี้
    public float respawnDelay = 2f; // ดีเลย์เกิดใหม่

    // ตัวแปรให้ MapGenerator เข้ามาจัดการสถานะ
    [HideInInspector] public int currentSpawnCount = 0;
    [HideInInspector] public float respawnTimer = 0f;
    [HideInInspector] public GameObject activeEnemy;

    private void Awake()
    {
        // เซ็ตให้พร้อมเสกทันทีเมื่อเริ่มเกม
        respawnTimer = respawnDelay;
    }

    private void OnDrawGizmos()
    {
        // แยกสี Gizmos ให้ดูง่ายขึ้นในฉาก (สีเขียว = พื้น, สีฟ้า = บิน)
        Gizmos.color = (nodeType == SpawnNodeType.Ground) 
            ? new Color(0f, 1f, 0f, 0.5f) 
            : new Color(0f, 1f, 1f, 0.5f);
            
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}