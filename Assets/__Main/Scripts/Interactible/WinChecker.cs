using UnityEngine;
using System.Collections.Generic;

public class WinChecker : MonoBehaviour
{
    [SerializeField] public List<GameObject> _EnemiesList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_EnemiesList.Count <= 0)
            {
                Debug.Log("You Have won and saved your friend");
                GameManager.instance.WinGame();
            }
            else
            {
                Debug.Log("Defeat All Nearby Enemies...");
                GameUIManager.instance.ShowPrompt("Defeat All Nearby Enemies First");
            }

        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameUIManager.instance.HidePrompt();
        }

    }



}
