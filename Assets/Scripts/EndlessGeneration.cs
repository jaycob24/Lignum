using System.Collections;
using UnityEngine;

class Tile
{
    public GameObject TileObject;
    public float CreationTime;

    public Tile(GameObject tile, float time)
    {
        TileObject = tile;
        CreationTime = time;
    }
}

public class EndlessGeneration : MonoBehaviour
{
    public GameObject Player;

    [Space]

    public GameObject TileObject;
    public int TileSize;

    [Space]

    public int RenderDistanceX;
    public int RenderDistanceZ;

    Vector3 StartPosition;
    Hashtable Tiles = new Hashtable();

    void Start()
    {
        this.gameObject.transform.position = Vector3.zero;
        StartPosition = Vector3.zero;

        var updateTime = Time.realtimeSinceStartup;

        for (int x = -RenderDistanceX; x <= RenderDistanceX; x++)
        {
            for (int z = -RenderDistanceZ; z <= RenderDistanceZ; z++)
            {
                var position = new Vector3(x * TileSize + StartPosition.x, 0, z * TileSize + StartPosition.z);
                var tile = Instantiate(TileObject, position, Quaternion.identity);

                var tileName = ((int)position.x).ToString() + " " + ((int)position.z).ToString();
                tile.name = tileName;
                Tiles.Add(tileName, new Tile(tile, updateTime));
            }
        }
    }

    void Update()
    {
        var movedX = (long)(Player.transform.position.x - StartPosition.x);
        var movedZ = (long)(Player.transform.position.z - StartPosition.z);

        if (Mathf.Abs(movedX) >= TileSize || Mathf.Abs(movedZ) >= TileSize)
        {
            var updateTime = Time.realtimeSinceStartup;

            var playerX = (long)(Mathf.Floor(Player.transform.position.x / TileSize) * TileSize);
            var playerZ = (long)(Mathf.Floor(Player.transform.position.z / TileSize) * TileSize);

            for (int x = -RenderDistanceX; x <= RenderDistanceX; x++)
            {
                for (int z = -RenderDistanceZ; z <= RenderDistanceZ; z++)
                {
                    var position = new Vector3(x * TileSize + playerX, 0, z * TileSize + playerZ);

                    var tileName = ((int)position.x).ToString() + " " + ((int)position.z).ToString();

                    if (!Tiles.ContainsKey(tileName))
                    {
                        var tile = Instantiate(TileObject, position, Quaternion.identity);
                        tile.name = tileName;
                        Tiles.Add(tileName, new Tile(tile, updateTime));
                    }
                    else
                    {
                        (Tiles[tileName] as Tile).CreationTime = updateTime;
                    }
                }
            }

            var updatedTiles = new Hashtable();

            foreach (Tile tile in Tiles.Values)
            {
                if (tile.CreationTime != updateTime)
                    Destroy(tile.TileObject);
                else
                    updatedTiles.Add(tile.TileObject.name, tile);
            }

            Tiles = updatedTiles;
            StartPosition = Player.transform.position;
        }
    }
}
