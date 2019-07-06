using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitController : MonoBehaviour {

    private Tile[,] tiles;
    private int numTilesRegistered = 0;
    private int[,] searchedTiles;

    // Use this for initialization
    void Start () {
		if (tiles == null) {
            tiles = new Tile[6, 4];
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Accept() {
        Debug.Log("You have found a solution!");
    }

    public void Decline() {
        Debug.Log("Not a solution!");
    }
 
    public void TurnTile(int x, int y, float rotation) {
        tiles[x, y].Turn(rotation);
        CheckTiles();
    }

    private void CheckTiles() {
        searchedTiles = new int[6, 4];
        bool[] result = CheckTile(5, 2, Tile.LEFT);

        for (int i = 0; i < result.Length; i++) {
            if (result[i] == false) {
                Decline();
                return;
            }
        }
        Accept();
    }

    private bool[] CheckTile(int x, int y, int inDirection) {
        Debug.Log("Visit " + x + ", " + y + " in direction " + inDirection);
        bool[] result;
        if (x > 5 || x < 0 || y < 0 || y > 3) {
            if (x == 3 && y == 4 && inDirection == Tile.UP) {
                result = CheckTile(2, 3, Tile.DOWN);
                Debug.Log("Go into 230 OUT right");
                if (result[0] == result[1]) {
                    result[2] = true;
                }
                return result;
            } else if (x == 2 && y == 4 && inDirection == Tile.UP) {
                result = CheckTile(3, 3, Tile.DOWN);
                Debug.Log("Got into 230 Out left");
                if (result[0] == result[1]) {
                    result[2] = true;
                }
                return result;
            } else if (x == -1 && y == 1 && inDirection == Tile.LEFT) {
                result = CheckTile(0, 2, Tile.RIGHT);
                result[0] = true;
                Debug.Log("Go into 115 left down");
                return result;
            } else if (x == -1 && y == 2 && inDirection == Tile.LEFT) {
                result = CheckTile(0, 1, Tile.RIGHT);
                result[0] = true;
                Debug.Log("Go into 115 left up");
                return result;
            } else if (x == 2 && y == -1 && inDirection == Tile.DOWN) {
                result = CheckTile(3, 0, Tile.RIGHT);
                result[1] = true;
                Debug.Log("Go into 115 down left");
                return result;
            } else if (x == 3 && y == -1 && inDirection == Tile.DOWN) {
                result = CheckTile(2, 0, Tile.RIGHT);
                result[1] = true;
                Debug.Log("Go into 115 down right");
                return result;
            } else if (x == 6 && y == 1 && inDirection == Tile.RIGHT) {
                Debug.Log("Reached the exit");
                return new bool[4] { false, false, false, true };
            } else {
                Debug.Log("Reached an dead end");
                return new bool[4] { false, false, false, false};
            }
        } else {
            searchedTiles[x, y]++;
            if (searchedTiles[x,y] > 2) {
                Debug.Log("Checked this subtree alread twice. Return!");
                return new bool[4] { false, false, false, false };
            }

            bool[] successors = tiles[x, y].SuccessorDirections(inDirection);
            result = new bool[4];

            for (int i = 0; i < successors.Length; i++) {
                if (successors[i]) {
                    int x1 = x;
                    int y1 = y;
                    if (i == Tile.UP) {
                        y1++;
                        Debug.Log("Go up");
                    }
                    if (i == Tile.RIGHT) {
                        x1++;
                        Debug.Log("Go right");
                    }
                    if (i == Tile.UP) {
                        y1--;
                        Debug.Log("Go down");
                    }
                    if (i == Tile.LEFT) {
                        x1--;
                        Debug.Log("Go left");
                    }
                    bool[] subresult = CheckTile(x1, y1, i);

                    for (int j = 0; j < result.Length; j++) {
                        result[j] = result[j] || subresult[j];
                    }
                }
            }
            Debug.Log("No where to go here. Return");
            return result;
        }
    }

    public void registerTile(int x, int y, bool[,] connections, float rotation) {
        if (tiles == null) {
            tiles = new Tile[6, 4];
        }

        if (tiles[x, y] == null) { 
            tiles[x, y] = new Tile(connections, rotation);
            numTilesRegistered++;
            Debug.Log("Registered " + numTilesRegistered + ". Tile at " + x + ", " + y + " with rotation " + rotation);
        } else {
            Debug.LogWarning("Double register at " + x + ", " + y);
        }
    } 

    public class Tile {

        public static readonly int UP = 0;
        public static readonly int RIGHT = 1;
        public static readonly int DOWN = 2;
        public static readonly int LEFT = 3;

        private bool[,] connections;
        private float rotation;

        public Tile(bool[,] connections, float rotation) {
            this.connections = connections;
            this.rotation = rotation;
        }

        public void Turn(float newRotation) {
            rotation = newRotation;
        }

        public bool[] SuccessorDirections(int direction) {
            int timesTurnesAdditional = 4 - getTimesTurned();
            int localInDirection = (direction + timesTurnesAdditional) % 4;
            bool[] globalOutDirections = new bool[4];
            
            for (int i = 0; i < 4; i++) {
                Debug.Log("Search direction " + i + " from " + localInDirection );
                if (connections[localInDirection, i]) {
                    globalOutDirections[(i + timesTurnesAdditional) % 4] = true;
                }
            }

            globalOutDirections[direction] = false;
            return globalOutDirections;
        }

        private int getTimesTurned() {
            int timesRotated = (int) (rotation % 360)/90;
            Debug.Log("Times rotated " + timesRotated + " degrees " + rotation);
            return timesRotated;
        }
    }
}
