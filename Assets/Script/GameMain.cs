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
    //�ڵ� Ȯ�� ui
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
    //�θ� ��ü
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
    //�ڵ� ī���� �迭
    private int[] _handCount = new int[41];
    //���� �ڵ�
    private GameObject[] _realHand = new GameObject[14];
    //���� Ÿ�� ����
    private GameObject[] _deck = new GameObject[136];
    private int _TusmoCount = 28;
    //������ ����
    private GameObject[] _removeDeck = new GameObject[136];
    private int _RemoveCount = 0;
    [SerializeField]
    private Image[] _doraSprite;
    //���� ǥ�� �� ����
    private GameObject[] _dora = new GameObject[4];
    private GameObject[] _ura = new GameObject[4];
    private int[] _doraCount = new int[41];
    //���� ǥ��
    private GameObject[] _deadwall = new GameObject[4];
    //�� ǥ�� ī��Ʈ
    private int _KkangCount = 0;
    private int[] KkangCountset = new int[41];
    [SerializeField]
    Button _KkangButton;
    private List<GameObject> _KkangSet = new List<GameObject>();
    //��ġ ����
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
    //������ Ȯ�� ����
    public bool _istenpai = false;
    //���� �μ�
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
    //�Ǽ� ������ ���� �̿�
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
    //�� �з�
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
    //�� ���� ����
    public void setDeck()
    {
        for (int i = 0; i < _deck.Length; i++)
        {
            _deck[i].transform.position = new Vector3(100, 0, 0);
        }
    }
    //ó�� �� ����
    public void setFirst()
    {
        _CountRefresh();
        //���� �� ����
        for (int i = 0; i < _deadwall.Length; i++)
        {
            _deadwall[i] = _deck[i];
        }
        //���� ����
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
    //�ڵ� ī���� �迭 �ʱ�ȭ
    public void _CountRefresh()
    {
        for (int i = 0; i < _handCount.Length; i++)
        {
            _handCount[i] = 0;
        }
    }
    //�ڵ� ī��Ʈ �迭�� ���� �ڵ� �� �ֱ�
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
    //���� �ڵ� ��ġ �� ui�� ǥ��
    public void _realHandSet(bool hand = true)
    {
        SelectSorting(_realHand, hand);
        for (int i = 0; i < _realHand.Length; i++)
        {
            _realHand[i].transform.position = new Vector3(i, -1, 0);
            //�ڵ� ���̾� ����
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
    //��� 1(�Ӹ�1, ����3)
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
                    Debug.Log("������");
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
            Debug.Log("������");
            _istenpai = true;
        }
    }
    ///��� 1(�Ӹ�1, ����3)
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
                //Ŀ�� ã��
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
                //Ŀ�� ã��
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
                }//���� ã��
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
                }//����� �� ���� ã��
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
                //Ŀ�� ã��
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
                }//���� ã��
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
    //�������� ������ ã��
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
                //Ŀ�� ã��
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
                }//���� ã��
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
    //�������� ������ ã��
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
                //Ŀ�� ã��
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
                }//���� ã��
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
    //��� 2 (����4)
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
                //Ŀ�� ã��
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
    //����
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
    //���
    public void Tusmo(int number = 13)
    {
        _removeDeck[_RemoveCount] = _realHand[number];
        _removeDeck[_RemoveCount].gameObject.layer = LayerMask.NameToLayer("Deck");
        _removeDeck[_RemoveCount].transform.position = new Vector3(110, 110, 0);
        _removeDeck[_RemoveCount].transform.SetParent(_removeParent.transform);
        _realHand[number] = _deck[_TusmoCount];
        //������ �и� �� ���������� ������Ų��
        var temp = _realHand[13];
        _realHand[13] = _realHand[number];
        _realHand[number] = temp;
        _RemoveCount++;
        _TusmoCount++;
        _realHandSet(false);
        _SetCount();
        KkangExiest();
        discard.Clear();
        //������ ���¸� üũ�Ѵ�
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
                //���� ã��
                if (i < 28 && Hand[i] >= 1 && Hand[i + 1] >= 1 && Hand[i + 2] >= 1)
                {
                    newMyHand[i] -= 1;
                    newMyHand[i + 1] -= 1;
                    newMyHand[i + 2] -= 1;
                    middleCount++;
                    return CheckBody1(newMyHand, middleCount);
                }
                //Ŀ�� ã��
                if (Hand[i] == 3 && i < 40)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckBody1(newMyHand, middleCount);
                }
            }
        } if (middleCount >= 4)
        {
            Debug.Log("���̵�");
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
                //Ŀ�� ã��
                if (Hand[i] == 3 && i < 40)
                {
                    newMyHand[i] -= 3;
                    middleCount++;
                    return CheckBody2(newMyHand, middleCount);
                }
                //���� ã��
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
            Debug.Log("���̵�");
            return 4;
        }
        return 0;
    }
    //������ ����
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
    //�� ġ��
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
    //�� ����
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
    //�� ���� �� ���󿡼� �и� �����´�.
    public void KkangTusmo(int number)
    {
        _realHand[number] = _deck[_KkangCount];

    }
   //��ġ ��ư�� ������ �� ��ġ ���·� ����
   //��ġ ���´� ���� ���¸� �����ϰ� ���̵� �� ������ �и� �����´�
    public void Reach()
    {
        _ReachButton.gameObject.SetActive(false);
        //�������� �� ����Ʈ�� �ߺ��� �����Ѵ�
        discard = discard.Distinct().ToList();
        int[] dis = discard.ToArray();
        //��ġ ������ �� ���� ���� �� ���� ��ȣ�ۿ��� �����Ѵ�.
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

        //���� �����ϴ� ���
        //���� ������ �������� ī��Ʈ �迭�� �߰��Ѵ�
        if (_KkangCount > 0)
        {
            for (int i = 0; i < _handCount.Length; i++)
            {
                _handCount[i] += KkangCountset[i];
            }
        }
        //����� ��� �� �迭�� �߰��Ѵ�
        for (int i = 0; i < _KkangCount + 1; i++)
        {
            var item = _dora[_KkangCount].GetComponent<Tile>();
            var item2 = _ura[_KkangCount].GetComponent<Tile>();

            int index = (int)item.type + item.Index;
            int index2 = (int)item2.type + item2.Index;
            item.setImage(index);
            item2.setImage(index2);
            //�� �׸��� ���� ��� ó���� ���� �ȴ�
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
            //��� �κе� �ٲ��ش�
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

        //���� ���
        for (int i = 0; i < _KkangCount + 1; i++)
        {

            if (_doraCount[i] >= 0)
            {
                pan += _doraCount[i] * _handCount[i];
                pan += _doraCount[i] * head[i];
            }
            Debug.Log("���� : " + pan);
        }
        Debug.Log("���� : " + pan);
        //1�� ���
        //���߿� ���
        pan += Tanyaochuu();
        Debug.Log("���߿� : " + pan);
        //���� ���
        pan += Pinfu();
        Debug.Log("���� : " + pan);
        //����� ���
        pan += threewon();
        Debug.Log("����� : " + pan);
        //ǳ�� ���
        pan += Direction();
        Debug.Log("ǳ�� : " + pan);
        //2�� ���
        //ȥ��� ���
        pan += honnodo();
        Debug.Log("ȥ��� : " + pan); 
        //�ϱ���� ���
        pan += Straight();
        Debug.Log("�ϱ���� : " + pan);
        //������� ���
        pan += threeColouerdStright();
        Debug.Log("������� : " + pan);
        //������� ���
        pan += threeColouerdTriple();
        Debug.Log("������� : " + pan);
        //����� ���
        pan += Sankantsu();
        Debug.Log("����� : " + pan);
        //��Ŀ ���
        pan += ankour();
        Debug.Log("��Ŀ : " + pan);
        //ȥ�ϻ� ���
        pan += onecolor();
        Debug.Log("ȥ�ϻ� : " + pan);
        //û�ϻ� ���
        pan += onlyonecolor();
        Debug.Log("û�ϻ� : " + pan);
        //������ ���
        pan += epeco();
        Debug.Log("������ : " + pan);
        //��Ÿ ���
        int canta1 = chanta1(_handCount);
        int canta2 = chanta2(_handCount);
        if(canta1 == 2 || canta2 == 2)
        {
            pan += 2;
            Debug.Log("��Ÿ : " + pan);
        }
        return pan;

    }
    //1. ���߿� ���

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
            Debug.Log("���߿�");
        return 1;
    }
    //2. ���� ���
    public int Pinfu()
    {
        int index = 0;
        int index2 = 0;
        if (_KkangCount > 0)
        {
            return 0;
        }
        //Ŀ�갡 �ִ��� ���
        for (int i = 0; i < _handCount.Length; i++)
        {
            if (_handCount[i] >= 3)
            {
                index++;
            }
        }
        //����а� ������� ���
        for (int i = 0; i < forMade.Length; i++)
        {
            if (forMade[i] > 0)
            {
                index2++;
            }
        }
        if (index == 0 && index2 == 2)
        {
            Debug.Log("����");
            return 1;
        }
        else
        {
            return 0;
        }
    }
    //3. ���� ���
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
        //�һ��� ���
        if (_handCount[36] == 3 && _handCount[37] == 3 && head[38] == 2)
        {
            Debug.Log("�һ���");
            pan += 2;
        } else if (_handCount[37] == 3 && _handCount[38] == 3 && head[36] == 2)
        {
            Debug.Log("�һ���");
            pan += 2;
        } else if (_handCount[38] == 3 && _handCount[36] == 3 && head[37] == 2)
        {
            Debug.Log("�һ���");
            pan += 2;
        }
        //���� ���
        if (_handCount[36] == 3 && _handCount[37] == 3 && _handCount[38] == 3)
        {
            Debug.Log("����");
            pan += 14;
        }
        return pan;
    }
    //4. ȥ��� ���
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
            Debug.Log("ȥ���");
            return 2;
        }
        return 0;
    }
    //5. �������
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
            Debug.Log("�������");
        }
        return pan;
    }
    //6. ������� 
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
            Debug.Log("�������");
        }
        return pan;
    }
    //7. �����
    public int Sankantsu()
    {
        if (_KkangCount == 3)
        {
            Debug.Log("�����");
            return 2;
        }
        else
        {
            return 0;
        }
    }
    //8 ��Ŀ
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
            Debug.Log("���Ŀ");
            return pan;
        }
        if (index == 4)
        {
            pan += 14;
            Debug.Log("����Ŀ");
            return 14;
        }
        else
        {
            return 0;
        }
    }
    //�ϱ���� ���
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
            Debug.Log("�ϱ����");
        }
        return pan;
    }
    //ȥ�ϻ� ���
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
                Debug.Log("ȥ�ϻ�");
                return 3;
            }
            else if (!circle && crack && !bamboo)
            {
                Debug.Log("ȥ�ϻ�");
                return 3;
            }
            else if (!circle && !crack && bamboo)
            {
                Debug.Log("ȥ�ϻ�");
                return 3;
            }
        }
        return 0;
    }
    //û�ϻ�
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
                Debug.Log("û�ϻ�");
                return 6;
            }
            else if (!circle && crack && !bamboo)
            {
                Debug.Log("û�ϻ�");
                return 6;
            }
            else if (!circle && !crack && bamboo)
            {
                Debug.Log("û�ϻ�");
                return 6;
            }
        }
        return 0;
    }
    //ǳ�� ���
    public int Direction()
    {
        int pan = 0;

        for (int i = 31; i < 35; i++)
        {
            //��ǳ ���
            if (_handCount[i] == 3 && i == (_myDirection + 31))
            {
                pan += 1;
                Debug.Log("��ǳ");
            }
            //��ǳ ���
            if (_handCount[i] == 3 && i == (_direction + 31))
            {
                pan += 1;
                Debug.Log("��ǳ");
            }
        }
        return pan;
    }
    //������ ���
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
    //��Ÿ ���
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
                //Ŀ�� ã��
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
                Debug.Log("��Ÿ");
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
                    //Ŀ�� ã��
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
                    Debug.Log("��Ÿ");
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
                //���콺 ���� Ŭ������ �ڵ� ������ �ϱ�
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
                        Debug.Log(hit.transform.name + "������");
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
                    //���콺 ���� Ŭ������ �ڵ� ������ �ϱ�
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
                            //���̵� ���� �п��� ���� �и� �����Ѵ�
                            for (int i = 0; i < forMade.Length; i++)
                            {
                                if (Made[i] == (item.Index + (int)item.type))
                                {
                                    Made[i] = 0;
                                }
                            }
                            //�ϼ��� �ʿ��� �и� �α׻� ǥ���Ѵ�
                            for (int i = 0; i < Made.Length; i++)
                            {
                                if (Made[i] > 0)
                                {
                                    //�ϼ� �� �ʿ��� �� �α�
                                    Debug.Log(i);
                                }
                            }
                        }
                        else if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Tile>()._isSelect == true && hit.transform.gameObject.layer == LayerMask.NameToLayer("Hand"))
                        {
                            Debug.Log(hit.transform.name + "������");
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
                                    Debug.Log("�����̵� : " + i);
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
                    //��� ���
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
    