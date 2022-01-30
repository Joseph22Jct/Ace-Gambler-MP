using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WinLossScreen : MonoBehaviour
{
    public Image Victory;
    public Image Defeat;
    
    public Image RetryOptions;
    public Image Cursor;
    public GameManagerAG GM;
    public Image dimScreen;
    float howDim = 0.5f;
    bool AnimDone;
    bool victoryCheck;
    bool animationDone;
    
    private void OnEnable() {
        GM = FindObjectOfType<GameManagerAG>();
        
    
    }

    public IEnumerator DimScreen(float howLong){
        float count = 0;
        while(count<howLong){
            yield return new WaitForSeconds(0.05f);
            count+=0.05f;
            dimScreen.color = new Color(0,0,0,(count/howLong)*howDim ) ;

        }
        count = 0;
        animationDone = true;
        
        
    }
    public IEnumerator unDimScreen(){
        float count = dimScreen.color.a;
        while(count>0){
            yield return new WaitForSeconds(0.05f);
            count-=0.05f;
            dimScreen.color = new Color(0,0,0,count ) ;

        }
        count = 0;
        animationDone = true;
        
        
    }
    

    public IEnumerator ShowVictoryLoss(bool isVictory){
        victoryCheck = isVictory;
        
        StartCoroutine(DimScreen(0.5f));
        yield return new WaitUntil(() => animationDone);
        if(victoryCheck){
            SoundManager.Instance.PlaySong("Win");
            Victory.transform.DOLocalMoveY(0,0.4f).SetEase(Ease.InOutSine);
        } 
        else{
            SoundManager.Instance.PlaySong("Lose");
            Defeat.transform.DOLocalMoveY(0,0.4f).SetEase(Ease.InOutSine);
        }
        
        animationDone = false;
        AnimDone = true;

        
    }

    public IEnumerator ContinueGame(){
        AnimDone = false;
        GM.PlayerReady = false;
        GM.enemyReady = false;

        StartCoroutine(unDimScreen());
        if(victoryCheck){
            Victory.transform.DOLocalMoveY(300,0.4f).SetEase(Ease.InOutSine);
        } 
        else{
            Defeat.transform.DOLocalMoveY(300,0.4f).SetEase(Ease.InOutSine);
        }
        yield return new WaitForSeconds(0.4f);
        GM.ResetGame = true;
        StopAllCoroutines();
        gameObject.SetActive(false);
        
    }
    

    private void Update() {
        if(AnimDone){
            if(GM.localPlayer.Horizontal!=0){

        }
            if(GM.localPlayer.Confirm){
                GM.cmdReadyPlayer(GM.localPlayer.netIdentity);
 
            }
            if(GM.PlayerReady && GM.enemyReady){
                StartCoroutine(ContinueGame());
            }
        }
        
        
    }
}
