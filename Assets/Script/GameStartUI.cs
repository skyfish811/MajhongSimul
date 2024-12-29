using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    [SerializeField]
    private GameScene _gameScene;
    [SerializeField]
    private Button _startButton;

    public void GameStart()
    {
        _gameScene.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
