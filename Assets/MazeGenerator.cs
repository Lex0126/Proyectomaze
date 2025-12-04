using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;
    
    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    //
    [Header("Solver")]
    [SerializeField]
    private GameObject _mousePrefab; 
    
    private MazeSolver _mouseInstance;
    

    private MazeCell[,] _mazeGrid;

    public event Action OnMazeGenerated;

    IEnumerator Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);

                
                _mazeGrid[x, z].X = x;
                _mazeGrid[x, z].Z = z;
               
            }
        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);

        //
        ResetAllCellVisitedFlags();
        // 

        Mazesolution();

        //
        OnMazeGenerated += StartSolver;
        OnMazeGenerated?.Invoke();
        // 
    }

    
    private void ResetAllCellVisitedFlags()
    {
        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z].ResetVisited();
            }
        }
    }
    
    
    // 
    private void StartSolver()
    {
        if (_mousePrefab == null)
        {
            Debug.LogWarning("no se asigno un prefab.");
            return;
        }

        
        Vector3 startPos = _mazeGrid[0, 0].transform.position + new Vector3(0, 0.1f, 0); 
        GameObject mouseGO = Instantiate(_mousePrefab, startPos, Quaternion.identity);
        _mouseInstance = mouseGO.GetComponent<MazeSolver>();

        if (_mouseInstance != null)
        {
           
            _mouseInstance.StartSolving(_mazeGrid, _mazeWidth, _mazeDepth);
        }
        else
        {
            Debug.LogError("el prefab no tiene el script.");
        }
    }
    // 


    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        // yield return new WaitForSeconds(0.05f);
        yield return null; 

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }

        } while (nextCell != null);
    }


    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => UnityEngine.Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];
            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];
            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];
            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    private void Mazesolution()
    {
        MazeCell entry = _mazeGrid[0, 0];
        entry.ClearBackWall(); 

        MazeCell exit = _mazeGrid[_mazeWidth - 1, _mazeDepth - 1];
        exit.ClearFrontWall(); 
    }

    void Update()
    {
    }
}