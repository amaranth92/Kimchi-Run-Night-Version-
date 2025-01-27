using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator PlayerAnimator;

    [Header("Settings")]
    public float JumpForce; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("References")]
    public Rigidbody2D PlayerRigidBody; 
    private bool isGrounded = true; 

    private bool isInvincible = false;
    public BoxCollider2D PlayerCollider; 
    void Hit(){
        GameManager.Instance.Lives -= 1;
    }
    void Heal(){
        GameManager.Instance.Lives = Mathf.Min(3, GameManager.Instance.Lives + 1); 
    }
    void StartInvincible(){
        isInvincible = true;
        Invoke("StopInvincible", 5f);
    }

    void StopInvincible(){
        isInvincible = false;
    }

    public void KillPlayer(){
        PlayerCollider.enabled = false;
        PlayerAnimator.enabled = false;
        PlayerRigidBody.AddForceY(JumpForce, ForceMode2D.Impulse);
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() // 
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded ){
            PlayerRigidBody.AddForceY(JumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            PlayerAnimator.SetInteger("state", 1 );
        }
        
    }
 
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.name == "Platform"){
            if(!isGrounded){
                PlayerAnimator.SetInteger("state", 2 );
            }
            isGrounded = true; 

        }
        
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.tag == "enemy"){
            if(!isInvincible){
                Destroy(collider.gameObject);
                Hit();
            }
        }
        else if(collider.gameObject.tag == "food"){
            Destroy(collider.gameObject);
            Heal();
        }
        else if(collider.gameObject.tag == "golden"){
            Destroy(collider.gameObject);
            StartInvincible();
        }
        
    }
}
