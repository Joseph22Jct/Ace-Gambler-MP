using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : MonoBehaviour
{
    #region singleton

    private static UIManager _instance;
    public static UIManager Instance{
        get{
            if(_instance == null){
                _instance = new UIManager();

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
    public GameObject CardPrefab;
    public HandManager PlayerHand;
    
    public List<CardData> enemyShownCards = new List<CardData>();

    public GameObject[] CardParents = new GameObject[4];
    public InputField inputField;
    public Text PlayerName;
    public Text EnemyName;

    List<UICard> PlayerCards = new List<UICard>();
    List<UICard> EnemyCards = new List<UICard>();
    List<UICard> PlayerRevealedCards = new List<UICard>();
    List<UICard> EnemyRevealedCards = new List<UICard>();
    public GameManagerAG GM;

    public GameObject PlayerCursor;
    public GameObject EnemyCursor;
    public GameObject HandCursor;

    public int oldHiddenSlot = 0;
    public int oldEnemySlot = 0;

    public int currentHiddenSlot = 0;
    public  int currentShownSlot = 0;
    public bool isFrontRow = false;
    public int enemySlot = 0;

    public int HiddenCardCount;
    public int ShownCardCount;
    public int HiddenEnemyCardCount;
    public int ShownEnemyCardCount;

    public CardData chosenCard;

    public void UpdateName(bool isPlayer, string pname){
        if(isPlayer){
            PlayerName.text = pname;
        }
        else EnemyName.text = pname;
    }

    

    

    public void UpdateCards(){

        
        
        MatchCards(GM.localPlayer.Hand.HCData, GM.localPlayer.Hand.SCData, GM.enemyPlayer.Hand.HCData.Count, GM.enemyPlayer.Hand.SCData);
    }


    // Start is called before the first frame update 
    public void MatchCards(List<CardData> PCards,List<CardData> PRCards,int ECAmount, List<CardData> ERCards ){

        
        HiddenCardCount = PCards.Count;
        ShownCardCount = PRCards.Count;
        HiddenEnemyCardCount = ECAmount;
        ShownEnemyCardCount = ERCards.Count;
        //Debug.Log(ECards[0].number);
        //TODO
        SpawnObjects(PCards, PRCards, ECAmount, ERCards);

        ShowCards(PCards, PRCards, ECAmount, ERCards);

        

        

        
    }

    public void HandleConfirm(){
        if(GM.localPlayer.Confirm){
            if(isFrontRow){
                GM.cmdCardChoiceReady ( GM.localPlayer.netIdentity,  PlayerRevealedCards[currentShownSlot].GetCardData());
            }
            else{
                GM.cmdCardChoiceReady ( GM.localPlayer.netIdentity,  PlayerCards[currentHiddenSlot].GetCardData());
            }
        }
    }
    public void HandleRevealConfirm(){
        if(GM.localPlayer.Confirm){
            
            GM.cmdCardRevealChoice( GM.localPlayer.netIdentity,  enemySlot);
            
        }
    }

    public void ResetSelection(){
        currentHiddenSlot = 0;
        currentShownSlot = 0;
        enemySlot = 0;
    }

    

    public void ShowCards(List<CardData> PCards,List<CardData> PRCards,int ECards,List<CardData> ERCards){
        for(int i=0; i<PlayerCards.Count; i++){
            if(i>= PCards.Count){
                PlayerCards[i].DiscardCard();
            }
            else{
                PlayerCards[i].gameObject.SetActive(true);
                PlayerCards[i].AddUICard(1, PCards[i].number, PCards[i].type, true);
            }
        }
        for(int i=0; i<PlayerRevealedCards.Count; i++){
            if(i>= PRCards.Count){
                PlayerRevealedCards[i].DiscardCard();
            }
            else{
                PlayerRevealedCards[i].gameObject.SetActive(true);
                PlayerRevealedCards[i].AddUICard(1, PRCards[i].number, PRCards[i].type, true);
            }
        }
        for(int i=0; i<EnemyCards.Count; i++){
            if(i>= ECards){
                EnemyCards[i].DiscardCard();
            }
            else{
                EnemyCards[i].gameObject.SetActive(true);
                EnemyCards[i].AddUICard(0,0, 0, false);
            }
        }
        for(int i=0; i<EnemyRevealedCards.Count; i++){
            if(i>= ERCards.Count){
                EnemyRevealedCards[i].DiscardCard();
            }
            else{
                EnemyRevealedCards[i].gameObject.SetActive(true);
                EnemyRevealedCards[i].AddUICard(1, ERCards[i].number, ERCards[i].type, true);
            }
        }
    }

    public void SpawnObjects(List<CardData> PCards,List<CardData> PRCards,int ECards,List<CardData> ERCards){
        if(PCards.Count > PlayerCards.Count){
            for (int i = PlayerCards.Count; i<PCards.Count;i++){
                GameObject go = Instantiate(CardPrefab);
                PlayerCards.Add(go.GetComponent<UICard>());
                go.transform.SetParent(CardParents[0].transform);
            }
        }
        if(PRCards.Count > PlayerRevealedCards.Count){
            for (int i = PlayerRevealedCards.Count; i<PRCards.Count;i++){
                GameObject go = Instantiate(CardPrefab);
                PlayerRevealedCards.Add(go.GetComponent<UICard>());
                go.transform.SetParent(CardParents[1].transform);
            }
        }
        if(ECards > EnemyCards.Count){
            for (int i = EnemyCards.Count; i<ECards;i++){
                GameObject go = Instantiate(CardPrefab);
                EnemyCards.Add(go.GetComponent<UICard>());
                go.transform.SetParent(CardParents[2].transform);
            }
        }
        if(ERCards.Count > EnemyRevealedCards.Count){
            for (int i = EnemyRevealedCards.Count; i<ERCards.Count;i++){
                GameObject go = Instantiate(CardPrefab);
                EnemyRevealedCards.Add(go.GetComponent<UICard>());
                go.transform.SetParent(CardParents[3].transform);
            }
        }
    }

    public void ManageCursor(bool isPlayer, bool changeRow, float slotChange){

        if(isPlayer){
            if(changeRow && GM.localPlayer.Hand.SCData.Count>0){
                isFrontRow = !isFrontRow;
            }
            if(slotChange>0 && !isFrontRow){
                currentHiddenSlot++;
                currentHiddenSlot%=HiddenCardCount;
            }
            else if(slotChange>0 && isFrontRow){
                currentShownSlot++;
                currentShownSlot%= ShownCardCount;
            }
            else if(slotChange<0 && !isFrontRow){
                currentHiddenSlot--;
                if(currentHiddenSlot<0) currentHiddenSlot = HiddenCardCount-1;
            }
            else if(slotChange<0&& isFrontRow) {
                currentShownSlot--;
                if(currentShownSlot<0) currentShownSlot = ShownCardCount-1;
            }
            MoveCursor(true);
            
        }
        else{
            if(slotChange > 0){
                enemySlot ++;
                enemySlot%=HiddenEnemyCardCount;
            }
            else{
                enemySlot--;
                if(enemySlot<0) enemySlot = HiddenEnemyCardCount-1;
            }
            MoveCursor(false);

        }

    }

    public void ResetCursors(){
        currentHiddenSlot = 0;
        currentShownSlot = 0;
        enemySlot = 0;
        isFrontRow = false;
        MoveCursor(true);
        MoveCursor(false);
    }
    public void HideCursors(){
        PlayerCursor.SetActive(false);
        HandCursor.SetActive(false);
        EnemyCursor.SetActive(false);
    }

    public void MoveRevealCursor(bool isPlayer, float dir){

        if(isPlayer){
            enemySlot-= (int) Mathf.Sign(dir);
            if(enemySlot<0) enemySlot = HiddenEnemyCardCount-1;
            enemySlot%=HiddenEnemyCardCount;
            
            
            HandCursor.SetActive(true);
            PlayerCursor.SetActive(true);
            //EnemyCards[enemySlot].gameObject.transform.localPosition = new Vector3(0,0,0);
            oldEnemySlot = enemySlot;
            //EnemyCards[enemySlot].gameObject.transform.position+=new Vector3(0,10,0);
            HandCursor.transform.localScale = new Vector3(-1,-1,1);
            
            HandCursor.transform.position = EnemyCards[enemySlot].gameObject.transform.position;
            PlayerCursor.transform.position = EnemyCards[enemySlot].gameObject.transform.position;
                
                    
                            

        }
        else{
            currentHiddenSlot+= (int) Mathf.Sign(dir);
            EnemyCursor.SetActive(true);
            //PlayerCards[oldHiddenSlot].transform.localPosition = new Vector3(0,0,0);
            oldHiddenSlot = currentHiddenSlot;
            //PlayerCards[currentHiddenSlot].transform.localPosition+=new Vector3(0,10,0);
            HandCursor.transform.position = EnemyCards[enemySlot].transform.position;
            PlayerCursor.transform.position = EnemyCards[enemySlot].transform.position;
            
        }

    }

    public void MoveCursor(bool isPlayer){ //Handles all cursor movement.
        if(isPlayer){
            if(isFrontRow){
                
                HandCursor.SetActive(true);
                HandCursor.transform.position = PlayerRevealedCards[currentShownSlot].transform.position;
                
            }
            else{
                HandCursor.transform.localScale = new Vector3(1,1,1);
                HandCursor.SetActive(true);
                PlayerCursor.SetActive(true);
                //PlayerCards[oldHiddenSlot].transform.localPosition = new Vector3(0,0,0);
                oldHiddenSlot = currentHiddenSlot;
                //PlayerCards[currentHiddenSlot].transform.localPosition+=new Vector3(0,10,0);
                HandCursor.transform.position = PlayerCards[currentHiddenSlot].transform.position;
                PlayerCursor.transform.position = PlayerCards[currentHiddenSlot].transform.position;
                    
                
            }

        }
        else{
            if(isFrontRow){
                return;
            }
            else{

                
                EnemyCursor.SetActive(true);
                //EnemyCards[enemySlot].gameObject.transform.localPosition = new Vector3(0,0,0);
                oldEnemySlot = enemySlot;
                //EnemyCards[enemySlot].gameObject.transform.position+=new Vector3(0,10,0);
                EnemyCursor.transform.position = EnemyCards[enemySlot].gameObject.transform.position;
                
            }
        }


    }
}
