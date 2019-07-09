using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
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
        checkBoard(gameBoard);
        createBoard(gameBoard);

        tileManager = GameObject.Find("Tile Grid");
        tileManager.GetComponent<TileManager>().addListener(swapTiles);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //checks specific object's location validity
    bool checkSpot(GameObject item, int index) {
        List<int> leftEdge = new List<int>{0,9,18,27,36};
        List<int> rightEdge = new List<int>{8,17,26,35,44};
        List<int> topEdge = new List<int>{0,1,2,3,4,5,6,7};
        List<int> bottomEdge = new List<int>{36,37,38,39,40,41,42,43,44};
        List<int> corners = new List<int>{0,8,36,44};
        //horizontal
        if (!verticalEdge(index))
            if (gameBoard[index-1] == item && gameBoard[index+1] == item) return false;
        else if (verticalEdge(index)) {
            if (leftEdge.Contains(index) && gameBoard[index+1] == item && gameBoard[index+2] == item) return false;
            else if (rightEdge.Contains(index) && gameBoard[index-1] == item && gameBoard[index-2] == item) return false;
        }
        //vertical
        if (!horizontalEdge(index))
            if (gameBoard[index-9] == item && gameBoard[index+9] == item) return false;
        else if (horizontalEdge(index)) {
            if (topEdge.Contains(index) && gameBoard[index+9] == item && gameBoard[index+18] == item) return false;
            else if (bottomEdge.Contains(index) && gameBoard[index-9] == item && gameBoard[index-18] == item) return false;
        }
        //diagonal
        if (!corners.Contains(index) && !topEdge.Contains(index) && !bottomEdge.Contains(index) && !leftEdge.Contains(index) && !rightEdge.Contains(index)) {
            if (gameBoard[(index-9) - 1] == item && gameBoard[(index+9) + 1] == item) return false;
            if (gameBoard[(index-9) + 1] == item && gameBoard[(index+9) - 1] == item) return false;
        }
        else {
            if (index == 0 && gameBoard[(index+9) + 1] == item && gameBoard[(index+18) + 2] == item) return false;
            else if (index == 8 && gameBoard[(index+9) - 1] == item && gameBoard[(index+18) - 2] == item) return false;
            else if (index == 36 && gameBoard[(index-9) + 1] == item && gameBoard[(index-18) + 2] == item) return false;
            else if (index == 44 && gameBoard[(index-9) - 1] == item && gameBoard[(index-18) - 2] == item) return false;
        }
        //no conflicts
        return true;
    }
    //checks if object is on horizontal edge
    bool horizontalEdge(int index) {
        List<int> edges = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 36, 37, 38, 39, 40, 41, 42, 43, 44};
        if (edges.Contains(index)) return true;
        else return false;
    }
    //checks if object is on the vertical edge
    bool verticalEdge(int index) {
        List<int> edges = new List<int> {0, 9, 18, 27, 36, 8, 17, 26, 35, 44};
        if (edges.Contains(index)) return true;
        else return false;
    }
    //spawns new, different object
    GameObject spawnNew(GameObject spawned, int index) {
        GameObject newObj = Sprites[Random.Range(0,5)];
        if (newObj == spawned) return spawnNew(spawned, index);
         if (checkSpot(newObj, index))
             return newObj;
        else return spawnNew(spawned,index);
        
    }
    //checks that the initial game board doesn't contain any matches
    void checkBoard(List<GameObject> board) {
        for (int i = 0; i < 45; ++i) { 
            if (!checkSpot(board[i], i)) board[i] = spawnNew(board[i], i);
        }
    }

    //spawns the initial game board
    void createBoard(List<GameObject> board) {
        float x = -8.03f;
        float y = 4.01f;
        int counter = 0;;
        for (int i = 0; i < 45; ++i) {
            var newItem = Instantiate(board[i], new Vector3(x, y, 0.0f), Quaternion.identity);
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

    void swapTiles(int a, int b) {
        
    }
}
