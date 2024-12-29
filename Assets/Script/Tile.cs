using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.NetworkInformation;
public enum Type
{
    BAMBOO = 0,
    CIRCLE = 10,
    CRACK = 20,
    EAST = 31,
    SOUTH = 32,
    WEST = 33,
    NORTH = 34,
    WHITE = 36,
    SHOOT = 37,
    MIDDLE = 38,
    DUMMI = 40
}

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _text;
    [SerializeField]
    private SpriteRenderer _sprite;
    [SerializeField]
    private Sprite[] _sprites;
    //각 패의 타입

    private Type _type;
    public Type type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
        }
    }
    private int _index = 0;
    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
        }
    }
    private bool _isRed = false;
    public bool _IsRed
    {
        get
        {
            return _isRed;
        }
        set
        {
            _isRed = value;
        }
    }
    public bool _isSelect = false;
    public void setText()
    {
        _text.text = _type.ToString() + _index.ToString();
    }
    public void setTextClolr(Color color)
    {
        _text.color = color;
    }
    public void setName()
    {
        this.transform.name = _type.ToString() + _index.ToString();
    }
    public void setImage(int index)
    {
        _sprite.sprite = _sprites[index];
    }
    public Sprite getImage()
    {
        return _sprite.sprite;
    }
    public Tile(int index)
    {
        _index = index;
    }

}
