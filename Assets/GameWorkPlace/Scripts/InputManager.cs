using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region singleton

    public static InputManager _instance;
    public static InputManager Instance{
        get{
            if(_instance == null){
                _instance = new InputManager();

            }
            return _instance;
        }
    }
    #endregion

    
}
