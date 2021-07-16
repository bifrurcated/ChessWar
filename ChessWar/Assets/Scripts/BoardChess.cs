using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardChess : MonoBehaviour
{
    DragAndDrop dad = new DragAndDrop();
    void Update()
    {
        dad.Action();
    }
}

class DragAndDrop
{
    State state;
    GameObject item;

    public DragAndDrop() 
    {
        state = State.none;
        item = null;
    }
    enum State
    { 
        none,
        wait,
        drop,
    }
    public void Action()
    {
        switch (state)
        {
            case State.none:
                if(IsMouseButtonPressed())
                Pick();
                break;
            case State.wait:
                if (!IsMouseButtonPressed())
                {
                    state = State.drop;
                }
                break;
            case State.drop:
                if (IsMouseButtonPressed())
                    Drop();
                break;
        }
    }

    private bool IsMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    private void Pick()
    {
        Ray clickPosition = GetClickPosition();
        Transform clickedItem = GetItemAt(clickPosition);
        if (clickedItem == null)
        { 
            return;
        }
        state = State.wait;
        item = clickedItem.gameObject;
        Debug.Log(item.name);
    }

    private Ray GetClickPosition()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private Transform GetItemAt(Ray position)
    {
        RaycastHit hit;
        //Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane"));
        if (Physics.Raycast(position, out hit, 24.5f))
            return hit.transform;
        return null;
    }

    private void Drag()
    {
        item.transform.position = GetClickPosition().origin;
    }

    private void Drop()
    {
        Debug.Log("Drop");
        item = null;
        state = State.none;
    }
}