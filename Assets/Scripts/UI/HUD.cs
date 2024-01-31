using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            
            Button restart = root.Q<Button>("Restart");
            restart.clicked += OnRestartButton;
            
            Button killAnimals = root.Q<Button>("KillAnimals");
            killAnimals.clicked += OnKillAnimalsButton;
            
            Button killTrees = root.Q<Button>("KillTrees");
            killTrees.clicked += OnKillTreesButton;
            
            Button separation = root.Q<Button>("Separation");
            separation.clicked += OnSeparationButton;
            
            Button allowPredation = root.Q<Button>("AllowPredation");
            allowPredation.clicked += OnAllowPredationButton;
            
            timeSpeed.RegisterValueChangedCallback(OnTimeSpeedChanged);

        }

        private void OnAllowPredationButton()
        {
            environmentData.ChangePredation();
        }

        private void OnSeparationButton()
        {
           environmentData.SeparationEvent.Invoke();
        }

        private void OnKillTreesButton()
        {
            environmentData.KillTreesEvent.Invoke();
        }

        private void OnKillAnimalsButton()
        {
            environmentData.KillAnimalsEvent.Invoke();
        }

        private void OnRestartButton()
        {
            environmentData.SeparationEvent.Invoke();
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        private void OnTimeSpeedChanged(ChangeEvent<int> evt)
        {
            environmentData.SetTimeSpeed(evt.newValue);
            visualizeTimeSpeed.text = $"{evt.newValue}x";
        }
    }
}