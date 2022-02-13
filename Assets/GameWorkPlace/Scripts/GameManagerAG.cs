 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManagerAG : NetworkBehaviour
{
    [SerializeField] public PlayerAG localPlayer;
    [SerializeField] public PlayerAG enemyPlayer;
    public bool GameStart;
    public UIManager UIM;
    public int enemyHiddenCards;
    public List<CardData> EnemyShownCards;
    public delegate void GameState();
    public GameState CurrentState;
    CardData playerCard;
    CardData enemyCard;
    public bool PlayerReady;
    public bool enemyReady;
    bool PickOpponentCardToReveal;
    bool BattleWon;
    bool CursorReset = false;
    int playerScore = 0;
    int enemyScore = 0;
    int RoundNumber = 0;
    bool PlayingAnimation;
    public bool ResetGame;
    public int roundsForWin = 7;
    public int RoundsTilDanger = 4;
    
    private void Start() {
        CurrentState = SetUp;
        
    }

    private void Update() {
        if(NetworkManager.singleton.numPlayers ==2 && enemyPlayer ==null){
            CmdFindPlayers();
            
        }

        if(GameStart){
            if(!PlayingAnimation)
            CurrentState();
        }



    }

    public void SetUp(){
        SoundManager.Instance.PlaySong("Battle Start");
        cmdUpdateNames(localPlayer.netIdentity, UIManager.Instance.inputField.text);
        if(localPlayer.Hand.HCData.Count==0) localPlayer.Hand.AddCards(8,8);

        if(localPlayer.Hand.HCData.Count>0 && UIManager.Instance.HiddenEnemyCardCount>0 ){
            CurrentState = SelectCards;
        }
        
        
    }
    public void DrawPhase(){
        Debug.Log("Loop reset");
        if(localPlayer.Hand.HCData.Count + localPlayer.Hand.SCData.Count<=4){
            localPlayer.Hand.AddCards(8,4);
        }
        if(localPlayer.Hand.HCData.Count + localPlayer.Hand.SCData.Count >=5){
            CurrentState = SelectCards;
        }
        
    }
    public void SelectCards(){
        if(localPlayer.Hand.HCData.Count == 0){
            CursorReset = true;
            RoundNumber++;
            UIManager.Instance.isFrontRow = true;

        }
        else if(!CursorReset && localPlayer.Hand.HCData.Count>0 && UIManager.Instance.HiddenEnemyCardCount > 0){
            UIManager.Instance.ResetCursors();
            CursorReset = true;
            

        }
        
        if(localPlayer.Vertical != 0 && localPlayer.Hand.SCData.Count!=0 && localPlayer.Hand.HCData.Count!=0){
            SoundManager.Instance.Play("Select");
            cmdMoveCards(localPlayer.netIdentity, true, 0, false);
            
        }
        if(localPlayer.Horizontal!=0){
            SoundManager.Instance.Play("Select");
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
            StartCoroutine(BattleCalculation());
            
            
        }
         
        
    }

    public void WaitForCombat(){
        if(PickOpponentCardToReveal&& UIManager.Instance.BattleUI.animationDone){
            UIManager.Instance.BattleUI.animationDone = false;
            CurrentState = PickEnemyCard;
        }
        else if(!PickOpponentCardToReveal&& UIManager.Instance.BattleUI.animationDone){
            UIManager.Instance.BattleUI.animationDone = false;
            Debug.Log("No picking for me.");
            cmdReadyPlayer(localPlayer.netIdentity);
            CurrentState = CheckIfWon;
        }
        

    }
    public void PickEnemyCard(){
        Debug.Log("Picking Time!");
        if(UIManager.Instance.HiddenEnemyCardCount<=1){
            cmdReadyPlayer(localPlayer.netIdentity);
            CurrentState = CheckIfWon;
            return;
        }
        if(localPlayer.Horizontal!=0){
            SoundManager.Instance.Play("Select");
            cmdMoveCards(localPlayer.netIdentity, false, localPlayer.Horizontal, true);
        }
        UIManager.Instance.HandleRevealConfirm();
        
    }
    public void CheckIfWon(){
        
        if(PlayerReady && enemyReady){
            StopAllCoroutines();
            PickOpponentCardToReveal = false;
            PlayerReady = false;
            enemyReady = false;
            
            UIManager.Instance.ResetCursors();
            UIManager.Instance.HideCursors();
            RoundNumber++;
            if(playerScore ==RoundsTilDanger || enemyScore == RoundsTilDanger){
                SoundManager.Instance.PlaySong("Last Round");
            }
            if(playerScore ==roundsForWin || enemyScore == roundsForWin){
                if (playerScore>enemyScore){
                    UIManager.Instance.ShowVictoryLoss(true);
                }
                else{
                    UIManager.Instance.ShowVictoryLoss(false);
                }
                CurrentState = WinScreen;
            }
            else CurrentState = DrawPhase;

        }
        
    }

    public void WinScreen(){
        
        

        if(ResetGame){
            ResetGame = false;
            playerScore = 0;
            enemyScore = 0;
            RoundNumber = 0;
            localPlayer.Hand.HCData = new List<CardData>();
            localPlayer.Hand.SCData = new List<CardData>();
            cmdUpdateScore(localPlayer.netIdentity, playerScore);
            CmdUpdateCards(localPlayer.Hand.HCData, localPlayer.Hand.SCData, localPlayer.gameObject);
            CurrentState = SetUp;


        }


    }
    

    IEnumerator BattleCalculation(){

        StartCoroutine(UIManager.Instance.BattleUI.DimScreen(0.5f));
        yield return  new WaitUntil(() => UIManager.Instance.BattleUI.animationDone);
        UIManager.Instance.BattleUI.animationDone = false;
        StartCoroutine(UIManager.Instance.BattleUI.BattleCards(playerCard, enemyCard));
        yield return new WaitUntil(() => UIManager.Instance.BattleUI.animationDone);
        UIManager.Instance.BattleUI.animationDone = false;
        
        
        if(enemyCard.type == (playerCard.type+1)%4){
            //if(playerCard.number != 0)
            playerCard.number +=2;
            Debug.Log("Advantage!");
            
        }
        if(enemyCard.type == (playerCard.type+2)%4){
            PickOpponentCardToReveal = true;
            Debug.Log("Reveal Cards");

        }
        if(enemyCard.type == (playerCard.type+3)%4){
            //if(enemyCard.number != 0)
            enemyCard.number +=2;
            Debug.Log("Disadvantage...");
        }
        Debug.Log(playerCard.number + " vs " + enemyCard.number);

        if(playerCard.number == 0 && enemyCard.number!=0){
        
            BattleWon = true;
            
        }
        else if(playerCard.number> enemyCard.number && enemyCard.number!=0){
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

        if(BattleWon) playerScore++;

        cmdUpdateScore(localPlayer.netIdentity, playerScore);

        

        CurrentState = WaitForCombat;
        

        



        

       
        

        
        Debug.Log("Result of Battle: "+BattleWon);
        Debug.Log(PickOpponentCardToReveal);


        
    }

    

    #region //Commands
    [Command (requiresAuthority = false)] public void cmdReadyPlayer(NetworkIdentity player){
        RpcReadyPlayer(player);
    }
    [ClientRpc] public void RpcReadyPlayer(NetworkIdentity player){
        if(player.isLocalPlayer){
            PlayerReady = true;
        }
        else{
            enemyReady = true;
        }

    }

    [Command (requiresAuthority = false)] public void cmdUpdateScore(NetworkIdentity player, int score){

        RpcUpdateScore(player, score);
    }

    [ClientRpc] public void RpcUpdateScore(NetworkIdentity player, int score){
        if(player.isLocalPlayer){
            playerScore = score;
            UIManager.Instance.UpdateScore(true, score);
        }
        else{
            enemyScore = score;
            UIManager.Instance.UpdateScore(false, score);
        } 
    }

    [Command (requiresAuthority = false)] public void cmdUpdateNames(NetworkIdentity player, string pname){

        RpcUpdateNames(player, pname);
    }

    [ClientRpc] public void RpcUpdateNames(NetworkIdentity player, string pname){
        if(player.isLocalPlayer){
            UIManager.Instance.UpdateName(true, pname);
        }
        else UIManager.Instance.UpdateName(false, pname);
    }
    [Command (requiresAuthority = false)] public void cmdCardRevealChoice(NetworkIdentity player, int slot){
        RpcCardRevealChoice(player, slot);
    }
    [ClientRpc] public void RpcCardRevealChoice(NetworkIdentity player, int slot){
        
        if(player.isLocalPlayer){
            PlayerReady = true;
            CurrentState = CheckIfWon;
            return;
        }
        else{
            enemyReady = true;
            if(PlayerReady && enemyReady){
                CurrentState = CheckIfWon;
            }
            localPlayer.Hand.RevealCard(slot);
            return;
            
            
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
                UIManager.Instance.ManageCursor(false,switchRows, direction);

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
