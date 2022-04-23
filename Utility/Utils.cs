using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UniRx;

//using Zenject;

namespace Utility
{
    public static class Utils
    {
        public static System.Random Random = new System.Random();
    
    
        // public static  T GetFromSceneContext<T>() where T : class 
        // {
        //     var source = ProjectContext.Instance.Container
        //         .Resolve<SceneContextRegistry>().SceneContexts;
        //     if (source is IList<SceneContext> sourceList)
        //     {
        //         switch (sourceList.Count)
        //         {
        //             case 0:
        //                 return null;
        //             case 1:
        //                 return sourceList[0].Container
        //                     .Resolve<T>();
        //         }
        //     }
        //     else
        //     {
        //         using (IEnumerator<SceneContext> enumerator = source.GetEnumerator())
        //         {
        //             if (!enumerator.MoveNext())
        //                 return null;
        //             SceneContext current = enumerator.Current;
        //             if (!enumerator.MoveNext())
        //                 return current.Container
        //                     .Resolve<T>();;
        //         }
        //     }
        //     return null;
        // }
        //
        // public static IDisposable SubscribeToText(this System.IObservable<string> source, TextMeshProUGUI text)
        // {
        //     return source.SubscribeWithState(text, (x, t) => t.text = x);
        // }
        //
        // public static IDisposable SubscribeToText<T>(this System.IObservable<T> source, TextMeshProUGUI text)
        // {
        //     return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        // }
        //
    
        public static Vector3 RandomPointInBounds(Bounds bounds) {
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    
        public static Vector2 GetAnchoredPosGorMousePos(this RectTransform rec,Camera iuCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rec.parent.transform as RectTransform,
                Input.mousePosition, iuCamera, out Vector2 result);
            return result;
        }

        public static string GetDescription(this Enum value)
        {
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(
                value.GetType().GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Single(x => x.GetValue(null).Equals(value)),
                typeof(DescriptionAttribute)))?.Description ?? value.ToString();
        }
    
        public static bool IsPointerOverUIObject(bool useRaycast = true)
        {
            bool result = false;

            if (useRaycast)
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                result = results.Count > 0;

                if (result != EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.LogError("This is probably a bug like KGD-158." +
                                   "If you think that this method must return true in this situation," +
                                   "consider calling it with useRaycasts=false parameter");
                }
            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            result = results.Count > 0;
#else
                result = EventSystem.current.IsPointerOverGameObject();
#endif
            }

            return result;
        }
        
   

    
        public static float GameScreenWidth
        {
#if UNITY_EDITOR
            get { return Single.Parse(UnityStats.screenRes.Split('x')[0]); }
#else
        get { return Screen.width; }
#endif
        }
    
        public static float GameScreenHeight
        {
#if UNITY_EDITOR
            get { return Single.Parse(UnityStats.screenRes.Split('x')[1]); }
#else
             get { return Screen.height; }
#endif
        }
    
        public static float GameScreenWidthRatio
        {
#if UNITY_EDITOR
            get { return GameScreenWidth / 1920f; }
#else
        get { return GameScreenWidth / 1920f; }
#endif
        }
    
        public static float GameScreenHeightRatio
        {
#if UNITY_EDITOR
            get { return GameScreenHeight / 1080f; }
#else
        get { return GameScreenHeight / 1080f; }
#endif
        }

   
    

        public static bool Overlaps(this RectTransform a, RectTransform b)
        { 
            var aa = a.WorldRect();
            var bb = b.WorldRect();
            return a.rect.Overlaps(b.rect);
        }

        public static (Vector3 upLeft, Vector3 upRight,Vector3 bottomLeft, Vector3 bottomRight) WorldRectTr(this RectTransform a)
        {
            Vector2 sizeDelta = a.rect.size;
            float rectTransformWidth = sizeDelta.x * a.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * a.lossyScale.y;

            var right =  a.transform.right * (rectTransformWidth * 0.5f);
            var up =a.transform.up * (rectTransformHeight * 0.5f);

            var ul = a.position + (up - right);
            var ur = a.position + (up + right);
            var bl = a.position + (-right -up );
            var br = a.position + (right -up);
       
            return (ul, ur, bl, br);
        }
    
        public static Rect WorldRect(this RectTransform rectTransform) {
            Vector2 sizeDelta = rectTransform.rect.size;
            float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

            Vector3 position = rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
        }
    
        public static Vector3 WorldToCanvasPosition(this Camera camera, Canvas canvas, Vector3 worldPosition)
        {
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var scale = canvasRect.sizeDelta;
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }
        //
        // public static TweenerCore<Color, Color, ColorOptions> AlphaOut(this Graphic target, float duration, float toAlpha)
        // {
        //     return ChangeAlpha(target,1,toAlpha,duration).SetEase(Ease.OutSine);
        // }
        //
        // public static TweenerCore<Color, Color, ColorOptions> AlphaOut(this Graphic target,float duration)
        // {
        //     return ChangeAlpha(target,1,0,duration).SetEase(Ease.OutSine);
        // }
        //
        // public static TweenerCore<Color, Color, ColorOptions> AlphaIn(this Graphic target, float duration)
        // {
        //     return ChangeAlpha(target,0,1,duration).SetEase(Ease.OutSine);
        // }
        //
        // public static TweenerCore<Color, Color, ColorOptions> AlphaIn(this Graphic target, float duration,float toAlpha)
        // {
        //     return ChangeAlpha(target,0,toAlpha,duration).SetEase(Ease.OutSine);
        // }
        //
        // public static TweenerCore<Color, Color, ColorOptions> ChangeAlpha(this Graphic target, float startValue, float endValue,float duration)
        // {
        //     Color startColor = target.color;
        //     startColor.a = startValue;
        //
        //     Color endColor = target.color;
        //     endColor.a = endValue;
        //
        //     target.color = startColor;
        //     return  target.DOColor(endColor, duration);
        // }
    
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            var c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
        public static void SetAlpha(this SpriteRenderer target, float alpha)
        {
            var c = target.color;
            c.a = alpha;
            target.color = c;
        }

        public static void Switch<T>(this IList<T> array, int indexFrom, int indexTo)
        {
            var aux = array[indexFrom];
            array[indexFrom] = array[indexTo];
            array[indexTo] = aux;
        }
    }
}