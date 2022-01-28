using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
