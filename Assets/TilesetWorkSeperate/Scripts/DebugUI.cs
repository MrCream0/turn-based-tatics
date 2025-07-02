using UnityEngine;

public class DebugUI : MonoBehaviour
{
    private void Update()
    {
        foreach (RectTransform rect in FindObjectsOfType<RectTransform>())
        {
            if (float.IsNaN(rect.anchoredPosition.x) || float.IsNaN(rect.anchoredPosition.y) ||
                float.IsInfinity(rect.anchoredPosition.x) || float.IsInfinity(rect.anchoredPosition.y))
            {
                Debug.LogError($"Invalid RectTransform position on {rect.gameObject.name}: {rect.anchoredPosition}");
            }
            if (rect.localScale == Vector3.zero || float.IsNaN(rect.localScale.x))
            {
                Debug.LogError($"Invalid RectTransform scale on {rect.gameObject.name}: {rect.localScale}");
            }
            if (rect.sizeDelta.x <= 0 || rect.sizeDelta.y <= 0 || float.IsNaN(rect.sizeDelta.x))
            {
                Debug.LogError($"Invalid RectTransform size on {rect.gameObject.name}: {rect.sizeDelta}");
            }
        }
    }
}