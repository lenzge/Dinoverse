using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace.UI
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Plot plotData;
        [SerializeField] private Sprite circleSprite;

        private RectTransform graphContainer;

        private void Awake()
        {
            graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        }

        private GameObject CreateCircle(Vector2 anchoredPosition)
        {
            GameObject circleObject = new GameObject("circle", typeof(Image));
            circleObject.transform.SetParent(graphContainer, false);
            circleObject.GetComponent<Image>().sprite = circleSprite;
            RectTransform rectTransform = circleObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            
            return circleObject;
        }

        public void ShowGraph()
        {
            ClearGraph();
            List<float> valueList = plotData.LifeSpan();

            float graphHeight = graphContainer.sizeDelta.y;
            float graphWidth = graphContainer.sizeDelta.x;
            float yMaximum = 300f;
            float xSize = graphWidth / (valueList.Count -1);
            GameObject lastCircle = null;
            
            for (int i = 0; i < valueList.Count; i++)
            {
                float xPosition = i * xSize;
                float yPosition = (valueList[i] / yMaximum * graphHeight);
                GameObject circleObject = CreateCircle(new Vector2(xPosition, yPosition));
                if (lastCircle != null)
                {
                    CreateConnection(lastCircle.GetComponent<RectTransform>().anchoredPosition, circleObject.GetComponent<RectTransform>().anchoredPosition);
                }
                lastCircle = circleObject;
            }

        }

        private void CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject connectionObject = new GameObject("dotConnection", typeof(Image));
            connectionObject.transform.SetParent(graphContainer, false);
            connectionObject.GetComponent<Image>().tintColor = new Color(1, 1, 1, .5f);
            RectTransform rectTransform = connectionObject.GetComponent<RectTransform>();
            Vector2 direction = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchoredPosition = dotPositionA + direction*distance*.5f;
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.localEulerAngles = new Vector3(0, 0, VectorToEuler(direction));
        }

        private float VectorToEuler(Vector2 v)
        { 
            float eulerAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            return eulerAngle;
        }

        private void ClearGraph()
        {
            int childCount = graphContainer.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Destroy( graphContainer.GetChild(i).gameObject);
            }
        }
    }
}