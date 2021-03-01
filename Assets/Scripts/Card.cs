using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [SerializeField] private TextMeshPro _titleText;
    [SerializeField] private TextMeshPro _descriptionText;
    [SerializeField] private SpriteRenderer _illustration;
    [SerializeField] private Counter _hpCounter;
    [SerializeField] private Counter _attackCounter;
    [SerializeField] private Counter _costCounter;

    [SerializeField] private SpriteRenderer _glow;

    private Tween _rotationTween;
    private Tween _positionXTween;
    private Tween _positionYTween;

    public bool isDragged;
    public bool isUp;
    public bool isInHand;
    private bool _ignoreEvents;
    private bool _isDead;

    private BoxCollider2D _coll;
    public BoxCollider2D collider => _coll;
    
    private SortingGroup _so;
    private Tween _glowTween;

    public delegate void CardDelegate(Card card);

    public CardDelegate onCardDrag;
    public CardDelegate onCardDrop;
    public CardDelegate onCardRemove;

    private Vector2 _homePosition;
    private float _homeRotation;
    
    private int _homeSortingOrder;
    
    public void Init()
    {
        _coll = GetComponent<BoxCollider2D>();
        _so = GetComponent<SortingGroup>();
    }

    public void Init(CardData data)
    {
        Init();
        
        SetName(data.cardName);
        SetCost(data.cost);
        SetAttack(data.attack);
        SetHP(data.hp);
        SetIllustration(data.illustration);
        SetDescription(data.description);
    }

    private void OnMouseEnter()
    {
        if (_ignoreEvents || isUp || isDragged)
            return;

        isUp = true;
        
        float dy = 1;
        
        SetGlowing(true);

        if (isInHand)
        {
            Rotate(0, 0.2f);
            MoveTo(_homePosition+ new Vector2(0, dy), 0.2f, false);
        }

        SetSortingOrder(30, false);
    }

    private void OnMouseExit()
    {
        if (_ignoreEvents || !isUp || isDragged)
            return;

        isUp = false;
        
        SetGlowing(false);
        
        Rotate(_homeRotation, 0.2f);
        MoveTo(_homePosition, 0.2f, false);
        
        SetSortingOrder(_homeSortingOrder, false);
    }

    private void OnMouseDown()
    {
        if (_ignoreEvents)
            return;

        isDragged = true;
        
        onCardDrag?.Invoke(this);
    }

    private void OnMouseUp()
    {
        if (_ignoreEvents)
            return;
        
        isDragged = false;
        
        SetGlowing(false);

        _ignoreEvents = true;
        
        onCardDrop?.Invoke(this);
    }

    private void OnMouseDrag()
    {
        if (_ignoreEvents && !isDragged)
            return;
        
        onCardDrag?.Invoke(this);
    }

    private void SetGlowing(bool value)
    {
        if(_glowTween!=null && _glowTween.active)
            _glowTween.Kill();

        _glowTween = _glow.DOFade(value?1:0, 0.2f);
    }

    public void SetRotation(float value, bool setAsHome = true)
    {
        Rotate(value, 1);

        if(setAsHome)
            _homeRotation = value;
    }

    private void Rotate(float value, float duration)
    {
        if(_rotationTween!=null && _rotationTween.active)
            _rotationTween.Kill();

        _rotationTween = transform.DORotate(new Vector3(0,0, value), duration);
    }

    public void SetPosition(Vector2 value, bool setAsHome = true, bool ignoreEventsOnMove = false)
    {
        MoveTo(value, 1, ignoreEventsOnMove);

        if(setAsHome)
            _homePosition = value;
    }

    public void DragTo(Vector2 pos)
    {
        MoveTo(pos, 0.1f, false);
    }

    private void MoveTo(Vector2 position, float duration, bool ignoreEventsOnMove)
    {
        if(_positionXTween!=null && _positionXTween.active)
            _positionXTween.Kill();
        
        if(_positionYTween!=null && _positionYTween.active)
            _positionYTween.Kill();
        
        _positionXTween = transform.DOMoveX(position.x, duration);
        _positionYTween = transform.DOMoveY(position.y, duration);

        if (!_isDead)
        {
            if(ignoreEventsOnMove)
                _positionXTween.OnComplete(OnMoveComplete);
        }
    }

    private void OnMoveComplete()
    {
        _ignoreEvents = false;
    }

    public void SetHP(int value)
    {
        _hpCounter.SetValue(value);

        if (value <= 0)
        {
            KillAllTweens();
            _ignoreEvents = true;
            _isDead = true;
            onCardRemove.Invoke(this);
        }
    }

    public void SetCost(int value)
    {
        _costCounter.SetValue(value);
    }

    public void SetAttack(int value)
    {
        _attackCounter.SetValue(value);
    }

    public void SetName(string value)
    {
        _titleText.text = value;
    }

    public void SetDescription(string value)
    {
        _descriptionText.text = value;
    }

    public void SetIllustration(string url)
    {
        StartCoroutine(LoadIllustrationCoroutine(url));
    }

    public void SetSortingOrder(int value, bool setAsHome = true)
    {
        _so.sortingOrder = value;

        Vector3 pos = transform.position;
        pos.z = -value;
        transform.position = pos;

        if (setAsHome)
            _homeSortingOrder = value;
    }
    
    IEnumerator LoadIllustrationCoroutine(string url) {
        Texture2D texture = new Texture2D(200, 200);
        Sprite sprite = Sprite.Create(texture, new Rect(0,0,200,200), new Vector2(0.5f,0.5f));

        _illustration.sprite = sprite;

        WWW www = new WWW(url);
        yield return www;

        www.LoadImageIntoTexture(texture);
        www.Dispose();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if(_positionXTween!=null && _positionXTween.active)
            _positionXTween.Kill();
        
        if(_positionYTween!=null && _positionYTween.active)
            _positionYTween.Kill();
        
        if(_rotationTween!=null && _rotationTween.active)
            _rotationTween.Kill();
    }
}
