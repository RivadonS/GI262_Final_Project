using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public GameObject[] chunkPrefabs;
    public int numberOfChunks = 5;
    
    [Header("Spawn Position")]
    public Transform startPosition;

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        Vector3 nextSpawnPosition = startPosition.position;

        for (int i = 0; i < numberOfChunks; i++)
        {
            // 1. สุ่มเลือก Chunk จากคลัง
            int randomIndex = Random.Range(0, chunkPrefabs.Length);
            GameObject chunkToSpawn = chunkPrefabs[randomIndex];

            // 2. เสกด่านออกมาที่ตำแหน่ง nextSpawnPosition
            GameObject newChunk = Instantiate(chunkToSpawn, nextSpawnPosition, Quaternion.identity);

            // 3. ค้นหาจุดจบของด่านนี้
            Transform endPoint = newChunk.transform.Find("EndPoint");

            if (endPoint != null)
            {
                // อัปเดตตำแหน่งจุดเกิดด่านถัดไป ให้อยู่ตรงขอบด่านนี้พอดี
                nextSpawnPosition = endPoint.position;
            }
            else
            {
                // แจ้งเตือนถ้าเพื่อนในทีมลืมใส่ EndPoint
                Debug.LogError($"Chunk ชื่อ {newChunk.name} ไม่มี GameObject ชื่อ 'EndPoint' นะ! ด่านจะทับกัน");
                break;
            }
        }
    }
}