using DG.Tweening;
using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    
    private int _value;
    private int _animatedValue;

    private Tween _valueTween;

    public void SetValue(int value)
    {
        _value = value;
        AnimateValue(_value);
    }

    public int ModifyValue(int value)
    {
        int v = _value + value;
        SetValue(v);
        return v;
    }

    private void AnimateValue(int value)
    {
        if(_valueTween!=null && _valueTween.active)
            _valueTween.Kill();
        
        _valueTween = ﻿﻿﻿﻿﻿﻿﻿DOTween.To(()=> _animatedValue, x=> _animatedValue = x, value, 1);
        _valueTween.onUpdate += OnAnimatedValueUpdate;
    }

    private void OnAnimatedValueUpdate()
    {
        _text.text = _animatedValue.ToString();
    }
}
