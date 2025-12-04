using UnityEngine;

public class MazeFinishSpawn : MonoBehaviour
{
    [SerializeField] private MazeGenerator _mazeGenerator; // reference for the player
    [SerializeField] private GameObject _finishPrefab;     // finish
    [SerializeField] private Transform _spawnPoint;        // spawn point

    private void Awake()
    {
        if (_mazeGenerator != null)
        {
            //this search the event on Mazegeneretor, so if the events its true, this script works or not
            _mazeGenerator.OnMazeGenerated += SpawnPlayer;
        }
        else
        {
            Debug.LogWarning("MazeGen is not located in MazeSpawn");
        }
    }

    private void OnDestroy()
    {
        if (_mazeGenerator != null)
        {
            // after we destroy this link for any problem this may cause.
            _mazeGenerator.OnMazeGenerated -= SpawnPlayer;
        }
    }

    private void SpawnPlayer()
    {     // we check for missing links in the game object
        if (_finishPrefab == null || _spawnPoint == null)
        {
            Debug.LogWarning("missing links in spawnplayer");
            return;
        }
        //teleport the player on the spawnpoint
        Instantiate(_finishPrefab, _spawnPoint.position, Quaternion.identity);
        Debug.Log("Player in correct position");
    }
}
