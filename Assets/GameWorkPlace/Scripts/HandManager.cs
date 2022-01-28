using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<CardData> HCData  = new List<CardData>();
    public List<CardData> SCData = new List<CardData>();

    public GameManagerAG GM;
    

    public void AddCards(int power, int count){
        for (int i = 0; i< count; i++){
            CardData data = new CardData((int) Random.Range(0,13), (int) Random.Range(0,4),false );
            HCData.Add(data);
        }
        if(GM==null){
            GM = FindObjectOfType<GameManagerAG>();
        }
        GM.CmdUpdateCards(HCData, SCData, this.gameObject);


        
    }
    public void RemoveCard(int slot, bool isFront){
        if(isFront){
            SCData.RemoveAt(slot);
        }
        else{
            HCData.RemoveAt(slot);
        }

        GM.CmdUpdateCards(HCData, SCData, this.gameObject);
    }

    public void RevealCard(int slot){
        
        SCData.Add(new CardData(HCData[slot].number, HCData[slot].type, true));
        HCData.RemoveAt(slot);

        GM.CmdUpdateCards(HCData, SCData, this.gameObject);
    }   
}
