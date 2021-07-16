using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ChessRules;
using System.Linq;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public Button btnQueen;
    public Button btnRook;
    public Button btnBishop;
    public Button btnKnight;
    public GameObject panelPickChess;
    public List<Material> chessMatIcons;
    public GameObject textGameEnd;

    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    public Chessman[,] Chessmans { get; set; }
    private Chessman selectedChessman;

    public GameObject textNotation;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private Dictionary<string, GameObject> chessPrefabs;
    private Dictionary<string, GameObject> figures;

    public int[] EnPassantMove { set; get; }

    private Quaternion orientation;

    public bool isWhiteTurn = true;

    private bool isActiveAnimation = false;
    private bool isPickFigure = false;
    private float animationTime;

    public Chess ChessEngine { set; get; }

    private Vector2 vFromPos, vToPos;

    private void Start()
    {
        Instance = this;
        ChessEngine = new Chess();
        SpawnAllChessmans();
        ShowFigure();
    }

    private void Update()
    {
        if(!isPickFigure)
        { 
            if (!isActiveAnimation)
            {
                UpdateSelection();
                DrawChessBoard();
                if (Input.GetMouseButtonDown(0)) { 
                    if (selectionX >= 0 && selectionY >= 0)
                    {
                        if (selectedChessman == null)
                        {
                            // Select the chessman
                            SelectChessman(selectionX, selectionY);
                        }
                        else
                        {
                            // Move the chessman
                            MoveChessman(selectionX, selectionY);
                        }
                    }
                }
            }
            else {
                ChessAnimation();
            }
        }
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            return;
        }

        if (Chessmans[x, y].isWhite != isWhiteTurn)
        {
            return;
        }
        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtleastOneMove = true;
                }
            }
        }
        if (!hasAtleastOneMove)
        {
            return;
        }
        selectedChessman = Chessmans[x, y];
        selectedChessman.GetComponent<Outline>().enabled = true;
        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            animationTime = 1f;
            if (Chessmans[x, y] == null)
            {
                selectedChessman.GetComponentInChildren<Animator>().SetBool("isRun", true);
            }
            else
            {
                selectedChessman.GetComponentInChildren<Animator>().SetBool("isAttack", true);
            }
            isActiveAnimation = true;
        }
        else {
            BoardHighlights.Instance.Hidehighlights();
            selectedChessman.GetComponent<Outline>().enabled = false;
            selectedChessman = null;
        }
    }

    private void doMove(int x, int y) 
    {
        vFromPos = new Vector2(selectedChessman.CurrentX, selectedChessman.CurrentY);
        vToPos = new Vector2(x, y);
        selectedChessman.GetComponentInChildren<Animator>().SetBool("isRun", false);
        selectedChessman.GetComponentInChildren<Animator>().SetBool("isAttack", false);
        Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        selectedChessman.SetPosition(x, y);
        Chessmans[x, y] = selectedChessman;
        if (selectedChessman.GetType() == typeof(Pawn))
        {
            if (y == 7)
            {
                isPickFigure = true;
                panelPickChess.SetActive(true);
                selectedChessman = Chessmans[x, y];
                btnQueen.GetComponent<Image>().material = chessMatIcons[0];
                btnRook.GetComponent<Image>().material = chessMatIcons[1];
                btnBishop.GetComponent<Image>().material = chessMatIcons[2];
                btnKnight.GetComponent<Image>().material = chessMatIcons[3];
                panelPickChess.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
                return;
                
            }
            else if (y == 0)
            {
                isPickFigure = true;
                panelPickChess.SetActive(true);
                selectedChessman = Chessmans[x, y];
                btnQueen.GetComponent<Image>().material = chessMatIcons[4];
                btnRook.GetComponent<Image>().material = chessMatIcons[5];
                btnBishop.GetComponent<Image>().material = chessMatIcons[6];
                btnKnight.GetComponent<Image>().material = chessMatIcons[7];
                panelPickChess.GetComponent<Image>().color = new Color(1, 1, 1, 0.9f);
                return;
            }
        }
        StartChessMove();
    }

    public void PickChessFigure(int btnId)
    {
        string pawnMorph = "";
        if (isWhiteTurn)
        {
            switch (btnId)
            {
                case 1: pawnMorph = "Q"; break;
                case 2: pawnMorph = "R"; break;
                case 3: pawnMorph = "B"; break;
                case 4: pawnMorph = "N"; break;
            }
        }
        else {
            switch (btnId)
            {
                case 1: pawnMorph = "q"; break;
                case 2: pawnMorph = "r"; break;
                case 3: pawnMorph = "b"; break;
                case 4: pawnMorph = "n"; break;
            }
        }
        isPickFigure = false;
        panelPickChess.SetActive(false);
        StartChessMove(pawnMorph);
    }

    private void StartChessMove(string pawnMorph = "") 
    {
        if (isWhiteTurn)
        {
            Camera.main.transform.position = new Vector3(4.2f, 8, 11);
            Camera.main.transform.rotation = Quaternion.Euler(50, 180, 0);
        }
        else
        {
            Camera.main.transform.position = new Vector3(4.2f, 8, -3.66f);
            Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
        }
        isWhiteTurn = !isWhiteTurn;
        DropObject(vFromPos, vToPos, pawnMorph);
        BoardHighlights.Instance.Hidehighlights();
        selectedChessman.GetComponent<Outline>().enabled = false;
        selectedChessman = null;
       
        if (ChessEngine.YieldValidMoves().Count() == 0)
        {
            textGameEnd.SetActive(true);
            if (isWhiteTurn)
            {
                //Победа чёрных
                textGameEnd.GetComponentInChildren<Text>().text = "Победа за Ордой!";
                textGameEnd.GetComponentInChildren<Text>().fontSize = 42;
                Camera.main.transform.position = new Vector3(4.2f, 8, 11);
                Camera.main.transform.rotation = Quaternion.Euler(50, 180, 0);
            }
            else
            {
                //Победа белых
                textGameEnd.GetComponentInChildren<Text>().text = "Победа за Альянсом!";
                textGameEnd.GetComponentInChildren<Text>().fontSize = 42;
                Camera.main.transform.position = new Vector3(4.2f, 8, -3.66f);
                Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
            }
            isWhiteTurn = !isWhiteTurn;
            ChessEngine = new Chess();
            ShowFigure();
        }
    }

    public void BtnOk()
    {
        textGameEnd.SetActive(false);
    }
    private void ChessAnimation()
    {
        animationTime -= Time.deltaTime;
        if (animationTime < 0)
        {
            isActiveAnimation = false;
            doMove(selectionX, selectionY);
        }
        else 
        {
            Vector3 startPosition = selectedChessman.transform.position;
            Vector3 endPosition = GetTileVector(selectionX,selectionY);
            float speed = 1.8f;

            selectedChessman.transform.position = Vector3.Lerp(startPosition, endPosition, speed * Time.deltaTime);
        }
    }
    private void UpdateSelection() 
    {
        if (!Camera.main) {return;}
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnAllChessmans()
    {
        chessPrefabs = new Dictionary<string, GameObject>();
        figures = new Dictionary<string, GameObject>();
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8,8];
        EnPassantMove = new int[2] { -1,-1};

        chessPrefabs["K"] = chessmanPrefabs[0];
        chessPrefabs["Q"] = chessmanPrefabs[1];
        chessPrefabs["R"] = chessmanPrefabs[2];
        chessPrefabs["B"] = chessmanPrefabs[3];
        chessPrefabs["N"] = chessmanPrefabs[4];
        chessPrefabs["P"] = chessmanPrefabs[5];

        chessPrefabs["k"] = chessmanPrefabs[6];
        chessPrefabs["q"] = chessmanPrefabs[7];
        chessPrefabs["r"] = chessmanPrefabs[8];
        chessPrefabs["b"] = chessmanPrefabs[9];
        chessPrefabs["n"] = chessmanPrefabs[10];
        chessPrefabs["p"] = chessmanPrefabs[11];

        //Create Natation
        for (int i = 0; i < 8; i++)
        {
            Vector3 start = new Vector3(9.15f,0,1*(i-1.67f));
            GameObject go = Instantiate(textNotation, start, Quaternion.Euler(90, 0, 0));
            go.GetComponent<TMPro.TextMeshPro>().text = (i+1).ToString();
            go.GetComponent<TMPro.TextMeshPro>().fontSize = 5;
            go.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start2 = new Vector3(1 * (i+10.40f), 0, -3f);
            GameObject go2 = Instantiate(textNotation, start2, Quaternion.Euler(90, 0, 0));
            go2.GetComponent<TMPro.TextMeshPro>().text = ((char)('a' + i)).ToString();
            go2.GetComponent<TMPro.TextMeshPro>().fontSize = 4;
            go2.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start3 = new Vector3(-1.24f, 0, 1 * (-i + 9.76f));
            GameObject go3 = Instantiate(textNotation, start3, Quaternion.Euler(90, 180, 0));
            go3.GetComponent<TMPro.TextMeshPro>().text = (i + 1).ToString();
            go3.GetComponent<TMPro.TextMeshPro>().fontSize = 5;
            go3.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start4 = new Vector3(1 * (-i - 2.39f), 0, 10.97f);
            GameObject go4 = Instantiate(textNotation, start4, Quaternion.Euler(90, 180, 0));
            go4.GetComponent<TMPro.TextMeshPro>().text = ((char)('a' + i)).ToString();
            go4.GetComponent<TMPro.TextMeshPro>().fontSize = 4;
            go4.GetComponent<TMPro.TextMeshPro>().color = Color.black;
        }
        //Create Figure
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string key = "" + x + y;
                figures[key] = new GameObject("Empty");
            }
        }
    }

    private Vector3 GetTileVector(int x, int y, float z = 0) 
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        origin.y = z;
        return origin;
    }

    private void DrawChessBoard() 
    {
        Vector3 wightLine = Vector3.right * 8;
        Vector3 hightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++) {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + wightLine);
            start = Vector3.right * i;
            Debug.DrawLine(start, start + hightLine);
            for (int j = 0; j <= 8; j++) {} // ???
        }

        //Draw the selection
        if (selectionX >= 0 && selectionY >= 0) {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(
                Vector3.forward * (selectionY + 1)+ Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    public void ShowFigure()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string key = "" + x + y;
                string figure = ChessEngine.GetFigureAt(x, y).ToString();
                if (figures[key] != null && figures[key].name == figure)
                    continue;
                if (figure != ".")
                {
                    Destroy(figures[key]);
                    if (figure.All(char.IsUpper))
                    {
                        orientation = Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        orientation = Quaternion.Euler(0, -90, 0);
                    }
                    if (figure == "N")
                    {
                        figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y, 0.7f), orientation);
                    }
                    else
                    {
                        figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y), orientation);
                    } 
                    figures[key].name = figure;
                    figures[key].AddComponent<Outline>();
                    figures[key].GetComponent<Outline>().OutlineWidth = 8f;
                    figures[key].GetComponent<Outline>().enabled = false;
                    figures[key].transform.SetParent(transform);
                    Chessmans[x, y] = figures[key].GetComponent<Chessman>();
                    Chessmans[x, y].SetPosition(x, y);
                    activeChessman.Add(figures[key]);
                }
                else 
                {
                    Destroy(figures[key]);
                }
            }
        }
    }

    public void DropObject(Vector2 from, Vector2 to, string pawnMorph = "")
    {
        string e2 = VectorToSquare(from);
        string e4 = VectorToSquare(to);
        string figure = ChessEngine.GetFigureAt(e2).ToString();
        string move = figure + e2 + e4 + pawnMorph;
        ChessEngine = ChessEngine.Move(move);
        ShowFigure();
    }
    public string VectorToSquare(Vector2 vector)
    {
        int x = Convert.ToInt32(vector.x);
        int y = Convert.ToInt32(vector.y);
        if(x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            return ((char)('a' + x)).ToString() + (y + 1).ToString();
        }
        return "";
    }
}
