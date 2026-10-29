using UnityEngine;
using UnityEngine.SceneManagement; // ต้องใช้ตัวนี้สำหรับการโหลดฉาก (Restart)
using UnityEngine.InputSystem;     // สำหรับรับปุ่ม ESC

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI; // ลาก PauseMenuUI (Panel) มาใส่ช่องนี้

    [Header("Input")]
    public InputAction pauseAction;

    // ตัวแปรเช็คสถานะ (ใช้ static เผื่อสคริปต์อื่นอยากรู้ว่าเกมหยุดอยู่ไหม)
    public static bool GameIsPaused = false; 

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    private void Update()
    {
        // ถัากดปุ่ม ESC เฟรมนี้
        if (pauseAction.WasPressedThisFrame())
        {
            if (GameIsPaused)
            {
                Resume(); // ถ้าหยุดอยู่ ให้เล่นต่อ
            }
            else
            {
                Pause(); // ถ้าเล่นอยู่ ให้หยุด
            }
        }
    }

    // ฟังก์ชันนี้ต้องเป็น public เพื่อให้ปุ่มใน UI มองเห็นและเรียกใช้ได้
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // ปิดหน้าต่าง UI
        Time.timeScale = 1f;          // คืนเวลาให้เดินปกติ (1 คือความเร็ว 100%)
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);  // เปิดหน้าต่าง UI
        Time.timeScale = 0f;          // หยุดเวลา (ฟิสิกส์และแอนิเมชันจะหยุดกึก)
        GameIsPaused = true;
    }

    public void Restart()
    {
        // สำคัญมาก! ต้องคืนเวลากลับเป็น 1 ก่อนโหลดฉาก ไม่งั้นฉากใหม่เวลาจะยังหยุดอยู่
        Time.timeScale = 1f;
        GameIsPaused = false;
        
        // โหลดฉากปัจจุบันใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void Options()
    {
        // เดี๋ยวเรามาทำ UI หน้า Option ซ้อนอีกที ตอนนี้ให้มันปริ้นข้อความไปก่อน
        Debug.Log("เปิดหน้าต่าง Options...");
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกม!");
        Application.Quit(); // คำสั่งนี้จะทำงานตอน Build เป็นไฟล์ .exe หรือ .apk แล้ว

        // ทริคลับของ Senior: โค้ดชุดนี้จะทำงานแค่ใน Unity Editor เท่านั้น ช่วยให้เราเทสต์ปุ่ม Quit ได้
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}