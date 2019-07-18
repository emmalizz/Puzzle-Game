using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private bool p1, p2, p3, p4 = false;
    private int t1, t2;
    private Tile tile1, tile2;

    public List<GameObject> Sprites;
    public List<GameObject> gameBoard = new List<GameObject>();
    private GameObject tileManager;
    private List<GameObject> drawnBoard = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 45; ++i)
        {
            int item = Random.Range(0, 5);
            gameBoard.Add(Sprites[item]);
        }

        checkBoard();
        createBoard();

        tileManager = GameObject.Find("Tile Grid");
        tileManager.GetComponent<TileManager>().click1.AddListener(indOne);
        tileManager.GetComponent<TileManager>().click2.AddListener(indTwo);
        tileManager.GetComponent<TileManager>().object1.AddListener(objOne);
        tileManager.GetComponent<TileManager>().object2.AddListener(objTwo);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (p1 && p2 && p3 && p4) {
            tileSwap();
        }
    }

    //checks specific object's location validity
    bool checkSpot(GameObject item, int index) {
        List<int> adjacentTiles = new List<int> {index - 9 - 1, index - 9, index - 9 + 1, index - 1, index + 1, index + 9 - 1, index + 9, index + 9 + 1};
        List<int> twoSteps = new List<int> {index - 18 - 2, index - 18, index - 18 + 2, index - 2, index + 2, index + 18 - 2, index + 18, index + 18 + 2};
        int counter = 0;
        foreach (int t in adjacentTiles) {
            if (t >= 0 && t <= 44 && twoSteps[counter] >= 0 && twoSteps[counter] <= 44) { 
                if (gameBoard[t].tag == gameBoard[twoSteps[counter]].tag && gameBoard[t].tag == item.tag)
                    return false;
            }
            ++counter;
        }
        return true;
    }


    //spawns new, different object
    GameObject spawnNew(GameObject spawned, int index) {
        foreach (GameObject sprite in Sprites) {
            if (checkSpot(sprite, index)) return sprite;
        }

       return spawned;
    }
    //checks that the initial game board doesn't contain any matches
    void checkBoard() {
        for (int i = 0; i < 45; ++i) { 
            if (!checkSpot(gameBoard[i], i)) gameBoard[i] = spawnNew(gameBoard[i], i);
        }
    }

    //spawns the initial game board
    void createBoard() {
        float x = -8.03f;
        float y = 4.01f;
        int counter = 0;;
        for (int i = 0; i < 45; ++i) {
            var newItem = Instantiate(gameBoard[i], new Vector3(x, y, 0.0f), Quaternion.identity);
            newItem.transform.parent = this.transform;
            drawnBoard.Add(newItem);
            counter += 1;
            if (counter == 9 && i != 0) {
                x = -8.03f;
                y -= 2;
                counter = 0;
            }
            else {
                x += 2;
            }
        }
    }

    //functions to aid in setting swap tile parameters
    void indOne(int one) {
        p1 = true;
        t1 = one;
    }

    void indTwo(int two) {
        p2 = true;
        t2 = two;
    }

    void objOne(Tile t){
        p3 = true;
        tile1 = t;
    }

    void objTwo(Tile t) {
        p4 = true;
        tile2 = t;
    }

    //function to swap tiles
    void tileSwap() {
        p1 = false; p2 = false; p3 = false; p4 = false;
        if (checkSwappable()) {
            changeTileColor(0);
            swapTiles();
        }
        else {
            changeTileColor(1);
        }
    }

    //returns boolean depending on if tiles can be swapped
    bool checkSwappable() {
        return checkAdjacent() && checkMatching();
    }

    //returns boolean depending if tiles are adjacent
    bool checkAdjacent() {
        List<int> adjacentTiles = new List<int> {t1-9,  t1 - 1, t1 + 1,  t1 + 9};
        return adjacentTiles.Contains(t2);
    }

    //returns boolean depending if swap causes 3+ matching tiles in a row
    bool checkMatching() {
        return checkTileMatches(t1, t2) || checkTileMatches(t2, t1);
    }

    //returns boolean depending on if individual tile has valid swaps
    bool checkTileMatches(int item, int newSpot) { 

        GameObject one = gameBoard[t1];
        gameBoard[t1] = gameBoard[t2];
        gameBoard[t2] = one;

        if (rightDiagonalCheck().Count == 0) {
            return false;
        }
        else {
            gameBoard[t2] = gameBoard[t1];
            gameBoard[t1] = one;
            return true;
        }

    }

    //function to change color of second tile to red or green depending on validity
    void changeTileColor(int color) {
        SpriteRenderer sprite2 = tile2.GetComponent<SpriteRenderer>();
        SpriteRenderer sprite1 = tile1.GetComponent<SpriteRenderer>();
        if (color == 0) {
            sprite2.color = Color.green;
        }

        else if (color == 1) {
            sprite2.color = Color.red;
            sprite1.color = Color.red;
            StartCoroutine(backToWhite(sprite1, sprite2));
        }
    } 

    //coroutine to wait a second and turn tiles back to white
    IEnumerator backToWhite(SpriteRenderer s1, SpriteRenderer s2) {
        yield return new WaitForSeconds(1);
        s1.color = Color.white;
        s2.color = Color.white;
    }

    //function that swaps and destroys tiles
    void swapTiles() {
        swapDestroy();
    }

    void swapDestroy() {
        GameObject obj1 = drawnBoard[t1];
        GameObject obj2 = drawnBoard[t2];

        Vector3 newPos1 = new Vector3(obj2.transform.position.x, obj2.transform.position.y, obj2.transform.position.z);
        Vector3 newPos2 = new Vector3(obj1.transform.position.x, obj1.transform.position.y, obj1.transform.position.z);

        drawnBoard[t1].transform.position = newPos1;
        drawnBoard[t2].transform.position = newPos2;

        drawnBoard[t1] = obj2;
        drawnBoard[t2] = obj1;

        GameObject one = gameBoard[t1];
        gameBoard[t1] = gameBoard[t2];
        gameBoard[t2] = one;

        StartCoroutine(destroy());
    }

    //coroutine to wait a second before tiles turn back to white and pieces are destroyed
    IEnumerator destroy(){
        yield return new WaitForSeconds(1);

        SpriteRenderer sprite2 = tile2.GetComponent<SpriteRenderer>();
        SpriteRenderer sprite1 = tile1.GetComponent<SpriteRenderer>();

        sprite2.color = Color.white;
        sprite1.color = Color.white;

        findDestroy();
    }

    //find string of items to destroy
    void findDestroy() {
    //     List<int> horizontalDestroy = horizontalCheck();
    //     foreach (int d in horizontalDestroy) {
    //        Destroy(drawnBoard[d]);
    //        drawnBoard[d] = null;
    //        gameBoard[d] = null; 
    //     }
    //    List<int> verticalDestroy = verticalCheck();
    //    foreach (int i in verticalDestroy) {
    //        Destroy(drawnBoard[i]);
    //        drawnBoard[i] = null;
    //        gameBoard[i] = null; 
    //    }
    //    List<int> ldDestroy = leftDiagonalCheck();
    //    foreach (int i in ldDestroy) {
    //        Destroy(drawnBoard[i]);
    //        drawnBoard[i] = null;
    //        gameBoard[i] = null; 
    //    }
        List<int> rdDestroy = rightDiagonalCheck();
        foreach (int i in rdDestroy) {
           Destroy(drawnBoard[i]);
           drawnBoard[i] = null;
           gameBoard[i] = null; 
       }
    }

    //checks horizontally for matches
    List<int> horizontalCheck() {
        int counter = 1;
        List<int> temp = new List<int>();
        List<int> itemsToDestroy = new List<int>();
        GameObject lastChecked = gameBoard[0];
        int consecutive = 1;
        for (int i = 1; i < 45; ++i) {          
            if (counter == 9) {
                consecutive = 1;
                lastChecked = gameBoard[i];
            }
            else {
                if (lastChecked.tag == gameBoard[i].tag) {
                    if (consecutive == 1) temp.Add(i - 1);
                    consecutive += 1;
                    temp.Add(i);
                }
                else { 
                    if (consecutive >= 3) itemsToDestroy = temp;
                    consecutive = 1;
                    temp = new List<int>();
                }
            }
            lastChecked = gameBoard[i];
            counter++;
        }

        return itemsToDestroy;
    }

    //checks vertically for matches

    List<int> verticalCheck() {
        List<int> itemsToDestroy = new List<int>();
        List<List<int>> indicies = new List<List<int>>();
        List<List<GameObject>> tileItems = new List<List<GameObject>>();

        for (int i = 0; i < 9; ++i) {
            List<int> tempIndex = new List<int>();
            List<GameObject> tempItems = new List<GameObject>();
            for (int j = 0; j < 5; ++j) {
                tempIndex.Add(i + (9 * j));
                tempItems.Add(gameBoard[i + (9 * j)]);
            }
            indicies.Add(tempIndex);
            tileItems.Add(tempItems);
        }

        for (int a = 0; a < tileItems.Count; ++a) {
            GameObject lastChecked = tileItems[a][0];
            int consecutive = 1;
            List<int> temp = new List<int>();
            for (int b = 1; b < tileItems[a].Count; ++b) { 
                if (tileItems[a][b].tag == lastChecked.tag) { 
                    if (consecutive == 1) temp.Add(indicies[a][b-1]);
                    consecutive += 1;
                    temp.Add(indicies[a][b]);
                    lastChecked = tileItems[a][b];
                }
                else {
                     if (consecutive >= 3) { 
                        itemsToDestroy = temp;
                    }
                    temp = new List<int>();
                    consecutive = 1;
                    lastChecked = tileItems[a][b];
                }
            }
            if (consecutive >= 3) { 
                itemsToDestroy = temp;
            }

        }
        return itemsToDestroy;

    }

    //checks diagonally starting at the upper left corner for matches
    List<int> leftDiagonalCheck(){
        List<int> itemsToDestroy = new List<int>();
        List<List<int>> indicies = new List<List<int>>();
        List<List<GameObject>> tileItems = new List<List<GameObject>>();
        List<int> diagonals = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 18, 27, 36};
        List<int> rightBottom = new List<int>  {8, 17, 26, 35, 44, 36, 37, 38, 39, 40, 41, 42, 43};

        foreach (int i in diagonals) {
            List<int> tempIndex = new List<int>();
            List<GameObject> tempItems = new List<GameObject>();
            int increment = 0;
            for (int j = 0; j < 5; ++j) {
                tempIndex.Add(i + (9 * j) + increment);
                tempItems.Add(gameBoard[i + (9 * j) + increment]);
                if (rightBottom.Contains(i + (9 * j) + increment)) break;
                increment += 1;
            }
            indicies.Add(tempIndex);
            tileItems.Add(tempItems);
        }
        for (int a = 0; a < tileItems.Count; ++a) {
            GameObject lastChecked = tileItems[a][0];
            int consecutive = 1;
            List<int> temp = new List<int>();
            for (int b = 1; b < tileItems[a].Count; ++b) {  
                if (tileItems[a][b].tag == lastChecked.tag) { 
                    if (consecutive == 1) temp.Add(indicies[a][b-1]);
                    consecutive += 1;
                    temp.Add(indicies[a][b]);
                    lastChecked = tileItems[a][b];
                }
                else {
                     if (consecutive >= 3) { 
                        itemsToDestroy = temp;
                    }
                    temp = new List<int>();
                    consecutive = 1;
                    lastChecked = tileItems[a][b];
                }
            }
            if (consecutive >= 3) { 
                itemsToDestroy = temp;
            }

        }
        return itemsToDestroy;
    }

    //checks diagonally starting in bottom left corner
    List<int> rightDiagonalCheck() {
        List<int> itemsToDestroy = new List<int>();
        List<List<int>> indicies = new List<List<int>>();
        List<List<GameObject>> tileItems = new List<List<GameObject>>();
        List<int> diagonals = new List<int> {0,9,18,27,36,37,38,39,40,41,42,43,44};
        List<int> topRight = new List<int> {0,1,2,3,4,5,6,7,8,17,26,35,44};

        foreach (int i in diagonals) {
            List<int> tempIndex = new List<int>();
            List<GameObject> tempItems = new List<GameObject>();
            int increment = 0;
            for (int j = 0; j < 5; ++j) {
                tempIndex.Add(i - (9 * j) + increment);
                tempItems.Add(gameBoard[i - (9 * j) + increment]);
                if (topRight.Contains(i - (9 * j) + increment)) break;
                increment += 1;
            }
            indicies.Add(tempIndex);
            tileItems.Add(tempItems);
        }
        for (int a = 0; a < tileItems.Count; ++a) {
            GameObject lastChecked = tileItems[a][0];
            int consecutive = 1;
            List<int> temp = new List<int>();
            for (int b = 1; b < tileItems[a].Count; ++b) {  
                if (tileItems[a][b].tag == lastChecked.tag) { 
                    if (consecutive == 1) temp.Add(indicies[a][b-1]);
                    consecutive += 1;
                    temp.Add(indicies[a][b]);
                    lastChecked = tileItems[a][b];
                }
                else {
                     if (consecutive >= 3) { 
                        itemsToDestroy = temp;
                    }
                    temp = new List<int>();
                    consecutive = 1;
                    lastChecked = tileItems[a][b];
                }
            }
            if (consecutive >= 3) { 
                itemsToDestroy = temp;
            }

        }
        return itemsToDestroy;
    }
}
