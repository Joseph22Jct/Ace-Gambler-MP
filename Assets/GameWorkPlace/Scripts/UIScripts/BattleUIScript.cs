using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUIScript : MonoBehaviour
{
    public Image dimScreen;
    public float howDim = 0.4f;

    
    public GameObject playerCardObject;
    public GameObject enemyCardObject;
    public bool animationDone;
    public GameObject Hint;

    
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
    public IEnumerator BattleCards(CardData playerCard, CardData enemyCard){

        //Show cards

        playerCardObject.transform.localPosition = new Vector3(-400,0,0);
        enemyCardObject.transform.localPosition = new Vector3(400,0,0);
        playerCardObject.GetComponent<UICard>().AddUICard(1,playerCard.number, playerCard.type, playerCard.revealed );
        enemyCardObject.GetComponent<UICard>().AddUICard(1,enemyCard.number, enemyCard.type, enemyCard.revealed );

        playerCardObject.transform.DOLocalMoveX(-150,0.5f, false).SetEase(Ease.OutCubic);
        enemyCardObject.transform.DOLocalMoveX(150,0.5f, false).SetEase(Ease.OutCubic);
        Hint.transform.DOLocalMoveY(0,0.5f, false).SetEase(Ease.OutCubic);
        

        yield return new WaitForSeconds(0.5f);
        
        //Show Bonuses

        if(enemyCard.type == (playerCard.type+1)%4){//Advantage
            UICard uicard = playerCardObject.GetComponent<UICard>();
            StartCoroutine(uicard.ShowIcon(0, 1));
            yield return new WaitUntil(()=> uicard.animationDone);
            uicard.animationDone = false;

            
            
            
        }
        if(enemyCard.type == (playerCard.type+2)%4){//Show Cards
            UICard uicard = playerCardObject.GetComponent<UICard>();
            StartCoroutine(uicard.ShowIcon(1, 1));
             UICard uicard2 = enemyCardObject.GetComponent<UICard>();
            StartCoroutine(uicard2.ShowIcon(1, 1));
            yield return new WaitUntil(()=> uicard.animationDone);
            uicard.animationDone = false;
            uicard2.animationDone = false;
            

        }
        if(enemyCard.type == (playerCard.type+3)%4){//Disadvantage
            UICard uicard = enemyCardObject.GetComponent<UICard>();
            StartCoroutine(uicard.ShowIcon(0, 1));
            yield return new WaitUntil(()=> uicard.animationDone);
            uicard.animationDone = false;
            
        }
        else{
            yield return new WaitForSeconds(2);
        }


        //Finish Combats

        playerCardObject.transform.DOLocalMoveX(-400,0.5f, false).SetEase(Ease.InCubic);
        enemyCardObject.transform.DOLocalMoveX(400,0.5f, false).SetEase(Ease.InCubic);
        Hint.transform.DOLocalMoveY(250,0.5f, false).SetEase(Ease.InCubic);


        StartCoroutine(unDimScreen());

        

        yield return new WaitForSeconds(0.5f);

        animationDone = true;
        StopAllCoroutines();

        
    }
}
