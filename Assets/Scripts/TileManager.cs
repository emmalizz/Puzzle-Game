using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class bothItemsClicked: UnityEvent<int> {};
public class TileManager : MonoBehaviour
{
    private int clickedItems = 0;
    private Tile tileOne, tileTwo;
    public List<GameObject> tiles;
    public bothItemsClicked bothClicked = new bothItemsClicked();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 45; ++i)
            tiles[i].GetComponent<Tile>().click.AddListener(tileHit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void tileHit(Tile t) {
        if (clickedItems == 0) {
            tileOne = t;
            clickedItems += 1;
        }
        else if (clickedItems == 1) {
            tileTwo = t;
            clickedItems = 0;

            int t1 = findTile(tileOne);
            int t2 = findTile(tileTwo);
            print(t1); print(t2);
        }
    }

    int findTile(Tile tile) {
        int t = 0;
        for (int i = 0; i < 45; ++i) {
            if (tiles[i].GetComponent<Tile>() == tile)
                t = i;
        }
        return t;
    }
}
