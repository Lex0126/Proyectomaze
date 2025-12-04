using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MazeSolver : MonoBehaviour
{
    [SerializeField]
    private float _mouseSpeed = 5f; // Velocidad de movimiento del ratón

    private MazeCell[,] _mazeGrid;
    private int _width;
    private int _depth;
    
    /// <summary>
    /// Método público llamado por el MazeGenerator para iniciar el proceso.
    /// </summary>
    public void StartSolving(MazeCell[,] grid, int width, int depth)
    {
        _mazeGrid = grid;
        _width = width;
        _depth = depth;

        StartCoroutine(Solve());
    }

    private IEnumerator Solve()
    {
        // 1. Ejecutar el algoritmo de "Relleno" (Flood Fill) para mapear distancias
        yield return FloodFill();

        // 2. Mover el ratón desde la entrada hasta la salida
        yield return MoveMouse();
    }

    /// <summary>
    /// Paso 1: Algoritmo Flood Fill (BFS)
    /// Comienza en la SALIDA y asigna una distancia a cada celda accesible.
    /// </summary>
    private IEnumerator FloodFill()
    {
        Queue<MazeCell> queue = new Queue<MazeCell>();

        // Empezamos en la celda de salida
        MazeCell exit = _mazeGrid[_width - 1, _depth - 1];
        exit.Distance = 0;
        exit.Visit(); // Usamos 'Visit()' para marcar que ya está en la cola
        queue.Enqueue(exit);

        while (queue.Count > 0)
        {
            MazeCell current = queue.Dequeue();

            // Iteramos por todos los vecinos accesibles
            foreach (MazeCell neighbor in GetAccessibleNeighbors(current))
            {
                // Si el vecino no ha sido visitado (no está en la cola)
                if (!neighbor.IsVisited)
                {
                    neighbor.Visit(); // Lo marcamos como visitado
                    neighbor.Distance = current.Distance + 1; // Asignamos la distancia
                    queue.Enqueue(neighbor); // Lo añadimos a la cola para procesar a sus vecinos
                }
            }
        }
        yield return null; // Termina en un frame
        Debug.Log("Flood Fill completado. Distancias asignadas.");
    }

    /// <summary>
    /// Paso 2: Movimiento del Ratón
    /// Comienza en la ENTRADA y sigue el camino de menor distancia.
    /// </summary>
    private IEnumerator MoveMouse()
    {
        // Posición inicial (elevada)
        transform.position = _mazeGrid[0, 0].transform.position + new Vector3(0, 0.1f, 0);
        
        MazeCell currentCell = _mazeGrid[0, 0];
        MazeCell exitCell = _mazeGrid[_width - 1, _depth - 1];

        // Mientras no estemos en la salida
        while (currentCell != exitCell)
        {
            // Obtenemos todos los vecinos a los que podemos movernos
            List<MazeCell> neighbors = GetAccessibleNeighbors(currentCell);

            // De esos vecinos, elegimos el que tenga la distancia MÁS BAJA
            // (Este es el núcleo del algoritmo de "seguir el relleno")
            MazeCell nextCell = neighbors.OrderBy(cell => cell.Distance).First();

            // Movemos el ratón suavemente a la siguiente celda
            Vector3 targetPos = nextCell.transform.position + new Vector3(0, 0.1f, 0);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, _mouseSpeed * Time.deltaTime);
                yield return null; // Espera al siguiente frame
            }
            // Aseguramos la posición final
            transform.position = targetPos; 
            
            // Actualizamos nuestra celda actual
            currentCell = nextCell;
        }

        Debug.Log("¡Laberinto resuelto!");
    }


    /// <summary>
    /// Devuelve una lista de celdas vecinas a las que se puede mover
    /// (es decir, donde no hay un muro bloqueando el paso).
    /// </summary>
    private List<MazeCell> GetAccessibleNeighbors(MazeCell current)
    {
        List<MazeCell> neighbors = new List<MazeCell>();
        int x = current.X;
        int z = current.Z;

        // Vecino Derecho (X + 1)
        if (!current.HasRightWall && x + 1 < _width)
            neighbors.Add(_mazeGrid[x + 1, z]);

        // Vecino Izquierdo (X - 1)
        if (!current.HasLeftWall && x - 1 >= 0)
            neighbors.Add(_mazeGrid[x - 1, z]);

        // Vecino Frontal (Z + 1)
        if (!current.HasFrontWall && z + 1 < _depth)
            neighbors.Add(_mazeGrid[x, z + 1]);

        // Vecino Trasero (Z - 1)
        if (!current.HasBackWall && z - 1 >= 0)
            neighbors.Add(_mazeGrid[x, z - 1]);

        return neighbors;
    }
}