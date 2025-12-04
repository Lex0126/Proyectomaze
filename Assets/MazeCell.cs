using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _unvisitedBlock; // Opcional: para visualizar la generacion

    public bool IsVisited { get; private set; }
    public int X { get; set; }
    public int Z { get; set; }
    public int Distance { get; set; } 

    // Banderas para saber que muros existen (importante para el solver)
    public bool HasLeftWall { get; private set; } = true;
    public bool HasRightWall { get; private set; } = true;
    public bool HasFrontWall { get; private set; } = true;
    public bool HasBackWall { get; private set; } = true;

    private void Awake()
    {
        // El bloque de "no visitado" se desactiva al visitar
        if (_unvisitedBlock != null)
        {
            _unvisitedBlock.SetActive(true);
        }
    }
    
    public void Visit()
    {
        IsVisited = true;
        if (_unvisitedBlock != null)
        {
            _unvisitedBlock.SetActive(false);
        }
    }

    public void ResetVisited()
    {
        IsVisited = false;
        // Opcional: Reactivar el bloque si quieres re-visualizar
        // if (_unvisitedBlock != null)
        // {
        //     _unvisitedBlock.SetActive(true);
        // }
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
        HasLeftWall = false;
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
        HasRightWall = false;
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
        HasFrontWall = false;
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
        HasBackWall = false;
    }
}