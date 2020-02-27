using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar 
{
    static Sprite onePixelSprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    Transform bar, frontground, background;
    BarControl barControl;

    public Bar(Transform parent)
    {
        bar = new GameObject("bar", typeof(BarControl)).transform;
        frontground = new GameObject("forntground", typeof(SpriteRenderer)).transform;
        background = new GameObject("background", typeof(SpriteRenderer)).transform;
        SetBarSprite(frontground, Color.green);
        SetBarSprite(background, Color.gray);
        SetBarTranfrom(frontground, 80, 8);
        SetBarTranfrom(background, 80, 8);
        barControl = bar.GetComponent<BarControl>();
        barControl.Init(frontground, background);
        bar.SetParent(parent, false);
    }

    public void SetBarSprite(Transform tran, Color color)
    {
        SpriteRenderer spriteRenderer = tran.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        spriteRenderer.sprite = onePixelSprite;
    }

    public void SetBarTranfrom(Transform tran, float w, float h)
    {
        tran.parent = bar;
        tran.localScale = new Vector3(w, h);
        tran.localScale = new Vector3(w, h);
    }

    class BarControl : MonoBehaviour
    {
        Transform frontground, background, cam;

        public void Init(Transform frontground, Transform background)
        {
            this.frontground = frontground;
            this.background = background;
            cam = Camera.main.transform;
            frontground.position -= Vector3.forward * 0.001f;
            transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
        }
    }
}
