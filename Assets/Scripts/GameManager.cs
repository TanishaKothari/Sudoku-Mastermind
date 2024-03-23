using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int mistakeCount = 0;
    public string chosenVariant;
    private Text variantText;
    public static GameManager _instance;

    private void Awake()
    {
        if(_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } 
        else if(_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        StartCoroutine(SetVariantText());
    }

    private IEnumerator SetVariantText()
    {
        yield return new WaitForEndOfFrame(); // Wait until the end of the frame

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Sudoku")
        {
            Text variantText = GameObject.Find("Canvas").transform.Find("Variant").GetComponent<Text>();
            if (variantText != null)
            {
                switch (chosenVariant)
                {
                    case "Classic":
                        variantText.text = "Classic Sudoku";
                        break;
                    case "KnightsMove":
                        variantText.text = "Knight's Move\nSudoku";
                        break;
                    case "BishopsMove":
                        variantText.text = "Bishop's Move\nSudoku";
                        break;
                    case "KingsMove":
                        variantText.text = "King's Move\nSudoku";
                        break;
                    default:
                        variantText.text = "Unknown Variant";
                        break;
                }
            }
        }
    }

    public void Classic()
    {
        chosenVariant = "Classic";
        SceneManager.LoadScene(1);
    }

    public void KnightsMove()
    {
        chosenVariant = "KnightsMove";
        SceneManager.LoadScene(1);
    }

    public void BishopsMove()
    {
        chosenVariant = "BishopsMove";
        SceneManager.LoadScene(1);
    }

    public void KingsMove()
    {
        chosenVariant = "KingsMove";
        SceneManager.LoadScene(1);
    }

    public void IncrementMistakeCount()
    {
        mistakeCount++;
    }
}