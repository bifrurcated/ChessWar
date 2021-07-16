using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public List<GameObject> chessmanPrefabs;
    private List<GameObject> figures;
    // Start is called before the first frame update
    void Start()
    {
        figures = new List<GameObject>();
        for (int i = 0; i < chessmanPrefabs.Count; i++)
        {
            if (chessmanPrefabs[i] != null)
            {
                GameObject go = Instantiate(chessmanPrefabs[i], new Vector3(0, 0, i), Quaternion.identity);
                figures.Add(go);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        SylvanasAnimation();
    }

    public bool isRun;
    public bool isAttack;

    public void SylvanasAnimation()
    {
        //Эти команды дают сигнал модели о том что нужно запустить определённую анимацию
        for (int i = 0; i < figures.Count; i++)
        {
            figures[i].GetComponentInChildren<Animator>().SetBool("isRun", isRun);
            figures[i].GetComponentInChildren<Animator>().SetBool("isAttack", isAttack);
        }
        
    }
}
