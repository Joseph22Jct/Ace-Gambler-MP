using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICard : MonoBehaviour
{

    public Sprite[] CardDesigns = new Sprite[10];
    public Sprite[] Numbers = new Sprite[13];
    public Sprite[] Types = new Sprite[4];
    int number;
    int type;
    bool revealed;
    public Image CardDesign;
    public Image Number;
    public Image Type;
    public Image BonusType;

    public Sprite[] iconImages;
    //public ParticleSystem PS;
    public bool animationDone = false;

    public IEnumerator ShowIcon(int iconType, float animationSpeed){
        BonusType.gameObject.SetActive(true);
        BonusType.color = new Vector4(1,1,1,0);
        yield return new WaitForSeconds(0.1f);

        if(iconType ==0){// +2
            BonusType.color = new Vector4(1,1,1,1);
            BonusType.sprite = iconImages[1];
            BonusType.transform.localScale = new Vector3(0.25f,0,1);
            BonusType.transform.DOScaleY(0.25f,1).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(0.2f/animationSpeed);
            //PS.Play();
            yield return new WaitForSeconds(2f/animationSpeed);
            float alpha = 1;
            while (alpha>0){
                alpha-=0.1f;
                BonusType.color = new Vector4(1,1,1,alpha);
                yield return new WaitForSeconds(0.03f/animationSpeed);

            }

            animationDone = true;
            
            yield return null;
            
        } 
        if(iconType ==1){ // Eyes
            int image = 2;
            BonusType.sprite = iconImages[image];

            float alpha = 0;
            while(alpha<1){
                BonusType.color = new Color(1,1,1,alpha);
                yield return new WaitForSeconds(0.1f/animationSpeed);
                alpha+=0.1f;
            }
            
            while(BonusType.sprite!= iconImages[5]){
                BonusType.sprite = iconImages[image];
                yield return new WaitForSeconds(0.1f/animationSpeed);
                image++;
            }
            yield return new WaitForSeconds(0.5f/animationSpeed);
            while (alpha>0){
                alpha-=0.1f;
                BonusType.color = new Vector4(1,1,1,alpha);
                yield return new WaitForSeconds(0.03f/animationSpeed);

            }
            animationDone = true;
            
            yield return null;
            
        }
    }
   
    public void AddUICard(int CD, int No, int Typ, bool Reveal){
        number = No;
        type = Typ;
        revealed = Reveal;
        this.gameObject.SetActive(true);
        if(!Reveal){
            CardDesign.sprite =CardDesigns[0];
            Number.gameObject.SetActive(false);
            Type.gameObject.SetActive(false);
        }
        else{
            CardDesign.sprite = CardDesigns[CD];
            Number.sprite = Numbers[No];
            Type.sprite = Types[Typ];
            Number.gameObject.SetActive(true);
            Type.gameObject.SetActive(true);
        }

    }

    

    public CardData GetCardData(){
        return new CardData(number, type, revealed);
    }
    public void MoveCard(){ //To revealed, or to combat area

    }

    
    public void DiscardCard(){
        this.gameObject.SetActive(false);
    }
}
