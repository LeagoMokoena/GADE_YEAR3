using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class prop : MonoBehaviour
{
    public int width;
    public int height;
    public map floor;
    public bool unitInMovement;
    public int charcterint;
    public Queue<int> movementQueue;
    public Queue<int> combatQueue;
    public float visualMovementSpeed = .15f;
    public List<Node> curr = null;
    public Sprite unitSprite;
    public TMP_Text hitPointsText;
    public GameObject Occup;
    public Image healthBar;
    public string unitName;
    public int moveSpeed = 2;
    public int attackRange = 1;
    public int attackDamage = 1;
    public int maxHealthPoints = 5;
    public int currentHealthPoints;
    public GameObject damagedParticle;
    public GameObject holder2D;
    public Canvas damagePopupCanvas;
    public TMP_Text damagePopupText;
    public Image damageBackdrop;
    public bool completedMovement = false;
    public enum movementStates
    {
        Unselected,
        Selected,
        Moved,
        Wait
    }
    public movementStates unitMoveState;

    public movementStates getMovementStateEnum(int i)
    {
        if (i == 0)
        {
            return movementStates.Unselected;
        }
        else if (i == 1)
        {
            return movementStates.Selected;
        }
        else if (i == 2)
        {
            return movementStates.Moved;
        }
        else if (i == 3)
        {
            return movementStates.Wait;
        }
        return movementStates.Unselected;

    }

    public void moveAgain()
    {

        curr = null;
        setMovementState(0);
        completedMovement = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        //gameObject.GetComponentInChildren<Renderer>().material = unitMaterial;
    }
    public void wait()
    {

        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
        //gameObject.GetComponentInChildren<Renderer>().material = unitWaitMaterial;
    }

    public void changeHealthBarColour(int i)
    {
        if (i == 0)
        {
            healthBar.color = Color.blue;
        }
        else if (i == 1)
        {

            healthBar.color = Color.red;
        }
    }
    public void setMovementState(int i)
    {
        if (i == 0)
        {
            unitMoveState = movementStates.Unselected;
        }
        else if (i == 1)
        {
            unitMoveState = movementStates.Selected;
        }
        else if (i == 2)
        {
            unitMoveState = movementStates.Moved;
        }
        else if (i == 3)
        {
            unitMoveState = movementStates.Wait;
        }


    }
    public void setFloor(int w, int h)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Next()
    {
        if (curr.Count == 0)
        {
            return;
        }
        else
        {
            StartCoroutine(moveOver(transform.gameObject, curr[curr.Count - 1]));
        }

    }

    public void dealDamage(int x)
    {
        currentHealthPoints = currentHealthPoints - x;
        updateHealthUI();
    }

    public void updateHealthUI()
    {
        healthBar.fillAmount = (float)currentHealthPoints / maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }

    public void unitDie()
    {
        if (holder2D.activeSelf)
        {
            StartCoroutine(fadeOut());
            StartCoroutine(checkIfRoutinesRunning());

        }

    }
    public IEnumerator checkIfRoutinesRunning()
    {
        while (combatQueue.Count > 0)
        {

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);

    }
    public IEnumerator fadeOut()
    {

        combatQueue.Enqueue(1);
        //setDieAnimation();
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Renderer rend = GetComponentInChildren<SpriteRenderer>();

        for (float f = 1f; f >= .05; f -= 0.01f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();


    }

    public IEnumerator displayDamageEnum(int damageTaken)
    {

        combatQueue.Enqueue(1);

        damagePopupText.SetText(damageTaken.ToString());
        damagePopupCanvas.enabled = true;
        for (float f = 1f; f >= -0.01f; f -= 0.01f)
        {

            Color backDrop = damageBackdrop.GetComponent<Image>().color;
            Color damageValue = damagePopupText.color;

            backDrop.a = f;
            damageValue.a = f;
            damageBackdrop.GetComponent<Image>().color = backDrop;
            damagePopupText.color = damageValue;
            yield return new WaitForEndOfFrame();
        }

        //damagePopup.enabled = false;
        combatQueue.Dequeue();

    }
    public IEnumerator moveOver(GameObject objectToMove, Node endNode)
    {
        movementQueue.Enqueue(1);

        //remove first thing on path because, its the tile we are standing on

        curr.RemoveAt(0);
        while (curr.Count != 0)
        {

            Vector3 endPos = floor.coord(curr[0].w, curr[0].h);
            objectToMove.transform.position = Vector3.Lerp(transform.position, endPos, visualMovementSpeed);
            if ((transform.position - endPos).sqrMagnitude < 0.001)
            {

                curr.RemoveAt(0);

            }
            yield return new WaitForEndOfFrame();
        }
        visualMovementSpeed = 0.15f;
        transform.position = floor.coord(endNode.w, endNode.h);

        width = endNode.w;
        height = endNode.h;
        Occup.GetComponent<click>().unitOnTile = null;
        Occup = floor.maptiles[width, height];
        movementQueue.Dequeue();

    }

    // Update is called once per frame
    void Update()
    {
        if(curr != null)
        {
            int cun = 0;

            while(cun < curr.Count - 1)
            {
                Vector3 st = floor.coord(curr[cun].w, curr[cun].h) + new Vector3(0,0,-1f);
                Vector3 fin = floor.coord(curr[cun+1].w, curr[cun+1].h) + new Vector3(0,0,-1f);

                Debug.DrawLine(st, fin, Color.blue);

                cun++;
            }
        }
    }
}
