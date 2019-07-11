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
        List<int> adjacentTiles = new List<int> {t1-9, (t1-9) - 1, (t1 - 9) + 1, t1 - 1, t1 + 1, (t1+9) - 1, t1 + 9, (t1 + 9) + 1};
        return adjacentTiles.Contains(t2);
    }

    //returns boolean depending if swap causes 3+ matching tiles in a row
    bool checkMatching() {
        return checkTileMatches(t1, t2) || checkTileMatches(t2, t1);
    }

    //returns boolean depending on if individual tile has valid swaps
    bool checkTileMatches(int t, int newSpot) {
        List<int> adjacentTiles = new List<int> {newSpot-9, (newSpot-9) - 1, (newSpot - 9) + 1, newSpot - 1, newSpot + 1, (newSpot+9) - 1, newSpot + 9, (newSpot + 9) + 1};
        int counter = 0;
        foreach (int tile in adjacentTiles) {
            if (tile >= 0 && tile <= 44) { 
                if (gameBoard[tile].tag == gameBoard[t].tag) {
                    if (counter == 0) {
                        if (newSpot - 18 >= 0 && gameBoard[newSpot - 18].tag == gameBoard[t].tag) return true;
                        else if (newSpot + 9 <= 44 && gameBoard[newSpot + 9].tag == gameBoard[t].tag) return true;
                    }
                    else if (counter == 1) {
                        if (newSpot - 18 - 1 >= 0 && gameBoard[newSpot - 18 - 1].tag == gameBoard[t].tag) return true;
                        else if (newSpot + 9 + 1 <= 44 && gameBoard[newSpot + 9 + 1].tag == gameBoard[t].tag) return true;
                    }
                    else if (counter == 2) {
                        if (newSpot - 18 + 1 >= 0 && gameBoard[newSpot - 18 + 1].tag == gameBoard[t].tag) return true;
                        else if (newSpot + 9 - 1 <= 44 && gameBoard[newSpot + 9 - 1].tag == gameBoard[t].tag) return true;
                    } 
                    else if (counter == 3 || counter == 4) {
                        if (newSpot - 2 >= 0 && gameBoard[newSpot - 2].tag == gameBoard[t].tag) return true;
                        else if (newSpot + 2 <= 44 && gameBoard[newSpot + 2].tag == gameBoard[t].tag) return true;
                    }
                    else if (counter == 5) {
                        if (newSpot + 18 - 1 <= 44  && gameBoard[newSpot + 18 - 1].tag == gameBoard[t].tag) return true;
                        else if (newSpot - 9 + 1 >= 0 && gameBoard[newSpot - 9 + 1].tag == gameBoard[t].tag) return true;
                    }
                    else if (counter == 6) {
                        if (newSpot + 18 <= 44 && gameBoard[newSpot + 18].tag == gameBoard[t].tag) return true;
                        else if (newSpot - 9 >= 0 && gameBoard[newSpot - 9].tag == gameBoard[t].tag) return true;
                    }
                    else if (counter == 7) {
                        if (newSpot + 18 + 1 <= 44 && gameBoard[newSpot + 18 + 1].tag == gameBoard[t].tag) return true;
                        else if (newSpot - 9 - 1 >= 0 && gameBoard[newSpot - 9 - 1].tag == gameBoard[t].tag) return true;
                    }
                }
            }
            ++counter;
        }

        return false;
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
        if (checkTileMatches(t1, t2)) {
            print("numbah one");
        }

        if (checkTileMatches(t2, t1)) {
            print("numbah two");
        }
    }
}
