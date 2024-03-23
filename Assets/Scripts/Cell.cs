using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Board board;
    private GameManager gameManager;
    public Image cellImage;
    private Text cellText;
    public Text gameOverText;
    public Button homeButton;
    public Button replayButton;

    public int row;
    public int col;
    public int value; // Current value of the cell (0 if empty)
    private bool isSelected = false;
    private static Cell selectedCell;

    private void Start()
    {
        gameManager = GameManager._instance;

        for (int i = 1; i <= 9; i++)
        {
            Button button = GameObject.Find("Button" + i).GetComponent<Button>();
            int num = i; // Capture the current value of i for the delegate
            button.onClick.AddListener(() => PlaceNumber(num));
        }

        Button backspaceButton = GameObject.Find("BackspaceButton").GetComponent<Button>();
        backspaceButton.onClick.AddListener(ClearCell);
    }
    
    public void SetCellValue(int cellValue)
    {
        cellText = GetComponentInChildren<Text>();
        value = cellValue;
        if (value != 0)
        {
            cellText.text = value.ToString();
            cellText.color = Color.black;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        selectedCell = this;
    }

    public void OnDeselect(BaseEventData data)
    {
        isSelected = false;
    }

    void Update()
    {
        if (value == 0 && isSelected)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    value = i;
                    cellText.text = value.ToString();
                    UpdateCellAppearance();
                    break; // Exit the loop after setting the value
                }
            }
        }
        else
        {
            if (cellImage.color == Color.red && Input.inputString == "\b")
            {
                ClearCell();
            }
        }
    }

    private void PlaceNumber(int num)
    {
        if (selectedCell != null && selectedCell == this) // Check if this cell is the selected cell
        {
            value = num;
            cellText.text = value.ToString();
            UpdateCellAppearance();
        }
    }

    private void ClearCell()
    {
        if (selectedCell != null && selectedCell == this && cellImage.color == Color.red)
        {       
            value = 0;
            cellText.text = "";
            cellImage.color = Color.white;
        }
    }

    private void UpdateCellAppearance()
    {
        if(value != 0 && value == board.solution[row, col])
        {
            cellImage.color = Color.white;
            cellText.color = Color.blue;
            board.board[row,col] = value;

            if(board.IsSolved())
            {
                board.gameOverBg.gameObject.SetActive(true);
                gameOverText.gameObject.SetActive(true);
                homeButton.gameObject.SetActive(true);
                replayButton.gameObject.SetActive(true);
                board.timerText.gameObject.SetActive(false);

                gameOverText.text = $"<b>Sudoku Solved!</b>\nTime Taken: {board.timerText.text}\nMistakes Made: {gameManager.mistakeCount}";
            }
        }
        else
        {
            cellImage.color = Color.red;
            cellText.color = Color.white;
            gameManager.IncrementMistakeCount();
        }
    }
}