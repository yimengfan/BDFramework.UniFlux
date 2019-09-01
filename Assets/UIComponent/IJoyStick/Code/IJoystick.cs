using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class IJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<Vector2> OnDragAction = null; //滑动回调,返回归一化向量,使用者需要自己转换成方向
    public Action OnPointUpAction = null;        //抬起回调
    public Action<Vector2> OnPointDownAction = null; //摁下回调,返回归一化向量,使用者需要自己转换成方向
    private Camera UICamera;
    private RectTransform bgRectTransform = null;
    private RectTransform centerPointReactTransform = null;
    public float R = 0; //半径 
    private Vector2 centerStartPos;  //center开始位置,
    private Vector2 bgOffsetPos;  //bg和center偏移的位置,
    void Awake()
    {
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>(); // 必须为Canvas-camera模式下生效
        bgRectTransform = this.transform as RectTransform;
        centerPointReactTransform = this.transform.GetChild(0) as RectTransform; 
        R = this.GetComponent<Image>().rectTransform.rect.width / 2; //一般joystick是内切圆，所以除以2就行,有特殊需求,需要自己改
        bgOffsetPos = new Vector2(R, R);
        centerStartPos = centerPointReactTransform.anchoredPosition;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        this.centerPointReactTransform.anchoredPosition = centerStartPos;
        if (OnPointUpAction != null) OnPointUpAction();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        var pos = GetUIPos(eventData.position);
        this.centerPointReactTransform.anchoredPosition = pos;
        if (OnPointDownAction != null) OnPointDownAction(pos);
    }
    public void OnDrag(PointerEventData eventData)
    {
        var pos = GetUIPos(eventData.position);
        if (pos.magnitude > R) pos = pos.normalized * R;
        this.centerPointReactTransform.anchoredPosition = pos;
        if (OnDragAction != null) OnDragAction(pos);
    }
    private Vector2 pos;
    private Vector2 GetUIPos(Vector2 viewPos)
    {
        pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.bgRectTransform, viewPos, UICamera, out pos);
        return pos - bgOffsetPos;
    }
}