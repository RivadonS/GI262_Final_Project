using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public GameObject[] chunkPrefabs;
    public int numberOfChunks = 5;
    public Transform startPosition;

    [Header("Enemy Pool Settings")]
    public GameObject[] groundEnemies; // ใส่ Prefab ศัตรูเดินดินทั้งหมดที่นี่
    public GameObject[] flyingEnemies; // ใส่ Prefab ศัตรูบินทั้งหมดที่นี่

    // เก็บรายการจุดเกิดทั้งหมดที่ถูกสร้างขึ้นในด่าน (Data Structure: List)
    private List<EnemySpawnPoint> allSpawnPoints = new List<EnemySpawnPoint>();

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        // จัดการระบบ Respawn ของทุกจุดเกิดจากศูนย์กลางที่เดียว
        HandleEnemyRespawns();
    }

    public void GenerateMap()
    {
        allSpawnPoints.Clear(); // เคลียร์ลิสต์เก่าเผื่อกรณีรีเซ็ตด่านใหม่
        Vector3 nextSpawnPosition = startPosition.position;

        for (int i = 0; i < numberOfChunks; i++)
        {
            int randomIndex = Random.Range(0, chunkPrefabs.Length);
            GameObject chunkToSpawn = chunkPrefabs[randomIndex];

            GameObject newChunk = Instantiate(chunkToSpawn, nextSpawnPosition, Quaternion.identity);

            // ค้นหาจุดเกิดทั้งหมดที่ซ่อนอยู่ใน Chunk นี้ แล้วเก็บเข้า List
            EnemySpawnPoint[] chunkNodes = newChunk.GetComponentsInChildren<EnemySpawnPoint>();
            allSpawnPoints.AddRange(chunkNodes);

            Transform endPoint = newChunk.transform.Find("EndPoint");
            if (endPoint != null)
            {
                nextSpawnPosition = endPoint.position;
            }
            else
            {
                Debug.LogError($"Chunk ชื่อ {newChunk.name} ไม่มี GameObject ชื่อ 'EndPoint' นะ! ด่านจะทับกัน");
                break;
            }
        }
    }

    private void HandleEnemyRespawns()
    {
        // วนลูปเช็คทุกจุดเกิดในด่าน
        foreach (EnemySpawnPoint node in allSpawnPoints)
        {
            if (node == null || node.currentSpawnCount >= node.maxSpawns) continue;

            // ถ้าศัตรูของจุดนี้ตายไปแล้ว (หรือยังไม่เคยเกิด)
            if (node.activeEnemy == null)
            {
                node.respawnTimer += Time.deltaTime;

                if (node.respawnTimer >= node.respawnDelay)
                {
                    SpawnEnemiesAtNode(node);
                    node.respawnTimer = 0f;
                }
            }
        }
    }

    // ฟังก์ชันเสกศัตรูตามประเภทของ Node (ตรงตามเมธอด SpawnEnemies() ใน Diagram)
    public void SpawnEnemiesAtNode(EnemySpawnPoint node)
    {
        GameObject[] targetPool = (node.nodeType == SpawnNodeType.Ground) ? groundEnemies : flyingEnemies;

        if (targetPool.Length == 0) return;

        int randomIndex = Random.Range(0, targetPool.Length);
        GameObject selectedEnemy = targetPool[randomIndex];

        // เสกศัตรูออกมา
        GameObject spawned = Instantiate(selectedEnemy, node.transform.position, Quaternion.identity);
        
        // อัปเดตสถานะกลับไปที่ Node
        node.activeEnemy = spawned;
        node.currentSpawnCount++;

        // 💡 จุดต่อยอดในอนาคต: ถ้าจะทำ Level Scaling สามารถดึงสคริปต์ Enemy มาอัปเลเวลตรงนี้ได้เลย!
        // Enemy enemyScript = spawned.GetComponent<Enemy>();
    }
}