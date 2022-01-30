using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandManager : MonoBehaviour
{
    public List<CardData> HCData  = new List<CardData>();
    public List<CardData> SCData = new List<CardData>();

    public GameManagerAG GM;
    

    public void AddCards(int power, int count){
        int cardpower = power*count - count;
        int[] cardNumbers = new int[count-1];
        for(int i = 0; i<cardpower; i++){
            int rand = Random.Range(0,count-1);
            if(cardNumbers[rand]>=12){
                i--;
                return;
            }
            cardNumbers[rand]++;
        }
        int StartingType = Random.Range(0,4);
        CardData Ace = new CardData(0,StartingType, false);
        HCData.Add(Ace);

        for (int i = 0; i< count-1; i++){
            CardData data = new CardData(cardNumbers[i], (i+StartingType+1)%4,false );
            HCData.Add(data);
        }

        for(int i = 0; i<HCData.Count; i++){
            Debug.Log(HCData[i].number+", "+ HCData[i].type);
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
