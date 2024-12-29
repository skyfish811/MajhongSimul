using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    private GameMain gameMain;
    // Start is called before the first frame update
    private void Awake()
    {
        gameMain.GameStart();
    }

}
