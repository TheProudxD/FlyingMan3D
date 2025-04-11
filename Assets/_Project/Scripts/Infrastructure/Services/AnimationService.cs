using System;
using _Project.Scripts.Tools.Extensions;
using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Infrastructure.Services
{
    public class AnimationService : IService
    {
        private void AddMotionHandle(MotionHandle mh, GameObject modifier,
            CompositeMotionHandle compositeMotionHandle)
        {
            if (compositeMotionHandle == null)
                mh.AddTo(modifier);
            else
                mh.AddTo(compositeMotionHandle);
        }

        public void FadeOut(GameObject popup, float animationDuration, Ease ease = Ease.Linear,
            Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null) =>
            AddMotionHandle(
                LMotion.Create(Vector3.zero, Vector3.one, animationDuration)
                    .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                    .WithEase(ease)
                    .WithOnCancel(callback)
                    .WithOnComplete(callback)
                    .Bind(x => popup.transform.localScale = x), popup,
                compositeMotionHandle);

        public void FadeIn(GameObject popup, float animationDuration, Ease ease = Ease.Linear,
            Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null) =>
            AddMotionHandle(
                LMotion.Create(Vector3.one, Vector3.zero, animationDuration)
                    .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                    .WithEase(ease)
                    .WithOnCancel(callback)
                    .WithOnComplete(callback)
                    .Bind(x => popup.transform.localScale = x), popup,
                compositeMotionHandle);

        public void ShakingScale(Transform modifierTransform, float from, float to, float duration,
            int loops = 2, Ease ease = Ease.Linear, Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion.Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithEase(ease)
                .WithLoops(loops, LoopType.Yoyo)
                .WithOnComplete(callback)
                .Bind(x => modifierTransform.localScale = new Vector3(x, x, x));

            AddMotionHandle(mh, modifierTransform.gameObject, compositeMotionHandle);
        }

        public void Scale(Transform modifierTransform, Vector3 from, Vector3 to, float duration,
            int loops = 1, Ease ease = Ease.Linear, float delay = 0, Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion.Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithEase(ease)
                .WithLoops(loops)
                .WithOnComplete(callback)
                .Bind(x => modifierTransform.localScale = x);

            AddMotionHandle(mh, modifierTransform.gameObject, compositeMotionHandle);
        }

        public void Move(Transform modifierTransform, Vector3 from, Vector3 to, float duration,
            int loops = 1, Ease ease = Ease.Linear, float delay = 0, Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion.Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithEase(ease)
                .WithLoops(loops)
                .WithOnComplete(callback)
                .Bind(x => modifierTransform.position = x);

            AddMotionHandle(mh, modifierTransform.gameObject, compositeMotionHandle);
        }

        public void Rotate(Transform modifierTransform, Vector3 from, Vector3 to, float duration,
            int loops = 1, Ease ease = Ease.Linear, LoopType loopType = LoopType.Restart, float delay = 0, Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion
                .Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithEase(ease)
                .WithLoops(loops, loopType)
                .WithOnComplete(callback)
                .Bind(x => modifierTransform.localRotation = Quaternion.Euler(x));

            AddMotionHandle(mh, modifierTransform.gameObject, compositeMotionHandle);
        }

        public void RotateZ(Transform modifierTransform, float shakeAngle, float duration,
            int loops = -1, Ease ease = Ease.OutSine, LoopType loopType = LoopType.Restart, Action callback = null, float delay = 0,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            Vector3 defaultPosition = modifierTransform.localRotation.eulerAngles;

            MotionHandle mh = LMotion
                .Create(defaultPosition.WithZ(-shakeAngle), defaultPosition.WithZ(shakeAngle), duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithLoops(loops, loopType)
                .WithEase(ease)
                .WithOnCancel(() => modifierTransform.localRotation = Quaternion.Euler(defaultPosition))
                .WithOnComplete(callback)
                .Bind(x => modifierTransform.localRotation = Quaternion.Euler(x));

            AddMotionHandle(mh, modifierTransform.gameObject, compositeMotionHandle);
        }

        public void Color(TMP_Text text, Color from, Color to, float duration,
            int loops = 1, Ease ease = Ease.Linear, LoopType loopType = LoopType.Restart, float delay = 0, Action callback = null,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion
                .Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithEase(ease)
                .WithLoops(loops, loopType)
                .WithOnComplete(callback)
                .Bind(c => text.color = c);

            AddMotionHandle(mh, text.gameObject, compositeMotionHandle);
        }

        public void Color(SpriteRenderer sprite, Color from, Color to, float duration,
            int loops = 1, Ease ease = Ease.Linear, Action callback = null, float delay = 0,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion
                .Create(from, to, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithDelay(delay)
                .WithEase(ease)
                .WithLoops(loops)
                .WithOnComplete(callback)
                .Bind(c => sprite.color = c);

            AddMotionHandle(mh, sprite.gameObject, compositeMotionHandle);
        }

        public void ResourceChanged(Transform transform, int oldValue, int currentValue, float duration,
            Action<int> onChanging, int loops = -1, Ease ease = Ease.OutSine,
            CompositeMotionHandle compositeMotionHandle = null)
        {
            MotionHandle mh = LMotion.Create(oldValue, currentValue, duration)
                .WithScheduler(MotionScheduler.UpdateIgnoreTimeScale)
                .WithEase(ease)
                .Bind(x => onChanging?.Invoke(x))
                .AddTo(transform.gameObject);

            AddMotionHandle(mh, transform.gameObject, compositeMotionHandle);
        }
    }
}