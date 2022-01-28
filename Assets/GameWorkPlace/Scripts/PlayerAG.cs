using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAG : NetworkBehaviour
{
     GameManagerAG GM;
    public HandManager Hand;
    
    public override void OnStartServer(){
        GM = FindObjectOfType<GameManagerAG>();
        Hand.GM = GM;
        //GM.netIdentity.AssignClientAuthority(connectionToClient);
        
    }
    public float Horizontal;

    public float Vertical;
    public bool Confirm;
    public bool Cancel;
    public float horizontalCD;
    public float VerticalCD;
    public float maxCD = 0.2f;

    public void HandleHorizontals(){
        if(Input.GetAxis("Horizontal")!=0 && horizontalCD<=0){
            horizontalCD = maxCD;
            Horizontal = Input.GetAxis("Horizontal");

        }
        else {
            Horizontal = 0;
            horizontalCD-=Time.deltaTime;
        }
    }
    public void HandleVerticals(){
        if(Input.GetAxis("Vertical")!=0 && VerticalCD<=0){
            VerticalCD = maxCD;
            Vertical = Input.GetAxis("Vertical");

        }
        else {
            Vertical = 0;
            VerticalCD-=Time.deltaTime;
        }
    }

    private void Update() {
        if(isLocalPlayer){
            HandleHorizontals();
            HandleVerticals();
            Confirm = Input.GetButtonDown("Confirm");
            Cancel = Input.GetButtonDown("Cancel");
        }
        
    }
}
