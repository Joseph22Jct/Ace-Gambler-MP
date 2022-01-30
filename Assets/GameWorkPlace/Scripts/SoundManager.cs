using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region singleton

    private static SoundManager _instance;
    public static SoundManager Instance{
        get{
            if(_instance == null){
                _instance = new SoundManager();

            }
            return _instance;
        }
    }
    #endregion
    private void Awake() {
         if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
    }

    
}
