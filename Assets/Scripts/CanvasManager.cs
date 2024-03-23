using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager._instance;
    }

    public void ClassicSelect()
    {
        gameManager.Classic();
    }

    public void KnightsMoveSelect()
    {
        gameManager.KnightsMove();
    }

    public void BishopsMoveSelect()
    {
        gameManager.BishopsMove();
    }

    public void KingsMoveSelect()
    {
        gameManager.KingsMove();
    }
}