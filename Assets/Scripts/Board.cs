using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    private GameManager gameManager;
    public Text gameOverText;
    public Button homeButton;
    public Button replayButton;
    public Text timerText;
    public Image gameOverBg;

	private float secondsCount;
	private int minuteCount;
	private int hourCount;
    public Cell[,] cells = new Cell[9, 9]; // 2D array to store references to Cell GameObjects
    
    [HideInInspector]
    public int[,] board = new int[9, 9]; // Represents the Sudoku board
    
    [HideInInspector]
    public int[,] solution;

    void Start()
    {
        gameManager = GameManager._instance;
        InitializeCellsArray();
        GenerateNewPuzzle();
    }

    void Update()
    {
        UpdateTimerUI();
    }

    public void UpdateTimerUI()
    {
		//set timer UI
		secondsCount += Time.deltaTime;
		timerText.text = hourCount +"h:"+ minuteCount +"m:"+(int)secondsCount + "s";
		if(secondsCount >= 60)
        {
			minuteCount++;
			secondsCount = 0;
		}
        else if(minuteCount >= 60)
        {
			hourCount++;
			minuteCount = 0;
		}	
	}
    private void InitializeCellsArray()
    {
        // Iterate over the rows and columns to dynamically find and assign Cell GameObjects
        for (int row = 0; row < 9; row++)
        {
            Transform rowTransform = transform.Find("Row_" + row);
            if (rowTransform == null)
            {
                Debug.LogError("Row_" + row + " not found.");
                continue;
            }

            for (int col = 0; col < 9; col++)
            {
                Transform cellTransform = rowTransform.Find("Cell_" + col);
                if (cellTransform == null)
                {
                    continue;
                }

                // Get the Cell component from the cell GameObject
                Cell cellComponent = cellTransform.GetComponent<Cell>();
                if (cellComponent == null)
                {
                    continue;
                }
                else
                {
                    // Assign the Cell component to the cells array
                    cells[row, col] = cellComponent;
                }
            }
        }
    }

    public void GenerateNewPuzzle()
    {
        ClearBoard();
        GenerateSolution(); // Generate a complete Sudoku solution
        solution = (int[,])board.Clone();
        RemoveNumbers(); // Remove numbers from the complete solution to create the puzzle

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int cellValue = board[row, col];
                Cell cell = cells[row, col]; // Retrieve the Cell GameObject from the cells array
                cell.SetCellValue(cellValue);
                cell.cellImage.color = Color.white;
            }
        }

        gameManager.mistakeCount = 0;
        gameOverText.gameObject.SetActive(false);
        homeButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        gameOverBg.gameObject.SetActive(false);

        secondsCount = 0;
        minuteCount = 0;
        hourCount = 0;
    }

    private void ClearBoard()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                board[row, col] = 0;
                cells[row,col].value = 0;
                Text cellText = cells[row,col].GetComponentInChildren<Text>();
                cellText.text = "";
            }
        }
    }

    private bool GenerateSolution()
    {
        return Solve(0, 0);
    }

    private bool Solve(int row, int col)
    {
        if (row == 9)
        {
            row = 0;
            if (++col == 9)
            {
                return true; // Completed the entire board
            }
        }

        if (board[row, col] != 0)
        {
            return Solve(row + 1, col);
        }

        for (int num = 1; num <= 9; num++)
        {
            if (IsValidMove(row, col, num))
            {
                board[row, col] = num;
                if (Solve(row + 1, col))
                {
                    return true;
                }
            }
        }

        board[row, col] = 0; // Backtrack
        return false;
    }

    private void RemoveNumbers()
    {
        // Remove numbers to create the puzzle
        int numberOfEmptyCells = UnityEngine.Random.Range(40, 50); // Adjust the range based on difficulty
        while (numberOfEmptyCells > 0)
        {
            int randomRow = UnityEngine.Random.Range(0, 9);
            int randomCol = UnityEngine.Random.Range(0, 9);
            if (board[randomRow, randomCol] != 0)
            {
                board[randomRow, randomCol] = 0;
                numberOfEmptyCells--;
            }
        }
    }

    public bool IsValidMove(int row, int col, int number)
    {
        if (gameManager.chosenVariant == "Classic")
        {
            if(!IsInRow(row, number) && !IsInColumn(col, number) && !IsInSubgrid(row, col, number))
            {
                return true;
            }
        }

        else if(gameManager.chosenVariant == "KnightsMove")
        {   
            if(!IsInRow(row, number) && !IsInColumn(col, number) && !IsInSubgrid(row, col, number) && !IsInKnightMove(row, col, number))
            {
                return true;
            }
        }

        else if(gameManager.chosenVariant == "BishopsMove")
        {
            if(!IsInRow(row, number) && !IsInColumn(col, number) && !IsInSubgrid(row, col, number) && !IsInBishopsMove(row, col, number))
            {
                return true;
            }
        }
        
        else if(gameManager.chosenVariant == "KingsMove")
        {
            if(!IsInRow(row, number) && !IsInColumn(col, number) && !IsInSubgrid(row, col, number) && !IsInKingsMove(row, col, number))
            {
                return true;
            }
        }
        else
        {
            Debug.Log("No chosen variant");
        }
        return false;
    }

    private bool IsInKnightMove(int row, int col, int number)
    {
        int[] knightMoveRowOffsets = { -2, -1, 1, 2, 2, 1, -1, -2 };
        int[] knightMoveColOffsets = { 1, 2, 2, 1, -1, -2, -2, -1 };

        for (int i = 0; i < knightMoveRowOffsets.Length; i++)
        {
            int newRow = row + knightMoveRowOffsets[i];
            int newCol = col + knightMoveColOffsets[i];

            if (IsValidCell(newRow, newCol) && board[newRow, newCol] == number)
            {
                return true; // Number already exists at the end of a knight's move path
            }
        }
        return false;
    }

    private bool IsInBishopsMove(int row, int col, int number)
    {
        int minBound = Mathf.Max(-row, -col);
        int maxBound = Mathf.Min(8 - row, 8 - col);

        for (int i = minBound; i <= maxBound; i++)
        {
            if (i != 0 && IsValidCell(row + i, col + i) && board[row + i, col + i] == number)
            {
                return true;
            }
            
            if (i != 0 && IsValidCell(row + i, col - i) && board[row + i, col - i] == number)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInKingsMove(int row, int col, int number)
    {
        int[] kingMoveRowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] kingMoveColOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < kingMoveRowOffsets.Length; i++)
        {
            int newRow = row + kingMoveRowOffsets[i];
            int newCol = col + kingMoveColOffsets[i];

            if (IsValidCell(newRow, newCol) && board[newRow, newCol] == number)
            {
                return true; // Number already exists at the end of a king's move path
            }
        }
        return false;
    }

    private bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < 9 && col >= 0 && col < 9;
    }

    private bool IsInRow(int row, int number)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == number)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInColumn(int col, int number)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[i, col] == number)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInSubgrid(int row, int col, int number)
    {
        int subgridStartRow = row - row % 3;
        int subgridStartCol = col - col % 3;
        for (int i = subgridStartRow; i < subgridStartRow + 3; i++)
        {
            for (int j = subgridStartCol; j < subgridStartCol + 3; j++)
            {
                if (board[i, j] == number)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsSolved()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if(board[row,col] != solution[row,col])
                {
                    return false;
                }
            }
        }
        // If all cells are filled correctly, the puzzle is solved
        return true;
    }

    public void ShowHint()
    {
        // Find an empty cell
        List<(int, int)> emptyCells = new List<(int, int)>();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    emptyCells.Add((row, col));
                }
            }
        }

        // If there are no empty cells, the puzzle is solved or invalid
        if (emptyCells.Count == 0)
        {
            Debug.Log("No empty cells.");
            return;
        }

        // Select a random empty cell
        (int, int) randomCell = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];

        // Fill the cell with the corresponding number from the solution
        int solutionNumber = solution[randomCell.Item1, randomCell.Item2];
        board[randomCell.Item1, randomCell.Item2] = solutionNumber;
        cells[randomCell.Item1, randomCell.Item2].SetCellValue(solutionNumber);
    }
    
    public void HomeButton()
    {
        SceneManager.LoadScene(0);
    }
}