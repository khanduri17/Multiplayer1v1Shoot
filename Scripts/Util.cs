
using UnityEngine;

public class Util 
{
    public static void SetLayerRecursively(GameObject _obj , int _newlayer)
    {
        if (_obj == null)
        {
            return;
        }

        _obj.layer = _newlayer;

        foreach(Transform _child in _obj.transform)
        {
            if (_child == null) { continue; }

            SetLayerRecursively(_child.gameObject, _newlayer);
        }
    }
    
}
