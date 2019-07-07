using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitController : MonoBehaviour {

    // Save all the cable tiles
    private Tile[,] tiles;
    private int numTilesRegistered = 0;

    // Save all correct configurations
    private bool[,,] correctPattern1 = new bool[4,6, 4] 
        { { { true, false, false, false }, {true, false, true, false}, {true, false, true, false}, {false, true, false, false}, {false, false, false, true}, { true, true, true, true} },
        { { true, false, true, false }, {true, false, false, false}, {true, false, true, false}, { true, true, true, true}, {true, false, false, false}, {true, false, true, false} },
        { { true, false, true, false}, { true, true, true, true}, {true, false, true, false}, {false, false, true, false}, { true, true, true, true}, {true, false, true, false} },
        { { true, true, true, true}, { false, true, false, false}, { false, true, false, true}, { false, true, false, true }, { true, true, true, true }, { true, true, true, true } } };

    private bool[,,] correctPattern2 = new bool[4, 6, 4]
        { { { true, false, false, false }, {true, false, true, false}, {true, false, true, false}, {false, false, true, false}, { true, true, true, true}, { true, true, true, true} },
        { { true, false, true, false }, {true, false, false, false}, {true, false, false, false}, { true, true, true, true}, {true, false, true, false}, {true, false, true, false} },
        { { true, false, true, false}, { true, true, true, true}, {true, false, true, false}, {false, false, true, false}, { true, true, true, true}, {true, false, true, false} },
        { { true, true, true, true}, { false, true, false, false}, { false, true, false, true}, { false, true, false, true }, { true, true, true, true }, { true, true, true, true } } };

    /**
     * Generate a new array on start if there is none
     */
    void Start () {
		if (tiles == null) {
            tiles = new Tile[6, 4];
        }
	}

    /**
     * The current state is accepted. The quest is fullfilled
     */
    public void Accept() {
        Debug.Log("You have found a solution!");
    }

    /**
     * The current state is not correct.
     */
    public void Decline() {
        Debug.Log("Not a solution!");
    }

    /**
     * Is called by a cable if it is turned
     */
    public void TurnTile(int x, int y, float rotation) {
        tiles[x, y].Turn(rotation);

        if (numTilesRegistered == 24) {
            CheckTiles();
        }
    }

    /**
     * Checks the current configuration
     */
    private void CheckTiles() {
        bool patternMatches = true;

        // First check some global constraints
        if (tiles[5,1].getTimesTurned() == 2 && tiles[5,2].getTimesTurned() == 0) {
            Debug.Log("Global constraints failed");
            Decline();
            return;
        }

        // Check first pattern
        for (int x = 0; x < 6; x++) {
            for (int y = 0; y < 4; y++) {
                if (!correctPattern1[y,x,tiles[x,y].getTimesTurned()]) {
                    patternMatches = false;
                    Debug.Log("Pattern 1 fails because of " + x + ", " + y + ". Turned: " + tiles[x, y].getTimesTurned());
                    goto Correct1End;
                }
            }
        }

    Correct1End:
        if (patternMatches) {
            Debug.Log("First pattern matches");
            Accept();
            return;
        }
        patternMatches = true;

        // Check second pattern
        for (int x = 0; x < 6; x++) {
            for (int y = 0; y < 4; y++) {
                if (!correctPattern2[y, x, tiles[x, y].getTimesTurned()]) {
                    patternMatches = false;
                    Debug.Log("Pattern 2 fails because of " + x + ", " + y + ". Turned: " + tiles[x, y].getTimesTurned()); ;
                    goto Correct2End;
                }
            }
        }
    Correct2End:
        if (patternMatches) {
            Debug.Log("Second patter matches");
            Accept();
        } else {
            Decline();
        }
    }
        
    /**
     * Cable register theirself with this method. 
     */
    public void registerTile(int x, int y, float rotation) {
        if (tiles == null) {
            tiles = new Tile[6, 4];
        }

        if (tiles[x, y] == null) { // Save position and rotation
            tiles[x, y] = new Tile(rotation);
            numTilesRegistered++;
            Debug.Log("Registered " + numTilesRegistered + ". Tile at " + x + ", " + y + " with rotation " + rotation);
        } else {                   // This tile is already registered
            Debug.LogWarning("Double register at " + x + ", " + y);
        }
    } 

    /**
     * The tile class. Only used by the circuit controller
     */
    private class Tile {
        
        private float rotation;

        public Tile(float rotation) {
            this.rotation = rotation;
        }

        public void Turn(float newRotation) {
            rotation = newRotation;
        }

        public int getTimesTurned() {
            int timesRotated = (int) (rotation % 360)/90;
            return timesRotated;
        }
    }
}
