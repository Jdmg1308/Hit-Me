using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum CardType {Attack, Huma, Mani, Nihtee, Heal, Utility };
public enum CardType {Multiplier, Additor, EnemyBuff, PlayerBuff, Heal, 
                            StatusEffect, EnemyDebuff, PlayerDebuff};

public enum ColorType
{
    Blue, Red, Green
};

public abstract class Card : ScriptableObject 
{
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public Sprite effectImage;
    public abstract CardType cardType{get;}
    //public abstract CardClass cardClass { get; }
    public int effectValue;
    public int id;
    public string cardUserAnim;
    public ColorType color;
    public float price;
    public int points;

    public abstract void use(GameManager GM);

    //public abstract void use(Player p, Enemy e);
    //public abstract void use(Player p);

}
