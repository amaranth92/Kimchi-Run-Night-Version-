using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 20f;

    void Start()
    {
        
    }

    void Update()
    {
        // 캐릭터 속도 증가
        transform.position += Vector3.left * GameManager.Instance.CalculateGameSpeed() * 1.2f * Time.deltaTime; //// : 1.2배 속도 증가*** ////
    }
}
