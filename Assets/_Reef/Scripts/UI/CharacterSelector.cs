using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    [Header("Settings")]
    public string sceneName = "GameScene"; 
    //public float hoverScale = 1.1f;      

    [Header("Optional Effects")]
    public GameObject hoverEffect;        

    private Vector3 originalScale;         

    void Start()
    {
        originalScale = transform.localScale;

        if (hoverEffect != null)
            hoverEffect.SetActive(false);
    }

    
    void OnMouseEnter()
    {
      //  transform.localScale = originalScale * hoverScale;

        if (hoverEffect != null)
            hoverEffect.SetActive(true);
    }


    void OnMouseExit()
    {
        transform.localScale = originalScale;

        if (hoverEffect != null)
            hoverEffect.SetActive(false);
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene(sceneName);
    }
}