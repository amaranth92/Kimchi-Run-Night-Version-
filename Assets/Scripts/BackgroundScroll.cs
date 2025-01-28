using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Settings")]
    public float scrollSpeed; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("References")]
    public MeshRenderer meshRenderer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() // 
    {
        meshRenderer.material.mainTextureOffset += new Vector2(scrollSpeed
        * GameManager.Instance.CalculateGameSpeed() / 20 * Time.deltaTime, 0);
    }
}
