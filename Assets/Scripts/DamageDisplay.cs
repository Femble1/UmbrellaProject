using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField]
    float duration;
    [SerializeField]
    float position_end;
    Tweener tween;
    Tweener tween2;
    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void display_damage(string dmg_type, int dmg_to_display)
    {
        Text dmg = GetComponent<Text>();
        RectTransform position = GetComponent<RectTransform>();

        position.localScale = new Vector3(2, 2, 2);

        dmg.text = dmg_to_display.ToString();

        tween = position.DOAnchorPos(new Vector2(0, position_end), duration, true).OnComplete(()=>erase());
        tween2 = position.DOScale(new Vector3 (1, 1, 1), 0.3f);
    }

    void erase()
    {
        DOTween.Kill(tween);
        DOTween.Kill(tween2);
        Destroy(transform.parent.gameObject);
    }
}
