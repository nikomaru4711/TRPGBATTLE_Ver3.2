using UnityEngine;

public class ObjectInfo
{
    public GameObject _gameObject;
    public int _dex;
    public int _id;
    public ObjectInfo(GameObject gameObject, int dex, int id)
    {
        _gameObject = gameObject;
        _dex = dex;
        _id = id;
    }
}
