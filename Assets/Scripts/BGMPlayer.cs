using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance;

    [Header("Settings")]
    public AudioSource defaultBGMSource; // 기본 배경음악
    public AudioSource invincibleBGMSource; // 무적 상태 배경음악

    private float originalDefaultBGMVolume; // 기본 BGM 원래 볼륨
    private bool isInvincibleActive = false; // 무적 상태 활성화 여부

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (defaultBGMSource != null)
        {
            originalDefaultBGMVolume = defaultBGMSource.volume; // 기본 BGM 볼륨 저장
        }
    }

    public void StartInvincibleBGM()
    {
        // 기본 BGM 볼륨을 0으로 줄이고 계속 재생
        defaultBGMSource.volume = 0f;

        // 무적 BGM이 재생 중이 아니면 재생
        if (!invincibleBGMSource.isPlaying)
        {
            invincibleBGMSource.Play();
        }

        isInvincibleActive = true;
    }

    public void StopInvincibleBGM()
    {
        if (!isInvincibleActive) return; // 무적 상태가 아니면 실행 안 함

        // 무적 BGM 정지
        invincibleBGMSource.Stop();

        // 기본 BGM 볼륨 원래대로 복구
        defaultBGMSource.volume = originalDefaultBGMVolume;

        isInvincibleActive = false;
    }
}
