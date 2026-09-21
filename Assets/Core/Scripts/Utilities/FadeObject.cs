using System.Collections;
using UnityEngine;

namespace Platformer.Utilities
{
    public class FadeObject : MonoBehaviour
    {
        [SerializeField] private GameObject _fadeObject;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private float _delayFadeStart = 1f;

        private void Start()
        {
            if (_fadeObject == null || _canvasGroup == null)
            {
                Debug.LogWarning("[FadeIn] Fade object or CanvasGroup is not assigned.");
                return;
            }

            _fadeObject.SetActive(true);
            _canvasGroup.alpha = 1f;

            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(_delayFadeStart);
            
            float elapsedTime = 0f;

            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _fadeObject.SetActive(false);
        }
    }
}