using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CardData", order = 1)]
public class CardsDataSO : ScriptableObject
{
    public CardData[] cards;

    public CardData GetRandomCard()
    {
        return cards[Random.Range(0, cards.Length)];
    }
}

[Serializable]
public struct CardData
{
    public string cardName;
    public int cost;
    public int attack;
    public int hp;
    public string illustration;
    [TextArea]public string description;
}