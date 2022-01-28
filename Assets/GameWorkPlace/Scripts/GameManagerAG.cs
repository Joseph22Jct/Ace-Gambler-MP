 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManagerAG : NetworkBehaviour
{
    [SerializeField] public PlayerAG localPlayer;
    [SerializeField] public PlayerAG enemyPlayer;
    public NetworkManager NM;
    public bool GameStart;
    public UIManager UIM;
    public int enemyHiddenCards;
    public List<CardData> EnemyShownCards;
    public delegate void GameState();
    public GameState CurrentState;
    CardData playerCard;
    CardData enemyCard;
    bool PlayerReady;
    bool enemyReady;
    bool PickOpponentCardToReveal;
    bool BattleWon;
    bool CursorReset = false;
    private void Start() {
        CurrentState = SetUp;
    }

    private void Update() {
        if(NetworkManager.singleton.numPlayers ==2 && enemyPlayer ==null){
            CmdFindPlayers();
            
        }

        if(GameStart){
            CurrentState();
        }



    }

    public void SetUp(){
        localPlayer.Hand.AddCards(7,7);
        CurrentState = SelectCards;
        
    }
    public void SelectCards(){
        if(!CursorReset && localPlayer.Hand.HCData.Count>0 && UIManager.Instance.HiddenEnemyCardCount > 0){
            UIManager.Instance.ResetCursors();
            CursorReset = true;

        }
        
        Debug.Log("Working");
        if(localPlayer.Vertical != 0 ){
            cmdMoveCards(localPlayer.netIdentity, true, 0, false);
            
        }
        if(localPlayer.Horizontal!=0){
            cmdMoveCards(localPlayer.netIdentity,false, Mathf.Sign(localPlayer.Horizontal), false);
        }
        
        UIManager.Instance.HandleConfirm();
        
    
        

    }
    public void InitiateCombat(){
        
        
        UIManager.Instance.HideCursors();
        CursorReset = false;
        if(PlayerReady && enemyReady){
            PlayerReady = false;
            enemyReady = false;
            BattleCalculation();
        }
        if(PickOpponentCardToReveal){
            CurrentState = PickEnemyCard;
        }
    }
    public void PickEnemyCard(){
        if(localPlayer.Horizontal!=0){
            cmdMoveCards(localPlayer.netIdentity, false, localPlayer.Horizontal, true);
        }
        UIManager.Instance.HandleRevealConfirm();
        
    }
    public void CheckIfWon(){
        PickOpponentCardToReveal = false;
        Debug.Log("Loop reset");
        UIManager.Instance.ResetCursors();
        UIManager.Instance.HideCursors();
        CurrentState = SelectCards;
    }
    

    void BattleCalculation(){
        
        if(enemyCard.type == (playerCard.type+1)%4){
            if(playerCard.number != 0)
            playerCard.number +=2;
            
        }
        if(enemyCard.type == (playerCard.type+2)%4){
            PickOpponentCardToReveal = true;

        }
        if(enemyCard.type == (playerCard.type+3)%4){
            if(enemyCard.number != 0)
            enemyCard.number +=2;
        }
        Debug.Log(playerCard.number + " vs " + enemyCard.number);

        if(playerCard.number> enemyCard.number && enemyCard.number!=0){
            BattleWon = true;
        }
        else{
            BattleWon = false;
            PickOpponentCardToReveal = true;
        }

        if(UIManager.Instance.isFrontRow){
            localPlayer.Hand.RemoveCard(UIManager.Instance.currentShownSlot, true);
        }
        else{
            
            localPlayer.Hand.RemoveCard(UIManager.Instance.currentHiddenSlot, false);
        }

        
        Debug.Log("Result of Battle: "+BattleWon);
        Debug.Log(PickOpponentCardToReveal);


        
    }

    #region //Commands

    [Command (requiresAuthority = false)] public void cmdCardRevealChoice(NetworkIdentity player, int slot){
        RpcCardRevealChoice(player, slot);
    }
    [ClientRpc] public void RpcCardRevealChoice(NetworkIdentity player, int slot){
        CurrentState = CheckIfWon;
        if(player.isLocalPlayer){
            return;
        }
        else{
            localPlayer.Hand.RevealCard(slot);
        }
        

    }
    [Command (requiresAuthority = false)] public void cmdCardChoiceReady(NetworkIdentity player, CardData card){
        rpcCardChoiceReady(player, card);
    }
    [ClientRpc] void rpcCardChoiceReady(NetworkIdentity player, CardData card){
        if (player.isLocalPlayer){
            CurrentState = InitiateCombat;
            playerCard = card;
            PlayerReady = true;

        }
        else{
            enemyCard = card;
            enemyReady = true;
        }
    }

    [Command (requiresAuthority = false)] void cmdMoveCards(NetworkIdentity player,bool switchRows, float direction,bool reveal){
        RpcMoveCards(player, switchRows, direction, reveal);
    }

    [ClientRpc] void RpcMoveCards(NetworkIdentity player, bool switchRows, float direction, bool reveal){
        if(!reveal){
            if(player.isLocalPlayer){
                UIManager.Instance.ManageCursor(true, switchRows,direction);
            }
            else{
                UIManager.Instance.ManageCursor(false,false, direction);

            }
        }
        else{
            if(player.isLocalPlayer){
                UIManager.Instance.MoveRevealCursor(true, direction );
            }
            else{
                UIManager.Instance.MoveRevealCursor(false, direction );
            }
        }
        

    }

    

    [Command (requiresAuthority = false)] public void CmdUpdateCards(List<CardData> HCCards, List<CardData> SCCards, GameObject player){
        RpcUpdateCards(HCCards, SCCards, player);
    }
    [ClientRpc] void RpcUpdateCards(List<CardData> HCCards, List<CardData> SCCards, GameObject player){
        if(player.GetComponent<NetworkIdentity>().isLocalPlayer){
            UIManager.Instance.UpdateCards();
            return;
        }
        else{
            enemyPlayer.Hand.HCData = HCCards;
            enemyPlayer.Hand.SCData = SCCards;
            UIManager.Instance.UpdateCards();
            
        }

        

    }

    [Command (requiresAuthority = false)] void CmdFindPlayers(){
        RcpFindPlayers();
    }

    [ClientRpc] void RcpFindPlayers(){
        PlayerAG[] players = FindObjectsOfType<PlayerAG>();
            if(players[0].isLocalPlayer){
                localPlayer = players[0];
                enemyPlayer = players[1];

            }
            else{
                localPlayer = players[1];
                enemyPlayer = players[0];
            }
            GameStart = true;
    }
    #endregion
}
