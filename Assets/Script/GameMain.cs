using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEditor.Progress;
public enum REACHTYPE
{
    CHECK0,
    CEHCK1,
    CEHCK2
}
public class GameMain : MonoBehaviour
{
    private float time = 0;
    //핸드 확인 ui
    [SerializeField]
    TextMeshProUGUI _uiText;
    [SerializeField]
    TextMeshProUGUI _countText1;
    [SerializeField]
    TextMeshProUGUI _countText10;
    [SerializeField]
    TextMeshProUGUI _countText20;
    [SerializeField]
    TextMeshProUGUI _countText130;
    //부모 개체
    [SerializeField]
    private GameObject _deckParent;
    [SerializeField]
    private GameObject _doraParent;
    [SerializeField]
    private GameObject _uraParent;
    [SerializeField]
    private GameObject _tilePrefab;
    [SerializeField]
    private GameObject _removeParent;
    //핸드 카운팅 배열
    private int[] _handCount = new int[41];
    //실제 핸드
    private GameObject[] _realHand = new GameObject[14];
    //실제 타일 모음
    private GameObject[] _deck = new GameObject[136];
    private int _TusmoCount = 28;
    //버림패 모음
    private GameObject[] _removeDeck = new GameObject[136];
    private int _RemoveCount = 0;
    [SerializeField]
    private Image[] _doraSprite;
    //도라 표시 패 모음
    private GameObject[] _dora = new GameObject[4];
    private GameObject[] _ura = new GameObject[4];
    private int[] _doraCount = new int[41];
    //영상 표시
    private GameObject[] _deadwall = new GameObject[4];
    //깡 표시 카운트
    private int _KkangCount = 0;
    private int[] KkangCountset = new int[41];
    [SerializeField]
    Button _KkangButton;
    private List<GameObject> _KkangSet = new List<GameObject>();
    //리치 관련
    [SerializeField]
    Button _ReachButton;
    private List<int> discard = new List<int>();
    public GameObject _reach;
    public bool _isMade = false;
    public bool _reachStart = false;
    private int[] head = new int[41];
    private int[] forMade = new int[41];
    private List<int> madeCount = new List<int>();
    private bool ReachButtonSelected = false;
    public int _direction = Math.Clamp(0, 0, 3);
    public int _myDirection = Math.Clamp(0, 0, 3);
    //텐파이 확인 변수
    public bool _istenpai = false;
    //랜덤 인수
    private static System.Random rand = new System.Random();

    private static GameMain instance;
    private bool _gameStart;

    public static GameMain Instance()
    {
        if (instance == null)
        {
            //instance = this;
        }
        return instance;
    }
    //피셔 예이츠 셔플 이용
    private void Suffle(GameObject[] array)
    {
        int n = array.Length;
        int last = n - 2;
        for (int i = 0; i <= last; i++)
        {
            int r = rand.Next(i, n);
            Swap(i, r);
        }
        //local method
        void Swap(int idxA, int idxB)
        {
            var temp = array[idxA];
            array[idxA] = array[idxB];
            array[idxB] = temp;
        }
    }
    //패 분류
    private void classification()
    {
        for (int i = 0; i < _deck.Length; i++)
        {
            var _tile = _deck[i].GetComponent<Tile>();
            if (_tile.Index < 16)
            {
                switch (_tile.Index % 4)
                {
                    case 0:
                        _tile.type = Type.EAST;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);

                        break;
                    case 1:
                        _tile.type = Type.WEST;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);
                        break;
                    case 2:
                        _tile.type = Type.SOUTH;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);

                        break;
                    case 3:
                        _tile.type = Type.NORTH;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);
                        break;
                }
            }
            else if (_tile.Index < 28)
            {
                switch (_tile.Index % 3)
                {
                    case 0:
                        _tile.type = Type.WHITE;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);
                        break;
                    case 1:
                        _tile.type = Type.SHOOT;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);
                        break;
                    case 2:
                        _tile.type = Type.MIDDLE;
                        _tile.Index = 0;
                        _tile.setName();
                        _tile.setText();
                        _tile.setImage(_tile.Index + (int)_tile.type);
                        break;
                    default:
                        break;
                }
            }
            else if (_deck[i].GetComponent<Tile>().Index < 64)
            {
                _tile.type = Type.CIRCLE;
                _tile.Index = (_tile.Index % 9 + 1);
                _tile.setName();
                _tile.setText();
                _tile.setImage(_tile.Index + (int)_tile.type);

            }
            else if (_tile.Index < 100)
            {
                _tile.type = Type.BAMBOO;
                _tile.Index = (_tile.Index % 9 + 1);
                _tile.setName();
                _tile.setText();
                _tile.setImage(_tile.Index + (int)_tile.type);
            }
            else if (_tile.Index < 136)
            {
                _tile.type = Type.CRACK;
                _tile.Index = (_tile.Index % 9 + 1);
                _tile.setName();
                _tile.setText();
                _tile.setImage(_tile.Index + (int)_tile.type);
            }
        }
    }
    //덱 간이 정렬
    public void setDeck()
    {
        for (int i = 0; i < _deck.Length; i++)
        {
            _deck[i].transform.position = new Vector3(100, 0, 0);
        }
    }
    //처음 패 배패
    public void setFirst()
    {
        _CountRefresh();
        //영상 패 정리
        for (int i = 0; i < _deadwall.Length; i++)
        {
            _deadwall[i] = _deck[i];
        }
        //도라 정리
        for (int i = 0; i < _dora.Length; i++)
        {
            _dora[i] = _deck[i + 4];
            _dora[i].transform.position = new Vector3(-2.5f + i, 4f, 0);
            _dora[i].transform.SetParent(_doraParent.transform);
            if (i >= 1)
            {
                _dora[i].GetComponent<Tile>().setImage(40);
            }
        }
        for (int i = 0; i < _ura.Length; i++)
        {
            _ura[i] = _deck[i + 9];
            _ura[i].transform.position = new Vector3(-2.5f + i, 2.75f, 0);
            _ura[i].transform.SetParent(_uraParent.transform);
            _ura[i].GetComponent<Tile>().setImage(40);
        }
        for (int i = 0; i < _realHand.Length; i++)
        {
            _realHand[i] = _deck[i + 14];
            _deck[i + 14] = null;
            int index = _realHand[i].GetComponent<Tile>().Index + (int)_realHand[i].GetComponent<Tile>().type;
            _handCount[index] += 1;
        }
    }
    //핸드 카운팅 배열 초기화
    public void _CountRefresh()
    {
        for (int i = 0; i < _handCount.Length; i++)
        {
            _handCount[i] = 0;
        }
    }
    //핸드 카운트 배열에 실제 핸드 값 넣기
    public void _SetCount()
    {
        _CountRefresh();
        _countText1.text = "";
        _countText10.text = "";
        _countText20.text = "";
        _countText130.text = "";
        for (int i = 0; i < _realHand.Length; i++)
        {
            var item = _realHand[i].GetComponent<Tile>();
            int index = (int)item.type + item.Index;
            _handCount[index] += 1;

        }
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (i <= 10)
            {
                _countText1.text += ", " + _handCount[i].ToString();
            }
            else if (i <= 20)
            {
                _countText10.text += ", " + _handCount[i].ToString();
            }
            else if (i <= 30)
            {
                _countText20.text += ", " + _handCount[i].ToString();
            }
            else if (i <= 40)
            {
                _countText130.text += ", " + _handCount[i].ToString();
            }
        }
    }
    //실제 핸드 배치 및 ui상 표시
    public void _realHandSet(bool hand = true)
    {
        SelectSorting(_realHand, hand);
        for (int i = 0; i < _realHand.Length; i++)
        {
            _realHand[i].transform.position = new Vector3(i, -1, 0);
            //핸드 레이어 설정
            if (_realHand[i].GetComponent<Tile>().type != Type.DUMMI)
            {
                _realHand[i].transform.gameObject.layer = LayerMask.NameToLayer("Hand");
            }
            else
            {
                _realHand[i].transform.gameObject.layer = LayerMask.NameToLayer("Dummi");
            }
        }
    }
    //방법 1(머리1, 몸통3)
    public void check1()
    {
        discard.Clear();
        int middleCount = _KkangCount;
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 2 && i < 40)
            {
                var newMyHand = (int[])_handCount.Clone();
                newMyHand[i] -= 2;
                int tenpai1 = CheckMiddle1(newMyHand, middleCount);
                int tenpai2 = CheckMiddle2(newMyHand, middleCount);
                int tenpai3 = CheckMiddle3(newMyHand, middleCount);
                int tenpai4 = CheckMiddle4(newMyHand, middleCount);
                if (tenpai1 == 4 || tenpai2 == 4
                    || tenpai3 == 4 || tenpai4 == 4)
                {
                    Debug.Log("텐파이");
                    _istenpai = true;
                }
            }
        }

    }
    public void check2()
    {
        var newMyHand2 = (int[])_handCount.Clone();
        int middleCount2 = _KkangCount;
        int tenpai1 = chcekBody(newMyHand2, middleCount2);
        int tenpai2 = CheckBody2(newMyHand2, middleCount2);
        int tenpai3 = chcekBody3(newMyHand2, middleCount2);
        int tenpai4 = chcekBody4(newMyHand2, middleCount2);
        if (tenpai1 == 4 || tenpai2 == 4
            || tenpai3 == 4 || tenpai4 == 4)
        {
            Debug.Log("텐파이");
            _istenpai = true;
        }
    }
    ///방법 1(머리1, 몸통3)
    public int CheckMiddle1(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();

        if (middleCount < 3)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                if (i < 28 && newMyHand[i] >= 1 && newMyHand[i + 1] >= 1 && newMyHand[i + 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    newMyHand[i + 2] -= 1;
                    middleCount++;
                    return CheckMiddle1(newMyHand, middleCount);
                }
                //커쯔 찾기
                if (Hand[i] == 3 && i <= 38)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckMiddle1(newMyHand, middleCount);
                }
            }
            return middleCount;
        } else if (middleCount >= 3)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                //커쯔 찾기
                if (newMyHand[i] == 2 && i <= 38)
                {
                    newMyHand[i] -= 2;
                    forMade[i] += 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }//슌쯔 찾기
                else if (i <= 27 && newMyHand[i] >= 1 && newMyHand[i + 1] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    forMade[i + 2] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }//가운데가 빈 슌쯔 찾기
                else if (newMyHand[i] >= 1 && newMyHand[i + 2] >= 1 && i <= 27)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 2] -= 1;
                    forMade[i + 1] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    public int CheckMiddle2(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();

        if (middleCount < 3)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                if (Hand[i] == 3 && i <= 38)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckMiddle2(newMyHand, middleCount);
                }
                if (i < 28 && newMyHand[i] >= 1 && newMyHand[i + 1] >= 1 && newMyHand[i + 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    newMyHand[i + 2] -= 1;
                    middleCount++;
                    return CheckMiddle2(newMyHand, middleCount);
                }

            }
            return middleCount;
        }
        else if (middleCount >= 3)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                //커쯔 찾기
                if (newMyHand[i] == 2 && i <= 38)
                {
                    newMyHand[i] -= 2;
                    forMade[i] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }//슌쯔 찾기
                else if (i <= 28 && newMyHand[i] >= 1 && newMyHand[i + 1] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    forMade[i + 2] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
                else if (newMyHand[i] >= 1 && newMyHand[i + 2] >= 1 && i <= 27)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 2] -= 1;
                    forMade[i + 1] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    //역순으로 버림패 찾기
    public int CheckMiddle3(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();

        if (middleCount < 3)
        {
            for (int i = 38; i > 0; i--)
            {
                if (Hand[i] == 3)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckMiddle3(newMyHand, middleCount);
                }
                if (i < 28 && newMyHand[i] >= 1 && newMyHand[i - 1] >= 1 && newMyHand[i - 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 1] -= 1;
                    newMyHand[i - 2] -= 1;
                    middleCount++;
                    return CheckMiddle3(newMyHand, middleCount);
                }
            }
            return middleCount;
        }
        else if (middleCount >= 3)
        {
            for (int i = 38; i > 0; i--)
            {
                //커쯔 찾기
                if (newMyHand[i] == 2)
                {
                    newMyHand[i] -= 2;
                    forMade[i] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }//슌쯔 찾기
                else if (i <= 28 && newMyHand[i] >= 1 && newMyHand[i - 1] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 1] -= 1;
                    forMade[i - 2] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
                else if (newMyHand[i] >= 1 && newMyHand[i - 2] >= 1 && i <= 27)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 2] -= 1;
                    forMade[i - 1] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    //역순으로 버림패 찾기
    public int CheckMiddle4(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();

        if (middleCount < 3)
        {
            for (int i = 38; i > 0; i--)
            {
                if (i < 28 && newMyHand[i] >= 1 && newMyHand[i - 1] >= 1 && newMyHand[i - 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 1] -= 1;
                    newMyHand[i - 2] -= 1;
                    middleCount++;
                    return CheckMiddle4(newMyHand, middleCount);
                }
                if (Hand[i] == 3)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckMiddle4(newMyHand, middleCount);
                }
            }
            return middleCount;
        }
        else if (middleCount >= 3)
        {
            for (int i = 38; i > 0; i--)
            {
                //커쯔 찾기
                if (newMyHand[i] == 2)
                {
                    newMyHand[i] -= 2;
                    forMade[i] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }//슌쯔 찾기
                else if (i <= 28 && newMyHand[i] >= 1 && newMyHand[i - 1] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 1] -= 1;
                    forMade[i - 2] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
                else if (newMyHand[i] >= 1 && newMyHand[i - 2] >= 1 && i <= 27)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i - 2] -= 1;
                    forMade[i - 1] = 1;
                    for (int j = 0; j < newMyHand.Length; j++)
                    {
                        if (newMyHand[j] >= 1)
                        {
                            discard.Add(j);
                        }
                    }
                    return 4;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    //경우 2 (몸통4)
    public int chcekBody(int[] handcount, int MidC)
    {
        int middleCount = MidC;
        var _countClone = (int[])handcount.Clone();
        if (middleCount < 4)
        {
            for (int i = 0; i < _handCount.Length; i++)
            {
                if (i < 28 && _countClone[i] >= 2 && _countClone[i + 1] >= 2 && _countClone[i + 2] >= 2)
                {
                    _countClone[i] -= 1;
                    _countClone[i + 1] -= 1;
                    _countClone[i + 2] -= 1;
                    middleCount++;
                    return chcekBody(_countClone, middleCount);
                }
                //커쯔 찾기
                else if (_countClone[i] == 3 && i < 39)
                {
                    _countClone[i] -= 3;
                    middleCount++;
                    return chcekBody(_countClone, middleCount);
                }
            }
            return 0;
        }
        else if (middleCount >= 4)
        {
            for (int i = 0; i < _countClone.Length; i++)
            {
                if (_countClone[i] >= 1)
                {
                    forMade[i] = 1;
                    discard.Add(i);
                }
            }
            return 4;
        }
        else
        {
            return 0;
        }
    }
    public int chcekBody2(int[] handcount, int MidC)
    {
        int middleCount = MidC;
        var _countClone = (int[])handcount.Clone();
        if (middleCount < 4)
        {
            for (int i = 0; i < _handCount.Length; i++)
            {
                if (_countClone[i] == 3 && i < 39)
                {
                    _countClone[i] -= 3;
                    middleCount++;
                    return chcekBody2(_countClone, middleCount);
                }
                if (i < 28 && _countClone[i] >= 2 && _countClone[i + 1] >= 2 && _countClone[i + 2] >= 2)
                {
                    _countClone[i] -= 1;
                    _countClone[i + 1] -= 1;
                    _countClone[i + 2] -= 1;
                    middleCount++;
                    return chcekBody2(_countClone, middleCount);
                }
            }
            return 0;
        }
        else if (middleCount >= 4)
        {
            for (int i = 0; i < _countClone.Length; i++)
            {
                if (_countClone[i] >= 1)
                {
                    forMade[i] = 1;
                    discard.Add(i);
                }
            }
            return 4;
        }
        else
        {
            return 0;
        }
    }
    public int chcekBody3(int[] handcount, int MidC)
    {
        int middleCount = MidC;
        var _countClone = (int[])handcount.Clone();
        if (middleCount < 4)
        {
            for (int i = 38; i > 0; i--)
            {
                if (_countClone[i] == 3)
                {
                    _countClone[i] -= 3;
                    middleCount++;
                    return chcekBody3(_countClone, middleCount);
                }
                if (i < 28 && _countClone[i] >= 2 && _countClone[i - 1] >= 2 && _countClone[i - 2] >= 2)
                {
                    _countClone[i] -= 1;
                    _countClone[i - 1] -= 1;
                    _countClone[i - 2] -= 1;
                    middleCount++;
                    return chcekBody3(_countClone, middleCount);
                }
            }
            return 0;
        }
        else if (middleCount >= 4)
        {
            for (int i = 0; i < _countClone.Length; i++)
            {
                if (_countClone[i] >= 1)
                {
                    forMade[i] = 1;
                    discard.Add(i);
                }
            }
            return 4;
        }
        else
        {
            return 0;
        }
    }
    public int chcekBody4(int[] handcount, int MidC)
    {
        int middleCount = MidC;
        var _countClone = (int[])handcount.Clone();
        if (middleCount < 4)
        {
            for (int i = 38; i > 0; i--)
            {
                if (i < 28 && _countClone[i] >= 2 && _countClone[i - 1] >= 2 && _countClone[i - 2] >= 2)
                {
                    _countClone[i] -= 1;
                    _countClone[i - 1] -= 1;
                    _countClone[i - 2] -= 1;
                    middleCount++;
                    return chcekBody4(_countClone, middleCount);
                }
                if (_countClone[i] == 3)
                {
                    _countClone[i] -= 3;
                    middleCount++;
                    return chcekBody4(_countClone, middleCount);
                }

            }
            return 0;
        }
        else if (middleCount >= 4)
        {
            for (int i = 0; i < _countClone.Length; i++)
            {
                if (_countClone[i] >= 1)
                {
                    forMade[i] = 1;
                    discard.Add(i);
                }
            }
            return 4;
        }
        else
        {
            return 0;
        }
    }
    //정렬
    public static GameObject[] SelectSorting(GameObject[] array, bool hand = true)
    {
        if (!hand)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < array.Length - 1; j++)
                {
                    if (array[j].GetComponent<Tile>().type < array[minIndex].GetComponent<Tile>().type)
                    {
                        minIndex = j;
                    }
                }
                var temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }

            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < array.Length - 1; j++)
                {

                    if (array[j].GetComponent<Tile>().type == array[minIndex].GetComponent<Tile>().type && array[j].GetComponent<Tile>().Index < array[minIndex].GetComponent<Tile>().Index)
                    {
                        minIndex = j;
                    }
                }

                var temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }
            return array;
        }
        else
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[j].GetComponent<Tile>().type < array[minIndex].GetComponent<Tile>().type)
                    {
                        minIndex = j;
                    }
                }
                var temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }

            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;

                for (int j = i + 1; j < array.Length; j++)
                {

                    if (array[j].GetComponent<Tile>().type == array[minIndex].GetComponent<Tile>().type && array[j].GetComponent<Tile>().Index < array[minIndex].GetComponent<Tile>().Index)
                    {
                        minIndex = j;
                    }
                }

                var temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }
            return array;
        }
    }
    //쯔모
    public void Tusmo(int number = 13)
    {
        _removeDeck[_RemoveCount] = _realHand[number];
        _removeDeck[_RemoveCount].gameObject.layer = LayerMask.NameToLayer("Deck");
        _removeDeck[_RemoveCount].transform.position = new Vector3(110, 110, 0);
        _removeDeck[_RemoveCount].transform.SetParent(_removeParent.transform);
        _realHand[number] = _deck[_TusmoCount];
        //가져온 패를 맨 오른쪽으로 고정시킨다
        var temp = _realHand[13];
        _realHand[13] = _realHand[number];
        _realHand[number] = temp;
        _RemoveCount++;
        _TusmoCount++;
        _realHandSet(false);
        _SetCount();
        KkangExiest();
        discard.Clear();
        //텐파이 상태를 체크한다
        check1();
        check2();
        if (_istenpai)
        {
            _ReachButton.gameObject.SetActive(true);
        }
    }
    public void ReachTusmo(int index = 13)
    {
        _removeDeck[_RemoveCount] = _realHand[index];
        _removeDeck[_RemoveCount].gameObject.layer = LayerMask.NameToLayer("Deck");
        _removeDeck[_RemoveCount].transform.position = new Vector3(110, 110, 0);
        _removeDeck[_RemoveCount].transform.SetParent(_removeParent.transform);
        var temp = _realHand[13];
        _realHand[13] = _realHand[index];
        _realHand[index] = temp;
        _realHand[index] = _deck[_TusmoCount];
        _RemoveCount++;
        _TusmoCount++;
        _realHandSet(false);
        _SetCount();
    }
    public void checkMade()
    {
        for (int i = 0; i < _handCount.Length; i++)
        {
            var newMyHand = (int[])_handCount.Clone();
            if (newMyHand[i] >= 2 && i <= 38)
            {
                int middleCount = _KkangCount;
                newMyHand[i] -= 2;
                int Made = CheckBody1(newMyHand, middleCount);
                int Made2 = CheckBody2(newMyHand, middleCount);
                if (Made == 4 || Made2 == 4)
                {
                    _isMade = true;
                    _handCount[i] -= 2;
                    head[i] += 2;
                    break;
                }
            }
        }
    }
    public int CheckBody1(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();
        if (middleCount < 4)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                //슌쯔 찾기
                if (i < 28 && Hand[i] >= 1 && Hand[i + 1] >= 1 && Hand[i + 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    newMyHand[i + 2] -= 1;
                    middleCount++;
                    return CheckBody1(newMyHand, middleCount);
                }
                //커쯔 찾기
                if (Hand[i] == 3 && i < 40)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckBody1(newMyHand, middleCount);
                }
            }
        } if (middleCount >= 4)
        {
            Debug.Log("메이드");
            return 4;
        }
        return 0;
    }
    public int CheckBody2(int[] Hand, int MidC)
    {
        int middleCount = MidC;
        var newMyHand = (int[])Hand.Clone();
        if (middleCount < 4)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                //커쯔 찾기
                if (Hand[i] == 3 && i < 40)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckBody2(newMyHand, middleCount);
                }
                //슌쯔 찾기
                if (i < 28 && Hand[i] >= 1 && Hand[i + 1] >= 1 && Hand[i + 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    newMyHand[i + 2] -= 1;
                    middleCount++;
                    return CheckBody2(newMyHand, middleCount);
                }
            }
        }
        if (middleCount >= 4)
        {
            Debug.Log("메이드");
            return 4;
        }
        return 0;
    }
    //적도라 생성
    public void SetRed()
    {
        bool CIRCLE = false;
        bool Bamboo = false;
        bool Crack = false;
        for (int i = 0; i < _deck.Length; i++)
        {
            if (_deck[i].GetComponent<Tile>().type == Type.BAMBOO && _deck[i].GetComponent<Tile>().Index == 5
                && Bamboo == false)
            {
                _deck[i].GetComponent<Tile>()._IsRed = true;
                _deck[i].GetComponent<Tile>().setTextClolr(Color.red);
                Bamboo = true;
            }
            if (_deck[i].GetComponent<Tile>().type == Type.CIRCLE && _deck[i].GetComponent<Tile>().Index == 5
                && CIRCLE == false)
            {
                _deck[i].GetComponent<Tile>()._IsRed = true;
                _deck[i].GetComponent<Tile>().setTextClolr(Color.red);
                CIRCLE = true;
            }
            if (_deck[i].GetComponent<Tile>().type == Type.CRACK && _deck[i].GetComponent<Tile>().Index == 5
                && Crack == false)
            {
                _deck[i].GetComponent<Tile>()._IsRed = true;
                _deck[i].GetComponent<Tile>().setTextClolr(Color.red);
                Crack = true;
            }
        }
    }
    //깡 치기
    public void KkangExiest()
    {
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] == 4)
            {
                _KkangButton.gameObject.SetActive(true);
            }
        }
    }
    //깡 구현
    public void Kkang()
    {
        int index = 0;
        int setin = 0;
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] == 4 && i < 40)
            {
                _handCount[i] = 0;
                _doraSprite[_KkangCount + 1].sprite = _dora[1 + _KkangCount * 2].GetComponent<Tile>().getImage();
                index = i;
                break;
            }
        }
        for (int j = 0; j < _realHand.Length; j++)
        {
            var item = _realHand[j].GetComponent<Tile>();
            if (item.Index + (int)item.type == index && setin < 3)
            {
                _KkangSet.Add(_realHand[j]);
                _realHand[j].transform.position = new Vector3(-14 + j, 0, 0);
                _realHand[j] = Instantiate(_tilePrefab);
                var tile = _realHand[j].GetComponent<Tile>();
                tile.type = Type.DUMMI;
                tile.Index = 0;
                tile.setImage(40);
                setin++;
                tile.gameObject.layer = LayerMask.NameToLayer("Dummi");
            } else if (item.Index + (int)item.type == index && setin == 3)
            {
                _KkangSet.Add(_realHand[j]);
                _realHand[j].transform.position = new Vector3(-14 + j, 0, 0);
                KkangTusmo(j);
                _KkangCount++;
            }
        }
        _realHandSet();
        _SetCount();
        KkangCountset[index] += 3;
        _KkangButton.gameObject.SetActive(false);
        KkangExiest();
    }
    //깡 했을 때 영상에서 패를 가져온다.
    public void KkangTusmo(int number)
    {
        _realHand[number] = _deck[_KkangCount];

    }
   //리치 버튼을 눌렀을 때 리치 상태로 들어간다
   //리치 상태는 패의 상태를 고정하고 메이드 될 때까지 패를 가져온다
    public void Reach()
    {
        _ReachButton.gameObject.SetActive(false);
        //버림가능 패 리스트의 중복을 제거한다
        discard = discard.Distinct().ToList();
        int[] dis = discard.ToArray();
        //리치 상태일 떄 버림 가능 패 제외 상호작용을 제거한다.
        foreach (var item2 in _realHand)
        {
            item2.layer = LayerMask.NameToLayer("Dummi");
            item2.GetComponent<Tile>()._isSelect = false;
        }
        for (int i = 0; i < _realHand.Length; i++)
        {
            var item = _realHand[i].GetComponent<Tile>();
            for (int j = 0; j < dis.Length; j++)
            {
                Debug.Log(dis[j]);
                if ((item.Index + (int)item.type) == dis[j])
                {
                    item.transform.localScale = new Vector3(0.9f, 1.2f, 0.5f);
                    item.gameObject.layer = LayerMask.NameToLayer("Hand");
                    item.GetComponent<Tile>()._isSelect = false;
                }
            }
        }
        ReachButtonSelected = true;
    }
    public void beforeCheckScore()
    {

        //깡이 존재하는 경우
        //깡한 내용을 몸통으로 카운트 배열에 추가한다
        if (_KkangCount > 0)
        {
            for (int i = 0; i < _handCount.Length; i++)
            {
                _handCount[i] += KkangCountset[i];
            }
        }
        //도라와 우라를 각 배열에 추가한다
        for (int i = 0; i < _KkangCount + 1; i++)
        {
            var item = _dora[_KkangCount].GetComponent<Tile>();
            var item2 = _ura[_KkangCount].GetComponent<Tile>();

            int index = (int)item.type + item.Index;
            int index2 = (int)item2.type + item2.Index;
            item.setImage(index);
            item2.setImage(index2);
            //각 항목의 끝인 경우 처음이 도라가 된다
            if (index == 9)
            {
                index = 0;
            }
            else if (index == 19)
            {
                index = 10;
            }
            else if (index == 29)
            {
                index = 20;
            }
            else if (index == 34)
            {
                index = 30;
            }
            else if (index == 38)
            {
                index = 35;
            }
            //우라 부분도 바꿔준다
            if (index2 == 9)
            {
                index2 = 0;
            }
            else if (index2 == 19)
            {
                index2 = 10;
            }
            else if (index2 == 29)
            {
                index2 = 20;
            }
            else if (index2 == 34)
            {
                index2 = 30;
            }
            else if (index2 == 38)
            {
                index2 = 35;
            }
            _doraCount[index + 1] += 1;
            _doraCount[index2 + 1] += 1;
        }

    }
    public int checkScore()
    {
        int pan = 1;

        //도라 계산
        for (int i = 0; i < _KkangCount + 1; i++)
        {

            if (_doraCount[i] >= 0)
            {
                pan += _doraCount[i] * _handCount[i];
                pan += _doraCount[i] * head[i];
            }
            Debug.Log("도라 : " + pan);
        }
        Debug.Log("도라 : " + pan);
        //1판 계산
        //탕야오 계산
        pan += Tanyaochuu();
        Debug.Log("탕야오 : " + pan);
        //핑후 계산
        pan += Pinfu();
        Debug.Log("핑후 : " + pan);
        //삼원패 계산
        pan += threewon();
        Debug.Log("삼원패 : " + pan);
        //풍패 계산
        pan += Direction();
        Debug.Log("풍패 : " + pan);
        //2판 계산
        //혼노두 계산
        pan += honnodo();
        Debug.Log("혼노두 : " + pan); 
        //일기통관 계산
        pan += Straight();
        Debug.Log("일기통관 : " + pan);
        //삼색동순 계산
        pan += threeColouerdStright();
        Debug.Log("삼색동순 : " + pan);
        //삼색동각 계산
        pan += threeColouerdTriple();
        Debug.Log("삼색동각 : " + pan);
        //산깡즈 계산
        pan += Sankantsu();
        Debug.Log("산깡쯔 : " + pan);
        //안커 계산
        pan += ankour();
        Debug.Log("안커 : " + pan);
        //혼일색 계산
        pan += onecolor();
        Debug.Log("혼일색 : " + pan);
        //청일색 계산
        pan += onlyonecolor();
        Debug.Log("청일색 : " + pan);
        //이페코 계산
        pan += epeco();
        Debug.Log("이페코 : " + pan);
        //찬타 계산
        int canta1 = chanta1(_handCount);
        int canta2 = chanta2(_handCount);
        if(canta1 == 2 || canta2 == 2)
        {
            pan += 2;
            Debug.Log("찬타 : " + pan);
        }
        return pan;

    }
    //1. 탕야오 계산

    public int Tanyaochuu()
    {
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (i % 9 == 1 && _handCount[i] >= 1 && i < 10)
            {
                return 0;
            }
            else if ((i - 10) % 9 == 1 && _handCount[i] >= 1 && i < 20)
            {
                return 0;
            }
            else if ((i - 20) % 9 == 1 && _handCount[i] >= 1 && i < 30)
            {
                return 0;
            }
            if (i % 9 == 1 && head[i] >= 1)
            {
                return 0;
            }
            else if ((i - 10) % 9 == 1 && head[i] >= 1 && i < 20)
            {
                return 0;
            }
            else if ((i - 20) % 9 == 1 && head[i] >= 1 && i < 30)
            {
                return 0;
            }
        }
            Debug.Log("탕야오");
        return 1;
    }
    //2. 핑후 계산
    public int Pinfu()
    {
        int index = 0;
        int index2 = 0;
        if (_KkangCount > 0)
        {
            return 0;
        }
        //커쯔가 있는지 계산
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 3)
            {
                index++;
            }
        }
        //대기패가 양면인지 계산
        for (int i = 0; i < forMade.Length; i++)
        {
            if (forMade[i] > 0)
            {
                index2++;
            }
        }
        if (index == 0 && index2 == 2)
        {
            Debug.Log("핑후");
            return 1;
        }
        else
        {
            return 0;
        }
    }
    //3. 역패 계산
    public int threewon()
    {
        int pan = 0;

        for (int i = 36; i < _handCount.Length; i++)
        {
            if (_handCount[i] == 3)
            {
                pan++;
            }
        }
        //소사희 계산
        if (_handCount[36] == 3 && _handCount[37] == 3 && head[38] == 2)
        {
            Debug.Log("소사희");
            pan += 2;
        } else if (_handCount[37] == 3 && _handCount[38] == 3 && head[36] == 2)
        {
            Debug.Log("소사희");
            pan += 2;
        } else if (_handCount[38] == 3 && _handCount[36] == 3 && head[37] == 2)
        {
            Debug.Log("소사희");
            pan += 2;
        }
        //대삼원 계산
        if (_handCount[36] == 3 && _handCount[37] == 3 && _handCount[38] == 3)
        {
            Debug.Log("대삼원");
            pan += 14;
        }
        return pan;
    }
    //4. 혼노두 계산
    public int honnodo()
    {
        int pan = 0;
        for (int i = 1; i < _handCount.Length; i++)
        {
            if (i % 9 != 1 && i < 10 && _handCount[i] >= 1)
            {
                pan -= 2;
                break;
            } else if ((i - 10) % 9 != 1 && i < 20 && _handCount[i] >= 1)
            {
                pan -= 2;

                break;
            } else if ((i - 20) % 9 != 1 && i < 30 && _handCount[i] >= 1)
            {
                pan -= 2;
                break;
            }
        }
        if (pan < 0)
        {
            for (int i = 1; i < head.Length; i++)
            {
                if (i % 9 != 1 && i < 10 && head[i] >= 1)
                {
                    pan -= 2;
                    break;
                }
                else if ((i - 10) % 9 != 1 && i < 20 && head[i] >= 1)
                {
                    pan -= 2;

                    break;
                }
                else if ((i - 20) % 9 != 1 && i < 30 && head[i] >= 1)
                {
                    pan -= 2;
                    break;
                }
            }
        }
        pan += 2;
        if (pan > 0)
        {
            Debug.Log("혼노두");
            return 2;
        }
        return 0;
    }
    //5. 삼색동순
    public int threeColouerdStright()
    {
        int pan = 0;
        for (int i = 0; i < 7; i++)
        {
            if (_handCount[i] >= 1 && _handCount[i + 1] >= 1 && _handCount[i + 2] >= 1)
            {
                if (_handCount[i + 10] >= 1 && _handCount[i + 11] >= 1 && _handCount[i + 12] >= 1)
                {
                    if (_handCount[i + 20] >= 1 && _handCount[i + 21] >= 1 && _handCount[i + 22] >= 1)
                    {
                        pan += 2;
                        break;
                    }
                }
            }

        }
        if (pan > 0)
        {
            Debug.Log("삼색동순");
        }
        return pan;
    }
    //6. 삼색동각 
    public int threeColouerdTriple()
    {
        int pan = 0;
        for (int i = 0; i < 10; i++)
        {
            if (_handCount[i] == 3)
            {
                if (_handCount[i + 10] >= 1 && _handCount[i + 11] >= 1 && _handCount[i + 12] >= 1)
                {
                    if (_handCount[i + 20] >= 1 && _handCount[i + 21] >= 1 && _handCount[i + 22] >= 1)
                    {
                        pan += 2;
                        break;
                    }
                }
            }

        }
        if (pan > 0)
        {
            Debug.Log("삼색동각");
        }
        return pan;
    }
    //7. 산깡쯔
    public int Sankantsu()
    {
        if (_KkangCount == 3)
        {
            Debug.Log("산깡쯔");
            return 2;
        }
        else
        {
            return 0;
        }
    }
    //8 안커
    public int ankour()
    {
        int index = 0;
        int pan = 0;
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] == 3)
            {
                index++;
            }
        }
        if (index == 3)
        {
            pan += 2;
            Debug.Log("산안커");
            return pan;
        }
        if (index == 4)
        {
            pan += 14;
            Debug.Log("스안커");
            return 14;
        }
        else
        {
            return 0;
        }
    }
    //일기통관 계산
    public int Straight()
    {
        int pan = 0;
        if (_handCount[1] == 1 && _handCount[2] == 1 && _handCount[3] == 1 && _handCount[4] == 1 && _handCount[5] == 1 && _handCount[6] == 1 && _handCount[7] == 1
            && _handCount[8] == 1 && _handCount[9] == 1)
        {
            pan += 2;
        }
        if (_handCount[11] == 1 && _handCount[12] == 1 && _handCount[13] == 1 && _handCount[14] == 1 && _handCount[15] == 1 && _handCount[16] == 1 && _handCount[17] == 1
            && _handCount[18] == 1 && _handCount[19] == 1)
        {
            pan += 2;
        }
        if (_handCount[21] == 1 && _handCount[22] == 1 && _handCount[23] == 1 && _handCount[24] == 1 && _handCount[25] == 1 && _handCount[26] == 1 && _handCount[27] == 1
    && _handCount[28] == 1 && _handCount[29] == 1)
        {
            pan += 2;
        }
        if (pan > 0)
        {
            Debug.Log("일기통관");
        }
        return pan;
    }
    //혼일색 계산
    public int onecolor()
    {
        bool circle = false;
        bool crack = false;
        bool bamboo = false;
        bool zape = false;

        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 1 && i <= 9)
            {
                bamboo = true;
            }
            else if (_handCount[i] >= 1 && i <= 19)
            {
                circle = true;
            }
            else if (_handCount[i] >= 1 && i <= 29)
            {
                crack = true;
            }
            else if (_handCount[i] >= 1 && i <= 38)
            {
                zape = true;
            }
        }
        for (int i = 0; i < head.Length; i++)
        {
            if (head[i] >= 1 && i <= 9)
            {
                bamboo = true;
            }
            else if (head[i] >= 1 && i <= 19)
            {
                circle = true;
            }
            else if (head[i] >= 1 && i <= 29)
            {
                crack = true;
            }
            else if (head[i] >= 1 && i <= 38)
            {
                zape = true;
            }
        }
        if (zape)
        {
            if (circle && !crack && !bamboo)
            {
                Debug.Log("혼일색");
                return 3;
            }
            else if (!circle && crack && !bamboo)
            {
                Debug.Log("혼일색");
                return 3;
            }
            else if (!circle && !crack && bamboo)
            {
                Debug.Log("혼일색");
                return 3;
            }
        }
        return 0;
    }
    //청일색
    public int onlyonecolor()
    {
        bool circle = false;
        bool crack = false;
        bool bamboo = false;
        bool zape = false;

        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 1 && i <= 9)
            {
                bamboo = true;
            }
            else if (_handCount[i] >= 1&& i <= 19)
            {
                circle = true;
            }
            else if (_handCount[i] >= 1 && i <= 29)
            {
                crack = true;
            }
            else if (_handCount[i] >= 1 && i <= 38)
            {
                zape = true;
            }
        }
        for (int i = 0; i < head.Length; i++)
        {
            if (head[i] >= 1 && i <= 9)
            {
                bamboo = true;
            }
            else if (head[i] >= 1 && i <= 19)
            {
                circle = true;
            }
            else if (head[i] >= 1 && i <= 29)
            {
                crack = true;
            }
            else if (head[i] >= 1 && i <= 38)
            {
                zape = true;
            }
        }
        if (!zape)
        {
            if (circle && !crack && !bamboo)
            {
                Debug.Log("청일색");
                return 6;
            }
            else if (!circle && crack && !bamboo)
            {
                Debug.Log("청일색");
                return 6;
            }
            else if (!circle && !crack && bamboo)
            {
                Debug.Log("청일색");
                return 6;
            }
        }
        return 0;
    }
    //풍패 계산
    public int Direction()
    {
        int pan = 0;

        for (int i = 31; i < 35; i++)
        {
            //자풍 계산
            if (_handCount[i] == 3 && i == (_myDirection + 31))
            {
                pan += 1;
                Debug.Log("자풍");
            }
            //장풍 계산
            if (_handCount[i] == 3 && i == (_direction + 31))
            {
                pan += 1;
                Debug.Log("장풍");
            }
        }
        return pan;
    }
    //이페코 계산
    public int epeco()
    {
        int pan = 0;
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 2 && _handCount[i + 1] >= 2 && _handCount[i + 2] >= 2 && i < 30)
            {
                pan += 1;
            }
        }
        return pan;
    }
    //찬타 계산
    public int chanta1(int[] Hand, int index = 0)
    {
        var newMyHand = (int[])Hand.Clone();
        if (index < 4)
        {
            for (int i = 0; i < Hand.Length; i++)
            {
                if (i < 28 && Hand[i] >= 1 && Hand[i + 1] >= 1 && Hand[i + 2] >= 1)
                {
                    if (i % 9 == 1 || (i + 2) % 9 == 1
                        || (i - 10) % 9 == 1 || (i - 8) % 9 == 1
                        || (i - 20) % 9 == 1 || (i - 18) % 9 == 1)
                    {
                        newMyHand[i] -= 1;
                        newMyHand[i + 1] -= 1;
                        newMyHand[i + 2] -= 1;
                        index++;
                        return chanta1(newMyHand, index);
                    }
                }
                //커쯔 찾기
                if (Hand[i] == 3 && i < 40)
                {
                    if (i % 9 == 1
                    || (i - 10) % 9 == 1
                    || (i - 20) % 9 == 1 || i > 30 && newMyHand[i] >= 3)
                    {
                        newMyHand[i] -= 3;
                        index++;
                        return chanta1(newMyHand, index);
                    }
                }
            }
        }
        if (index >= 4)
        {
            int pair = 0;
            for (int i = 1; i < head.Length; i++)
            {
                if (i % 9 != 1 && i < 10 && head[i] >= 1)
                {
                    pair++;
                    break;
                }
                else if ((i - 10) % 9 != 1 && i < 20 && head[i] >= 1)
                {
                    pair++;
                    break;
                }
                else if ((i - 20) % 9 != 1 && i < 30 && head[i] >= 1)
                {
                    pair++;
                    break;
                }
            }
            if (pair == 0)
            {
                Debug.Log("찬타");
                return 2;
            }
            return 0;
        }
        return 0;
    }
    public int chanta2(int[] Hand, int index = 0)
    {
        if (index < 4)
        {
            var newMyHand = (int[])Hand.Clone();
            if (index < 4)
            {
                for (int i = 0; i < Hand.Length; i++)
                {
                    //커쯔 찾기
                    if (Hand[i] == 3 && i < 40)
                    {
                        if (i % 9 == 1
                        || (i - 10) % 9 == 1
                        || (i - 20) % 9 == 1 || i > 30 && newMyHand[i] >= 3)
                        {
                            newMyHand[i] -= 3;
                            index++;
                            return chanta2(newMyHand, index);
                        }
                    }

                    if (i < 28 && Hand[i] >= 1 && Hand[i + 1] >= 1 && Hand[i + 2] >= 1)
                    {
                        if (i % 9 == 1 || (i + 2) % 9 == 1
                            || (i - 10) % 9 == 1 || (i - 8) % 9 == 1
                            || (i - 20) % 9 == 1 || (i - 18) % 9 == 1)
                        {
                            newMyHand[i] -= 1;
                            newMyHand[i + 1] -= 1;
                            newMyHand[i + 2] -= 1;
                            index++;
                            return chanta2(newMyHand, index);
                        }
                    }
                }
            }
            if (index >= 4)
            {
                int pair = 0;
                for (int i = 1; i < head.Length; i++)
                {
                    if (i % 9 != 1 && i < 10 && head[i] >= 1)
                    {
                        pair++;
                        break;
                    }
                    else if ((i - 10) % 9 != 1 && i < 20 && head[i] >= 1)
                    {
                        pair++;
                        break;
                    }
                    else if ((i - 20) % 9 != 1 && i < 30 && head[i] >= 1)
                    {
                        pair++;
                        break;
                    }
                }
                if (pair == 0)
                {
                    Debug.Log("찬타");
                    return 2;
                }
                }
                return 0;
        }
        return 0;
    }
    public void GameStart()
    {
        for (int i = 0; i < _deck.Length; i++)
        {
            var tile = Instantiate(_tilePrefab);
            tile.transform.SetParent(_deckParent.transform);
            tile.GetComponent<Tile>().Index = i;
            _deck[i] = tile;
        }
        classification();
        SetRed();
        Suffle(_deck);
        setDeck();
        setFirst();
        _realHandSet();
        _SetCount();
        KkangExiest();
        discard.Clear();
        check1();
        check2();
        _gameStart = true;
    }
    

    private void Update()
    {
        if (_gameStart)
        {
            time += Time.deltaTime;
            if (!_reachStart)
            {
                //마우스 더블 클릭으로 핸드 버림패 하기
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Tile>()._isSelect == false && hit.transform.gameObject.layer == LayerMask.NameToLayer("Hand"))
                    {
                        for (int i = 0; i < _realHand.Length; i++)
                        {

                            _realHand[i].GetComponent<Tile>()._isSelect = false;

                        }
                        hit.transform.GetComponent<Tile>()._isSelect = true;

                    }
                    else if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Tile>()._isSelect == true && hit.transform.gameObject.layer == LayerMask.NameToLayer("Hand"))
                    {
                        Debug.Log(hit.transform.name + "버리기");
                        int number = Array.FindIndex(_realHand, i => i == hit.transform.gameObject);
                        Tusmo(number);
                        var item = _realHand[number].GetComponent<Tile>();
                        for (int i = 0; i < _realHand.Length; i++)
                        {
                            _realHand[i].GetComponent<Tile>()._isSelect = false;
                        }
                    }
                }
                if (_istenpai && ReachButtonSelected)
                {
                    //마우스 더블 클릭으로 핸드 버림패 하기
                    if (Input.GetMouseButtonDown(0))
                    {

                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Tile>()._isSelect == false && hit.transform.gameObject.layer == LayerMask.NameToLayer("Hand"))
                        {
                            for (int i = 0; i < _realHand.Length; i++)
                            {
                                _realHand[i].GetComponent<Tile>()._isSelect = false;
                            }
                            int number = Array.FindIndex(_realHand, i => i == hit.transform.gameObject);
                            hit.transform.GetComponent<Tile>()._isSelect = true;
                            var item = _realHand[number].GetComponent<Tile>();
                            int[] Made = (int[])forMade.Clone();
                            //메이드 가능 패에서 버림 패를 제외한다
                            for (int i = 0; i < forMade.Length; i++)
                            {
                                if (Made[i] == (item.Index + (int)item.type))
                                {
                                    Made[i] = 0;
                                }
                            }
                            //완성에 필요한 패를 로그상에 표시한다
                            for (int i = 0; i < Made.Length; i++)
                            {
                                if (Made[i] > 0)
                                {
                                    //완성 에 필요한 패 로그
                                    Debug.Log(i);
                                }
                            }
                        }
                        else if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Tile>()._isSelect == true && hit.transform.gameObject.layer == LayerMask.NameToLayer("Hand"))
                        {
                            Debug.Log(hit.transform.name + "버리기");
                            int number = Array.FindIndex(_realHand, i => i == hit.transform.gameObject);
                            for (int i = 0; i < _realHand.Length; i++)
                            {
                                _realHand[i].GetComponent<Tile>()._isSelect = false;
                            }
                            var item = _realHand[number].GetComponent<Tile>();
                            for (int i = 0; i < forMade.Length; i++)
                            {
                                if (forMade[i] == (item.Index + (int)item.type))
                                {
                                    forMade[i] = 0;
                                }
                            }
                            for (int i = 0; i < forMade.Length; i++)
                            {
                                if (forMade[i] > 0)
                                {
                                    Debug.Log("포메이드 : " + i);
                                }
                            }
                            Tusmo(number);
                            for (int i = 0; i < _realHand.Length; i++)
                            {
                                _realHand[i].transform.localScale = new Vector3(0.7f, 1f, 0.5f);
                            }
                            _reachStart = true;
                        }
                    }
                    //언두 기능
                    else if (Input.GetMouseButtonDown(1))
                    {
                        for (int i = 0; i < _realHand.Length; i++)
                        {
                            var item = _realHand[i].GetComponent<Tile>();
                            item.setImage((int)item.type + item.Index);
                            item.transform.gameObject.layer = LayerMask.NameToLayer("Hand");
                        }
                        _istenpai = false;
                        ReachButtonSelected = false;
                        _ReachButton.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (!_isMade && time > 0.5f)
                {
                    ReachTusmo();
                    checkMade();
                    time = 0f;
                }
                if (_isMade && time > 3f)
                {
                    Debug.Log(checkScore());
                    time -= 1000f;
                }
            }
        }
    }
    }
    