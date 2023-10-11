using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private EnvironmentData environmentData;

        private Label visualizeTimeSpeed;
        
        private void OnEnable()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            SliderInt timeSpeed = root.Q<SliderInt>("TimeSpeed");
            visualizeTimeSpeed = root.Q<Label>("VisualizeSpeed");

            timeSpeed.highValue = environmentData.MaxTimeSpeed;
            environmentData.SetTimeSpeed(timeSpeed.value);
            
            timeSpeed.RegisterValueChangedCallback(OnTimeSpeedChanged);

        }

        private void OnTimeSpeedChanged(ChangeEvent<int> evt)
        {
            environmentData.SetTimeSpeed(evt.newValue);
            visualizeTimeSpeed.text = $"{evt.newValue}x";
        }
    }
}