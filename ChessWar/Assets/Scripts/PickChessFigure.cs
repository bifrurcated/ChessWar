using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickChessFigure : MonoBehaviour
{
    public Button btnQueen;
    public Button btnRook;
    public Button btnBishop;
    public Button btnKnight;
    public GameObject queen;
    private Dictionary<string, GameObject> chessPrefabs;

    void Start()
    {
        GameObject figure = Instantiate(queen, btnQueen.transform.position, Quaternion.identity); ;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit))
            {

            }
        }
    }
}
