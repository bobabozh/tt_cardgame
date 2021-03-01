using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private Card _cardPrefab;
    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _grave;
    [SerializeField] private Transform _table;
    [SerializeField] private Collider2D _tableCollider;
    [Space] [SerializeField] private int _nCards = 5;

    [SerializeField] private float _angleStep = 10;
    [SerializeField] private Vector2 _positionStep = new Vector2(1, 1);
    [Space] [SerializeField] private Button _gameButton;
    [Space] [SerializeField] private Button _restartButton;

    [Space] [SerializeField] private CardsDataSO _cardsData;

    private List<Card> _cards = new List<Card>();
    private List<Card> _cardsInHand = new List<Card>();
    private List<Card> _cardsOnTable = new List<Card>();

    private Vector2 _deckPosition = new Vector2(-5, -10);

    private Coroutine _regroupCardsOnTableCoroutine;
    private Coroutine _regroupCardsInHandCoroutine;

    private int _currentCard;

    private Card _draggedCard;

    private void Start()
    {
        Init();
        StartGame();
    }

    public void Init()
    {
        _gameButton.onClick.AddListener(OnRedButtonClick);
        _restartButton.onClick.AddListener(RestartGame);
    }

    public void StartGame()
    {
        _nCards = Random.Range(4, 7);
        
        CreateCards();
        
        RegroupCards();
    }

    private void RestartGame()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            Destroy(_cards[i].gameObject);
        }
        
        _cards.Clear();
        
        _cardsInHand.Clear();
        _cardsOnTable.Clear();
        
        StartGame();
    }

    private void OnRedButtonClick()
    {
        int param = Random.Range(0, 3);
        Card card = _cards[_currentCard];
        int value = Random.Range(-2, 9);

        if (param == 0)
            card.SetCost(value);
        else if (param == 1)
            card.SetAttack(value);
        else if (param == 2)
            card.SetHP(value);

        _currentCard++;
        if (_currentCard >= _cards.Count)
            _currentCard = 0;
    }

    private void CreateCards()
    {
        for (int i = 0; i < _nCards; i++)
        {
            Card card = Instantiate(_cardPrefab, _hand); 
            _cards.Add(card);
            _cardsInHand.Add(card);

            card.Init(_cardsData.GetRandomCard());
            card.transform.position = _deckPosition;
            card.isInHand = true;
            
            card.onCardDrag += OnCardDrag;
            card.onCardDrop += OnCardDrop;
            card.onCardRemove += OnCardRemove;
        }
    }

    private void OnCardRemove(Card card)
    {
        card.SetPosition(_grave.position);
        card.SetRotation(0);

        if (_cardsInHand.Contains(card))
            _cardsInHand.Remove(card);

        if (_cardsOnTable.Contains(card))
            _cardsOnTable.Remove(card);
        
        RegroupCards();
    }

    private void OnCardDrag(Card card)
    {
        if (_draggedCard == null)
        {
            if (_cardsInHand.Contains(card))
                _cardsInHand.Remove(card);

            if (_cardsOnTable.Contains(card))
                _cardsOnTable.Remove(card);
            
            _draggedCard = card;
            
            RegroupCards();
        }else if (_draggedCard != card)
            return;
        
        card.DragTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void OnCardDrop(Card card)
    {
        if (_draggedCard == card)
        {
            if (_tableCollider.IsTouching(card.collider))
            {
                card.isInHand = false;
                _cardsOnTable.Add(card);
                print(true);
            }
            else
            {
                card.isInHand = true;
                _cardsInHand.Add(card);
                print(false);
            }
            
            RegroupCards();
                
            _draggedCard = null;
        }
    }
    
    private void RegroupCards()
    {
        if (_regroupCardsInHandCoroutine != null)
            StopCoroutine(_regroupCardsInHandCoroutine);

        _regroupCardsInHandCoroutine = StartCoroutine(RegroupCardsInHandCoroutine());
        
        if (_regroupCardsOnTableCoroutine != null)
            StopCoroutine(_regroupCardsOnTableCoroutine);

        _regroupCardsOnTableCoroutine = StartCoroutine(RegroupCardsOnTableCoroutine());
    }

    private IEnumerator RegroupCardsOnTableCoroutine()
    {
        for (int i = 0; i < _cardsOnTable.Count; i++)
        {
            float cardX = (i - ((float)_cardsOnTable.Count - 2) / 2) * _positionStep.x;
            float cardY = 0;
                
            Vector2 position = _table.position + new Vector3(cardX, cardY);

            _cardsOnTable[i].SetRotation(0);
            _cardsOnTable[i].SetPosition(position, true, true);
            
            _cardsOnTable[i].SetSortingOrder(i);

            yield return new WaitForSeconds(0.2f);
        }
    }
    
    private IEnumerator RegroupCardsInHandCoroutine()
    {
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
                float rotation = (((float)_cardsInHand.Count - 1) / 2 - i) * _angleStep;

                float cardX = (i - ((float)_cardsInHand.Count - 1) / 2) * _positionStep.x;
                float cardY = - Mathf.Abs(((float)_cardsInHand.Count-1) / 2 - i)* _positionStep.y;

                Vector2 position = _hand.position + new Vector3(cardX, cardY);

                _cardsInHand[i].SetRotation(rotation);
                _cardsInHand[i].SetPosition(position, true, true);
            
                _cardsInHand[i].SetSortingOrder(i);

                yield return new WaitForSeconds(0.2f);
        }
    }
}