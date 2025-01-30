using UnityEngine;
using System.Collections; 
 
public class Player : MonoBehaviour
{
    public Animator PlayerAnimator;
    public SpriteRenderer spriteRenderer; // 캐릭터의 SpriteRenderer

    [Header("Settings")]
    public float JumpForce;
    public int maxJumpCount = 3;

    [Header("References")]
    public Rigidbody2D PlayerRigidBody; 
    public AudioSource jumpAudioSource; // 점프 효과음용 AudioSource
    public AudioClip jumpSound; // 점프 효과음
    public AudioSource hitAudioSource; // 충돌 효과음용 AudioSource
    public AudioClip hitSound; // 충돌 효과음
    public AudioClip goldenSound; // 골든 배추 효과음

    private Color originalColor; // 원래 색상 저장
    private bool isFlickering = false; // 깜빡임 상태 확인
    private bool isGrounded = true;
    private int currentJumpCount = 0;
    private bool isInvincible = false;

    public BoxCollider2D PlayerCollider;

    IEnumerator Start() 
    {
        // ✅ GameManager가 초기화될 때까지 대기
        while (GameManager.Instance == null)
        {
            yield return null;
        }
 
        originalColor = spriteRenderer.color; // 원래 색상 저장

        // ✅ Rigidbody 초기화
        PlayerRigidBody.linearVelocity = Vector2.zero;
        PlayerRigidBody.angularVelocity = 0f;
        
        isGrounded = true;
        currentJumpCount = 0;
        ResetAnimator(); // ✅ 애니메이터 강제 초기화
    }

    // ✅ 애니메이터 강제 초기화 함수 추가
    void ResetAnimator()
    {
        PlayerAnimator.Rebind();
        PlayerAnimator.Update(0);
    }

    void Update()
    {
        HandleJump();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Platform")
        {
            HandleLanding();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "enemy" && !isInvincible)
        {
            Destroy(collider.gameObject);
            Hit();
        }
        else if (collider.gameObject.tag == "food")
        {
            Destroy(collider.gameObject);
            Heal();
        }
        else if (collider.gameObject.tag == "golden")
        {
            Destroy(collider.gameObject);
            StartInvincible();
        }
    }

    // 🟢 무적 상태 시작
    void StartInvincible()
    {
        ResetInvincibleTimer(); // 무적 타이머 초기화
        BGMPlayer.Instance.StartInvincibleBGM(); // 무적 상태 배경음악 실행

        if (!isFlickering) // 깜빡임 상태가 아니면 시작
        {
            StartCoroutine(FlickerSprite());
        }

        isInvincible = true;
    }

    // 🔴 무적 상태 종료
    void StopInvincible()
    {
        isInvincible = false;
        isFlickering = false;

        StopCoroutine(FlickerSprite()); // 깜빡임 중단
        ResetSprite(); // 스프라이트 초기화
        BGMPlayer.Instance.StopInvincibleBGM(); // 무적 배경음악 종료
    }

    // 🎵 점프 처리
    void HandleJump()
    { 
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || currentJumpCount < maxJumpCount))
        {
            Debug.Log("JumpForce: " + JumpForce); // ✅ JumpForce 값이 정상적인지 확인
            PlayerRigidBody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            isGrounded = false;
            currentJumpCount++;

            PlayerAnimator.SetInteger("state", 1); // 점프 애니메이션

            PlaySound(jumpAudioSource, jumpSound);
        }
    }

    // 🟢 착지 처리
    void HandleLanding()
    {
        if (!isGrounded)
        {
            PlayerAnimator.SetInteger("state", 2); // 착지 애니메이션
        }

        isGrounded = true;
        currentJumpCount = 0;
    }

    // 🛑 충돌 처리
    void Hit()
    {
        GameManager.Instance.Lives -= 1;
        PlaySound(hitAudioSource, hitSound); // 충돌 효과음
    }

    // ❤️ 회복 처리
    void Heal()
    {
        GameManager.Instance.Lives = Mathf.Min(3, GameManager.Instance.Lives + 1);
    }

    // 🟡 무적 상태 타이머 초기화
    void ResetInvincibleTimer()
    {
        CancelInvoke("StopInvincible");
        Invoke("StopInvincible", 5f); // 무적 상태 5초 후 종료
    }

    // 🎨 깜빡임 효과
    private System.Collections.IEnumerator FlickerSprite()
    {
        isFlickering = true;
        while (isInvincible)
        {
            spriteRenderer.color = Color.yellow; // 무적 상태: 노란색
            yield return new WaitForSeconds(0.1f); // 0.1초 대기

            spriteRenderer.color = originalColor; // 원래 색상 복구
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        }
        ResetSprite(); // 무적 종료 시 복구
    }
 
    // 🎨 스프라이트 복구
    void ResetSprite()
    {
        spriteRenderer.color = originalColor; // 원래 색상 복구
    }


    // 🎵 효과음 재생
    void PlaySound(AudioSource audioSource, AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip이 로드되지 않았습니다!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource가 없습니다!");
            return;
        }

        audioSource.PlayOneShot(clip);
    }
    public void KillPlayer()
    {
        // 플레이어가 죽었을 때 실행되는 동작
        PlayerCollider.enabled = false; // 충돌 감지 비활성화
        PlayerAnimator.enabled = false; // 애니메이션 정지
        PlayerRigidBody.linearVelocity = Vector2.zero; // 움직임 정지
        PlayerRigidBody.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse); // 위로 튕겨나는 동작 추가
    }

}
