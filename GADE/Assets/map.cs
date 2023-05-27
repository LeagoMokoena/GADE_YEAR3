using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class map : MonoBehaviour
{
    public GameObject selected;
    public Tiles[] types;

    public GameObject ground;
    public GameObject cursesr;
    public GameObject waypath;
    public fight BMS;
    public manager GMS;
    public GameObject[,] quadOnMap;
    public GameObject[,] quadOnMapForUnitMovementDisplay;
    public GameObject[,] quadOnMapCursor;
    public bool unitSelected = false;

    public List<Node> turrent = null;
    int[,] tiles;
    public GameObject[,] maptiles;
    public HashSet<Node> selectedRange;
    public HashSet<Node> MovementRange;
    public int unitSelectedPreviousX;
    public int unitSelectedPreviousY;

    public GameObject previousOccupiedTile;

    int width = 10;
    int height = 10;
    // Start is called before the first frame update
    void Start()
    {
        selected.GetComponent<prop>().width = (int)selected.transform.position.x;
        selected.GetComponent<prop>().height = (int)selected.transform.position.z;
        selected.GetComponent<prop>().floor = this;
        tiles = new int[width, height];

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                tiles[i, j] = 0;
            }
        }

        tiles[4, 4] = 1;
        tiles[5, 4] = 1;
        tiles[6,4] = 1;
        tiles[7,4] = 1;
        tiles[8,4] = 1;
        build();
    }

    float entering(int widths,int tarwid,int tarhei, int heights)
    {
        Tiles tut = types[tiles[tarwid, tarhei]];

        float cos = tut.move;

        if(widths!=tarwid && heights != tarhei)
        {
            cos += 0.001f;
        }

        return cos;
    }

    public Node[,] nodes;
    void pathGraph()
    {
        nodes = new Node[width, height];

        for( int i = 0; i < width; ++i)
        {
            for( int j = 0; j < height; ++j)
            {
                if(i > 0)
                    nodes[i, j].children.Add(nodes[i - 1, j]); 
                if(i < width - 1)
                    nodes[i, j].children.Add(nodes[i + 1, j]); 
                if(j > 0)
                    nodes[i, j].children.Add(nodes[i-1, j-1]); 
                if(j < height - 1)
                    nodes[i, j].children.Add(nodes[i+1, j + 1]);


            }
        }
    }

    void build()
    {
        for(int i = 0; i < width; ++i)
        {
            for( int j = 0; j < height; ++j)
            {
                Tiles t = types[tiles[i, j]];

                GameObject ob = Instantiate(t.ob, new Vector3(i,0,j), Quaternion.identity);

                click cl = ob.GetComponent<click>();
                cl.width = i; cl.height = j;
                cl.tile = this;
            }
        }
    }

    public Vector3 coord(int wid,int hei)
    {
        return new Vector3(wid, 0, hei);
    }
    public void Move(int wid, int hei)
    {

        Dictionary<Node, float> dis = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        
        List<Node> notReached = new List<Node>();
        Node sour = nodes[selected.GetComponent<prop>().width,selected.GetComponent<prop>().height];

        Node tar = nodes[wid, hei];


        dis[sour] = 0;
        prev[sour] = null;

        foreach( Node node in nodes )
        {
            if( node != sour) 
            {
                dis[node] = Mathf.Infinity; prev[node] = null;
            }

            notReached.Add( node );
        }

        while (notReached.Count > 0) 
        {
            Node iu = null;

            foreach ( Node node in notReached )
            {
                if(iu == null || dis[node] < dis[iu])
                {
                    iu = node;
                }
            }

            if(iu == tar)
            {
                break;
            }

            notReached.Remove( iu );

            foreach( Node de in iu.children)
            {
                float al = dis[iu] + entering(de.w,iu.w,iu.h,de.h);
                if(al < dis[de])
                {
                    dis[de] = al;
                    prev[de] = iu;
                }
            }
        }

        if (prev[tar] == null)
        {
            return;
        }
        
        List<Node> cuur = new List<Node>();

        Node one = tar;
    }
    public void movemant()
    {
        if(selected != null)
        {
            selected.GetComponent<prop>().Next();
        }
    }

    public float costToEnterTile(int wid, int hei)
    {

        if (unitCanEnterTile(wid, hei) == false)
        {
            return Mathf.Infinity;

        }

        //Gotta do the math here
        Tiles t = types[tiles[wid, hei]];
        float dist = t.move;

        return dist;
    }

  
    public bool unitCanEnterTile(int _width, int _height)
    {
        if (maptiles[_width, _height].GetComponent<click>().unitOnTile != null)
        {
            if (maptiles[_width, _height].GetComponent<click>().unitOnTile.GetComponent<prop>().charcterint != selected.GetComponent<prop>().charcterint)
            {
                return false;
            }
        }
        return types[tiles[_width, _height]].Walkable;
    }


    //In:  
    //Out: void
    //Desc: uses a raycast to see where the mouse is pointing, this is used to select units
    public void mouseClickToSelectUnit()
    {
        GameObject tempSelectedUnit;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);



        if (Physics.Raycast(ray, out hit))
        {


            //Debug.Log(hit.transform.tag);
            if (selected == false)
            {

                if (hit.transform.gameObject.CompareTag("Tile"))
                {
                    if (hit.transform.GetComponent<click>().unitOnTile != null)
                    {


                        tempSelectedUnit = hit.transform.GetComponent<click>().unitOnTile;
                        if (tempSelectedUnit.GetComponent<prop>().unitMoveState == tempSelectedUnit.GetComponent <prop>().getMovementStateEnum(0)
                            && tempSelectedUnit.GetComponent<prop>().charcterint == GMS.currentTeam
                            )
                        {
                            disableHighlightUnitRange();
                            selected = tempSelectedUnit;
                            selected.GetComponent<prop>().floor = this;
                            selected.GetComponent<prop>().setMovementState(1);
                            unitSelected = true;

                            highlightUnitRange();
                        }
                    }
                }

                else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))
                {

                    tempSelectedUnit = hit.transform.parent.gameObject;
                    if (tempSelectedUnit.GetComponent<prop>().unitMoveState == tempSelectedUnit.GetComponent<prop>().getMovementStateEnum(0)
                          && tempSelectedUnit.GetComponent<prop>().charcterint == GMS.currentTeam
                        )
                    {

                        disableHighlightUnitRange();
                        selected = tempSelectedUnit;
                        selected.GetComponent<prop>().setMovementState(1);
                        //These were here before I don't think they do anything the unit location is set beforehand
                        //selectedUnit.GetComponent<UnitScript>().x = (int)selectedUnit.transform.position.x;
                        // selectedUnit.GetComponent<UnitScript>().y = (int)selectedUnit.transform.position.z;
                        selected.GetComponent<prop>().floor = this;
                        unitSelected = true;

                        highlightUnitRange();
                    }
                }
            }

        }
    }



    //In:  
    //Out: void
    //Desc: finalizes the movement, sets the tile the unit moved to as occupied, etc
    public void finalizeMovementPosition()
    {
        maptiles[selected.GetComponent<prop>().width, selected.GetComponent<prop>().height].GetComponent<click>().unitOnTile = selected;
        //After a unit has been moved we will set the unitMoveState to (2) the 'Moved' state


        selected.GetComponent<prop>().setMovementState(2);

        highlightUnitAttackOptionsFromPosition();
        highlightTileUnitIsOccupying();
    }



    //In:  
    //Out: void
    //Desc: selects a unit based on the cursor from the other script
    public void mouseClickToSelectUnitV2()
    {

        if (unitSelected == false && GMS.tileBeingDisplayed != null)
        {

            if (GMS.tileBeingDisplayed.GetComponent<click>().unitOnTile != null)
            {
                GameObject tempSelectedUnit = GMS.tileBeingDisplayed.GetComponent<click>().unitOnTile;
                if (tempSelectedUnit.GetComponent<prop>().unitMoveState == tempSelectedUnit.GetComponent<prop>().getMovementStateEnum(0)
                               && tempSelectedUnit.GetComponent<prop>().charcterint == GMS.currentTeam
                               )
                {
                    disableHighlightUnitRange();
                    //selectedSound.Play();
                    selected = tempSelectedUnit;
                    selected.GetComponent<prop>().floor = this;
                    selected.GetComponent<prop>().setMovementState(1);
                    unitSelected = true;
                    highlightUnitRange();

                }
            }
        }

    }
    //In:  
    //Out: void
    //Desc: finalizes the player's option, wait or attack
    public void finalizeOption()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HashSet<Node> attackableTiles = getUnitAttackOptionsFromPosition();

        if (Physics.Raycast(ray, out hit))
        {

            //This portion is to ensure that the tile has been clicked
            //If the tile has been clicked then we need to check if there is a unit on it
            if (hit.transform.gameObject.CompareTag("Tile"))
            {
                if (hit.transform.GetComponent<click>().unitOnTile != null)
                {
                    GameObject unitOnTile = hit.transform.GetComponent<click>().unitOnTile;
                    int unitX = unitOnTile.GetComponent<prop>().width;
                    int unitY = unitOnTile.GetComponent<prop>().height;

                    if (unitOnTile == selected)
                    {
                        disableHighlightUnitRange();
                        Debug.Log("ITS THE SAME UNIT JUST WAIT");
                        selected.GetComponent<prop>().wait();
                        selected.GetComponent<prop>().setMovementState(3);
                        deselectUnit();


                    }
                    else if (unitOnTile.GetComponent<prop>().charcterint != selected.GetComponent<prop>().charcterint && attackableTiles.Contains(nodes[unitX, unitY]))
                    {
                        if (unitOnTile.GetComponent<prop>().currentHealthPoints > 0)
                        {
                            Debug.Log("We clicked an enemy that should be attacked");
                            Debug.Log(selected.GetComponent<prop>().currentHealthPoints);
                            StartCoroutine(BMS.attack(selected, unitOnTile));


                            StartCoroutine(deselectAfterMovements(selected, unitOnTile));
                        }
                    }
                }
            }
            else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))
            {
                GameObject unitClicked = hit.transform.parent.gameObject;
                int unitX = unitClicked.GetComponent<prop>().width;
                int unitY = unitClicked.GetComponent<prop>().height;

                if (unitClicked == selected)
                {
                    disableHighlightUnitRange();
                    Debug.Log("ITS THE SAME UNIT JUST WAIT");
                    selected.GetComponent<prop>().wait();
                    selected.GetComponent<prop>().setMovementState(3);
                    deselectUnit();

                }
                else if (unitClicked.GetComponent<prop>().charcterint != selected.GetComponent<prop>().charcterint && attackableTiles.Contains(nodes[unitX, unitY]))
                {
                    if (unitClicked.GetComponent<prop>().currentHealthPoints > 0)
                    {

                        Debug.Log("We clicked an enemy that should be attacked");
                        Debug.Log("Add Code to Attack enemy");
                        //selectedUnit.GetComponent<UnitScript>().setAttackAnimation();
                        StartCoroutine(BMS.attack(selected, unitClicked));

                        // selectedUnit.GetComponent<UnitScript>().wait();
                        //Check if soemone has won
                        //GMS.checkIfUnitsRemain();
                        StartCoroutine(deselectAfterMovements(selected, unitClicked));
                    }
                }

            }
        }

    }

    //In:  
    //Out: void
    //Desc: de-selects the unit
    public void deselectUnit()
    {

        if (selected != null)
        {
            if (selected.GetComponent<prop>().unitMoveState == selected.GetComponent<prop>().getMovementStateEnum(1))
            {
                disableHighlightUnitRange();
                disableUnitUIRoute();
                selected.GetComponent<prop>().setMovementState(0);


                selected = null;
                unitSelected = false;
            }
            else if (selected.GetComponent<prop>().unitMoveState == selected.GetComponent<prop>().getMovementStateEnum(3))
            {
                disableHighlightUnitRange();
                disableUnitUIRoute();
                unitSelected = false;
                selected = null;
            }
            else
            {
                disableHighlightUnitRange();
                disableUnitUIRoute();
                maptiles[selected.GetComponent<prop>().width, selected.GetComponent<prop>().height].GetComponent<click>().unitOnTile = null;
                maptiles[unitSelectedPreviousX, unitSelectedPreviousY].GetComponent<click>().unitOnTile = selected;

                selected.GetComponent<prop>().width = unitSelectedPreviousX;
                selected.GetComponent<prop>().height = unitSelectedPreviousY;
                selected.GetComponent<prop>().Occup = previousOccupiedTile;
                selected.transform.position = coord(unitSelectedPreviousX, unitSelectedPreviousY);
                selected.GetComponent<prop>().setMovementState(0);
                selected = null;
                unitSelected = false;
            }
        }
    }


    //In:  
    //Out: void
    //Desc: highlights the units range options (this is the portion shown in the video)
    public void highlightUnitRange()
    {


        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> finalEnemyUnitsInMovementRange = new HashSet<Node>();

        int attRange = selected.GetComponent<prop>().attackRange;
        int moveSpeed = selected.GetComponent<prop>().moveSpeed;


        Node unitInitialNode = nodes[selected.GetComponent<prop>().width, selected.GetComponent<prop>().height];
        finalMovementHighlight = getUnitMovementOptions();
        totalAttackableTiles = getUnitTotalAttackableTiles(finalMovementHighlight, attRange, unitInitialNode);
        //Debug.Log("There are this many available tiles for the unit: "+finalMovementHighlight.Count);

        foreach (Node n in totalAttackableTiles)
        {

            if (maptiles[n.w, n.h].GetComponent<click>().unitOnTile != null)
            {
                GameObject unitOnCurrentlySelectedTile = maptiles[n.w, n.h].GetComponent<click>().unitOnTile;
                if (unitOnCurrentlySelectedTile.GetComponent<prop>().charcterint != selected.GetComponent<prop>().charcterint)
                {
                    finalEnemyUnitsInMovementRange.Add(n);
                }
            }
        }


        highlightEnemiesInRange(totalAttackableTiles);
        //highlightEnemiesInRange(finalEnemyUnitsInMovementRange);
        highlightMovementRange(finalMovementHighlight);
        //Debug.Log(finalMovementHighlight.Count);
        MovementRange = finalMovementHighlight;

        //This final bit sets the selected Units tiles, which can be accessible in other functions
        //Probably bad practice, but I'll need to get things to work for now (2019-09-30)
        selectedRange = getUnitTotalRange(finalMovementHighlight, totalAttackableTiles);
        //Debug.Log(unionTiles.Count);

        //Debug.Log("exiting the while loop");
        //This will for each loop will highlight the movement range of the units


    }


    //In:  
    //Out: void
    //Desc: disables the quads that are being used to highlight position
    public void disableUnitUIRoute()
    {
        foreach (GameObject quad in quadOnMapForUnitMovementDisplay)
        {
            if (quad.GetComponent<Renderer>().enabled == true)
            {

                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    //In:  
    //Out: HashSet<Node> of the tiles that can be reached by unit
    //Desc: returns the hashSet of nodes that the unit can reach from its position
    public HashSet<Node> getUnitMovementOptions()
    {
        float[,] cost = new float[width, height];
        HashSet<Node> UIHighlight = new HashSet<Node>();
        HashSet<Node> tempUIHighlight = new HashSet<Node>();
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        int moveSpeed = selected.GetComponent<prop>().moveSpeed;
        Node unitInitialNode = nodes[selected.GetComponent<prop>().width, selected.GetComponent<prop>().height];

        ///Set-up the initial costs for the neighbouring nodes
        finalMovementHighlight.Add(unitInitialNode);
        foreach (Node n in unitInitialNode.children)
        {
            cost[n.w, n.h] = costToEnterTile(n.w, n.h);
            //Debug.Log(cost[n.x, n.y]);
            if (moveSpeed - cost[n.w, n.h] >= 0)
            {
                UIHighlight.Add(n);
            }
        }

        finalMovementHighlight.UnionWith(UIHighlight);

        while (UIHighlight.Count != 0)
        {
            foreach (Node n in UIHighlight)
            {
                foreach (Node neighbour in n.children)
                {
                    if (!finalMovementHighlight.Contains(neighbour))
                    {
                        cost[neighbour.w, neighbour.h] = costToEnterTile(neighbour.w, neighbour.h) + cost[n.w, n.h];
                        //Debug.Log(cost[neighbour.x, neighbour.y]);
                        if (moveSpeed - cost[neighbour.w, neighbour.h] >= 0)
                        {
                            //Debug.Log(cost[neighbour.x, neighbour.y]);
                            tempUIHighlight.Add(neighbour);
                        }
                    }
                }

            }

            UIHighlight = tempUIHighlight;
            finalMovementHighlight.UnionWith(UIHighlight);
            tempUIHighlight = new HashSet<Node>();

        }
        Debug.Log("The total amount of movable spaces for this unit is: " + finalMovementHighlight.Count);
        Debug.Log("We have used the function to calculate it this time");
        return finalMovementHighlight;
    }

    //In:  finalMovement highlight and totalAttackabletiles
    //Out: a hashSet of nodes that are the combination of the two inputs
    //Desc: returns the unioned hashSet
    public HashSet<Node> getUnitTotalRange(HashSet<Node> finalMovementHighlight, HashSet<Node> totalAttackableTiles)
    {
        HashSet<Node> unionTiles = new HashSet<Node>();
        unionTiles.UnionWith(finalMovementHighlight);
        //unionTiles.UnionWith(finalEnemyUnitsInMovementRange);
        unionTiles.UnionWith(totalAttackableTiles);
        return unionTiles;
    }
    //In:  finalMovement highlight, the attack range of the unit, and the initial node that the unit was standing on
    //Out: hashSet Node of the total attackable tiles for the unit
    //Desc: returns a set of nodes that represent the unit's total attackable tiles
    public HashSet<Node> getUnitTotalAttackableTiles(HashSet<Node> finalMovementHighlight, int attRange, Node unitInitialNode)
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        foreach (Node n in finalMovementHighlight)
        {
            neighbourHash = new HashSet<Node>();
            neighbourHash.Add(n);
            for (int i = 0; i < attRange; i++)
            {
                foreach (Node t in neighbourHash)
                {
                    foreach (Node tn in t.children)
                    {
                        tempNeighbourHash.Add(tn);
                    }
                }

                neighbourHash = tempNeighbourHash;
                tempNeighbourHash = new HashSet<Node>();
                if (i < attRange - 1)
                {
                    seenNodes.UnionWith(neighbourHash);
                }

            }
            neighbourHash.ExceptWith(seenNodes);
            seenNodes = new HashSet<Node>();
            totalAttackableTiles.UnionWith(neighbourHash);
        }
        totalAttackableTiles.Remove(unitInitialNode);

        //Debug.Log("The unit node has this many attack options" + totalAttackableTiles.Count);
        return (totalAttackableTiles);
    }


    //In:  
    //Out: hashSet of nodes get all the attackable tiles from the current position
    //Desc: returns a set of nodes that are all the attackable tiles from the units current position
    public HashSet<Node> getUnitAttackOptionsFromPosition()
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        Node initialNode = nodes[selected.GetComponent<prop>().width, selected.GetComponent<prop>().height];
        int attRange = selected.GetComponent<prop>().attackRange;


        neighbourHash = new HashSet<Node>();
        neighbourHash.Add(initialNode);
        for (int i = 0; i < attRange; i++)
        {
            foreach (Node t in neighbourHash)
            {
                foreach (Node tn in t.children)
                {
                    tempNeighbourHash.Add(tn);
                }
            }
            neighbourHash = tempNeighbourHash;
            tempNeighbourHash = new HashSet<Node>();
            if (i < attRange - 1)
            {
                seenNodes.UnionWith(neighbourHash);
            }
        }
        neighbourHash.ExceptWith(seenNodes);
        neighbourHash.Remove(initialNode);
        return neighbourHash;
    }

    //In:  
    //Out: hashSet node that the unit is currently occupying
    //Desc: returns a set of nodes of the tile that the unit is occupying
    public HashSet<Node> getTileUnitIsOccupying()
    {

        int w = selected.GetComponent<prop>().width;
        int h = selected.GetComponent<prop>().height;
        HashSet<Node> singleTile = new HashSet<Node>();
        singleTile.Add(nodes[w, h]);
        return singleTile;

    }

    //In:  
    //Out: void
    //Desc: highlights the selected unit's options
    public void highlightTileUnitIsOccupying()
    {
        if (selected != null)
        {
            highlightMovementRange(getTileUnitIsOccupying());
        }
    }

    //In:  
    //Out: void
    //Desc: highlights the selected unit's attackOptions from its position
    public void highlightUnitAttackOptionsFromPosition()
    {
        if (selected != null)
        {
            highlightEnemiesInRange(getUnitAttackOptionsFromPosition());
        }
    }

    //In:  Hash set of the available nodes that the unit can range
    //Out: void - it changes the quadUI property in the gameworld to visualize the selected unit's movement
    //Desc: This function highlights the selected unit's movement range
    public void highlightMovementRange(HashSet<Node> movementToHighlight)
    {
        foreach (Node n in movementToHighlight)
        {
            quadOnMap[n.w, n.h].GetComponent<Renderer>().material.color = Color.blue;
            quadOnMap[n.w, n.h].GetComponent<MeshRenderer>().enabled = true;
        }
    }



    //In:  Hash set of the enemies in range of the selected Unit
    //Out: void - it changes the quadUI property in the gameworld to visualize an enemy
    //Desc: This function highlights the enemies in range once they have been added to a hashSet
    public void highlightEnemiesInRange(HashSet<Node> enemiesToHighlight)
    {
        foreach (Node n in enemiesToHighlight)
        {
            quadOnMap[n.w, n.h].GetComponent<Renderer>().material.color = Color.red;
            quadOnMap[n.w, n.h].GetComponent<MeshRenderer>().enabled = true;
        }
    }


    //In:  
    //Out: void 
    //Desc: disables the highlight
    public void disableHighlightUnitRange()
    {
        foreach (GameObject quad in quadOnMap)
        {
            if (quad.GetComponent<Renderer>().enabled == true)
            {
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    //In:  
    //Out: void 
    //Desc: moves the unit then finalizes the movement
    public IEnumerator moveUnitAndFinalize()
    {
        disableHighlightUnitRange();
        disableUnitUIRoute();
        while (selected.GetComponent<prop>().movementQueue.Count != 0)
        {
            yield return new WaitForEndOfFrame();
        }
        finalizeMovementPosition();
    }


    //In:  both units engaged in a battle
    //Out:  
    //Desc: deselects the selected unit after the action has been taken
    public IEnumerator deselectAfterMovements(GameObject unit, GameObject enemy)
    {
        //selectedSound.Play();
        selected.GetComponent<prop>().setMovementState(3);
        disableHighlightUnitRange();
        disableUnitUIRoute();
        //If i dont have this wait for seconds the while loops get passed as the coroutine has not started from the other script
        //Adding a delay here to ensure that it all works smoothly. (probably not the best idea)
        yield return new WaitForSeconds(.25f);
        while (unit.GetComponent<prop>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        while (enemy.GetComponent<prop>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();

        }
        Debug.Log("All animations done playing");

        deselectUnit();


    }

    //In:  
    //Out: true if there is a tile that was clicked that the unit can move to, false otherwise 
    //Desc: checks if the tile that was clicked is move-able for the selected unit
    public bool selectTileToMoveTo()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.gameObject.CompareTag("Tile"))
            {

                int clickedTileX = hit.transform.GetComponent<click>().width;
                int clickedTileY = hit.transform.GetComponent<click>().height;
                Node nodeToCheck = nodes[clickedTileX, clickedTileY];
                //var unitScript = selectedUnit.GetComponent<UnitScript>();

                if (MovementRange.Contains(nodeToCheck))
                {
                    if ((hit.transform.gameObject.GetComponent<click>().unitOnTile == null || hit.transform.gameObject.GetComponent<click>().unitOnTile == selected) && (MovementRange.Contains(nodeToCheck)))
                    {
                        Debug.Log("We have finally selected the tile to move to");
                        Move(clickedTileX, clickedTileY);
                        return true;
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("Unit"))
            {

                if (hit.transform.parent.GetComponent<prop>().charcterint != selected.GetComponent<prop>().charcterint)
                {
                    Debug.Log("Clicked an Enemy");
                }
                else if (hit.transform.parent.gameObject == selected)
                {

                    Move(selected.GetComponent<click>().width, selected.GetComponent<prop>().height);
                    return true;
                }
            }

        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (selected != null)
            {
                selected.GetComponent<prop>().Next();
            }
        }
    }


}
