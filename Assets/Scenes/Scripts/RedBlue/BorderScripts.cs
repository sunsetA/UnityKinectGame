using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderScripts : MonoBehaviour
{

    private void Start()
    {
        this.GetComponent<BoxCollider2D>().size = new Vector2(Screen.width, this.GetComponent<BoxCollider2D>().size.y);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.GetComponent<InteractableObject>();
        if (obj!=null)
        {
            bool isDrag = obj.GetDragState();
            if (!isDrag)
            {
                obj.GetComponent<Image>().raycastTarget = false;
                obj.GetComponent<Collider2D>().enabled = false;
                obj.GetComponent<Image>().DOFade(0,1f);
            }
        }
    }
}
