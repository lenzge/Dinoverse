﻿using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace DefaultNamespace.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private EnvironmentData environmentData;
        [SerializeField] private GameObject HUD;
        [SerializeField] private MainController mainController;

        private VisualElement root;
        
        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            
            Button startButton = root.Q<Button>("Start");
            startButton.clicked += OnStartButton;

            RadioButton asexual = root.Q<RadioButton>("Asexual");
            asexual.value = !environmentData.SexualReproduction;
            
            RadioButton sexual = root.Q<RadioButton>("Sexual");
            sexual.value = environmentData.SexualReproduction;
            sexual.RegisterValueChangedCallback(OnSexualChanged);

            Toggle predation = root.Q<Toggle>("Predation");
            predation.value = environmentData.AllowPredation;
            predation.RegisterValueChangedCallback(OnPredationChanged);
            
            IntegerField initialAnimals = root.Q<IntegerField>("InitialAnimals");
            initialAnimals.value = environmentData.InitialAnimalAmount;
            initialAnimals.RegisterValueChangedCallback(OnInitialAnimalsChanged);
            
            IntegerField maxAnimals = root.Q<IntegerField>("MaxAnimals");
            maxAnimals.value = environmentData.MaxAnimalAmount;
            maxAnimals.RegisterValueChangedCallback(OnMaxAnimalsChanged);
            
            IntegerField reproductionEnergy = root.Q<IntegerField>("ReproductionEnergy");
            reproductionEnergy.value = environmentData.ReproductionEnergy;
            reproductionEnergy.RegisterValueChangedCallback(OnReproductionEnergyChanged);
            
            IntegerField initialTrees = root.Q<IntegerField>("InitialTrees");
            initialTrees.value = environmentData.InitialTreeAmount;
            initialTrees.RegisterValueChangedCallback(OnInitialTreesChanged);
            
            IntegerField maxTrees = root.Q<IntegerField>("MaxTrees");
            maxTrees.value = environmentData.MaxTrees;
            maxTrees.RegisterValueChangedCallback(OnMaxTreesChanged);
            
            IntegerField minTrees = root.Q<IntegerField>("MinTrees");
            minTrees.value = environmentData.MinTrees;
            minTrees.RegisterValueChangedCallback(OnMinTreesChanged);
            
            IntegerField lakeCount = root.Q<IntegerField>("LakeCount");
            lakeCount.value = environmentData.LakeCount;
            lakeCount.RegisterValueChangedCallback(OnLakeCountChanged);
            
            IntegerField mapSize = root.Q<IntegerField>("MapSize");
            mapSize.value = environmentData.MapSize;
            mapSize.RegisterValueChangedCallback(OnMapSizeChanged);

        }

        private void OnMapSizeChanged(ChangeEvent<int> evt)
        {
            environmentData.MapSize = evt.newValue;
        }

        private void OnLakeCountChanged(ChangeEvent<int> evt)
        {
            environmentData.LakeCount = evt.newValue;
        }

        private void OnMinTreesChanged(ChangeEvent<int> evt)
        {
            environmentData.MinTrees = evt.newValue;
        }

        private void OnMaxTreesChanged(ChangeEvent<int> evt)
        {
            environmentData.MaxTrees = evt.newValue;
        }

        private void OnInitialTreesChanged(ChangeEvent<int> evt)
        {
            environmentData.InitialTreeAmount = evt.newValue;
        }

        private void OnReproductionEnergyChanged(ChangeEvent<int> evt)
        {
            environmentData.ReproductionEnergy = evt.newValue;
        }

        private void OnMaxAnimalsChanged(ChangeEvent<int> evt)
        {
            environmentData.MaxAnimalAmount = evt.newValue;
        }

        private void OnInitialAnimalsChanged(ChangeEvent<int> evt)
        {
            environmentData.InitialAnimalAmount = evt.newValue;
        }

        private void OnPredationChanged(ChangeEvent<bool> evt)
        {
            environmentData.AllowPredation = evt.newValue;
        }

        private void OnSexualChanged(ChangeEvent<bool> evt)
        {
            environmentData.SexualReproduction = evt.newValue;
        }

        private void OnStartButton()
        {
            HUD.SetActive(true);
            gameObject.SetActive(false);
            
            mainController.StartGame();
        }
    }
}