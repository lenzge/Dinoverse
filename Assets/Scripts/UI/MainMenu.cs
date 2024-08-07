﻿using System.Diagnostics;
using Enums;
using UnityEngine;
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
        private IntegerField lakeCount;
        
        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            
            Button startButton = root.Q<Button>("Start");
            startButton.clicked += OnStartButton;
            
            Button quitButton = root.Q<Button>("Quit");
            quitButton.clicked += OnQuitButton;

            RadioButton asexual = root.Q<RadioButton>("Asexual");
            asexual.value = !environmentData.SexualReproduction;
            
            RadioButton sexual = root.Q<RadioButton>("Sexual");
            sexual.value = environmentData.SexualReproduction;
            sexual.RegisterValueChangedCallback(OnSexualChanged);
            
            RadioButton noneChange = root.Q<RadioButton>("NoneChange");
            if (environmentData.RateOfChange == Change.none) noneChange.value = true;
            else noneChange.value = false;
            noneChange.RegisterValueChangedCallback(OnNoneChangeChanged);
            
            RadioButton slowChange = root.Q<RadioButton>("SlowChange");
            if (environmentData.RateOfChange == Change.slow) slowChange.value = true;
            else slowChange.value = false;
            slowChange.RegisterValueChangedCallback(OnSlowChangeChanged);
            
            RadioButton rapidChange = root.Q<RadioButton>("RapidChange");
            if (environmentData.RateOfChange == Change.rapid) rapidChange.value = true;
            else rapidChange.value = false;
            rapidChange.RegisterValueChangedCallback(OnRapidChangeChanged);

            Toggle predation = root.Q<Toggle>("Predation");
            predation.value = environmentData.AllowPredation;
            predation.RegisterValueChangedCallback(OnPredationChanged);
            
            Toggle classify = root.Q<Toggle>("Classify");
            classify.value = environmentData.Classify;
            classify.RegisterValueChangedCallback(OnPClassifyChanged);
            
            IntegerField initialAnimals = root.Q<IntegerField>("InitialAnimals");
            initialAnimals.value = environmentData.InitialAnimalAmount;
            initialAnimals.RegisterValueChangedCallback(OnInitialAnimalsChanged);
            
            IntegerField maxAnimals = root.Q<IntegerField>("MaxAnimals");
            maxAnimals.value = environmentData.MaxAnimalAmount;
            maxAnimals.RegisterValueChangedCallback(OnMaxAnimalsChanged);
            
            IntegerField minAnimals = root.Q<IntegerField>("MinAnimals");
            minAnimals.value = environmentData.MinAnimalAmount;
            minAnimals.RegisterValueChangedCallback(OnMinAnimalsChanged);

            IntegerField initialTrees = root.Q<IntegerField>("InitialTrees");
            initialTrees.value = environmentData.InitialTreeAmount;
            initialTrees.RegisterValueChangedCallback(OnInitialTreesChanged);
            
            IntegerField maxTrees = root.Q<IntegerField>("MaxTrees");
            maxTrees.value = environmentData.MaxTrees;
            maxTrees.RegisterValueChangedCallback(OnMaxTreesChanged);
            
            IntegerField minTrees = root.Q<IntegerField>("MinTrees");
            minTrees.value = environmentData.MinTrees;
            minTrees.RegisterValueChangedCallback(OnMinTreesChanged);
            
            lakeCount = root.Q<IntegerField>("LakeCount");
            lakeCount.value = environmentData.LakeCount;
            lakeCount.RegisterValueChangedCallback(OnLakeCountChanged);
            
            IntegerField maxLakeCount = root.Q<IntegerField>("MaxLakeCount");
            maxLakeCount.value = environmentData.MaxLakeCount;
            maxLakeCount.RegisterValueChangedCallback(OnMaxLakeCountChanged);
            
            IntegerField mapSize = root.Q<IntegerField>("MapSize");
            mapSize.value = environmentData.MapSize;
            mapSize.RegisterValueChangedCallback(OnMapSizeChanged);
            
            Toggle constantTreeAmount = root.Q<Toggle>("ConstantTreeAmount");
            constantTreeAmount.value = environmentData.ConstantTreeAmount;
            constantTreeAmount.RegisterValueChangedCallback(OnConstantTreeAmountChanged);
            
            Toggle randomSpawnPoint = root.Q<Toggle>("RandomSpawnPoint");
            randomSpawnPoint.value = environmentData.RandomSpawnPoint;
            randomSpawnPoint.RegisterValueChangedCallback(OnRandomSpawnPointChanged);
            
            Toggle endlessWorld = root.Q<Toggle>("EndlessWorld");
            endlessWorld.value = environmentData.EndlessWorld;
            endlessWorld.RegisterValueChangedCallback(OnEndlessWorldChanged);

        }

        private void OnMinAnimalsChanged(ChangeEvent<int> evt)
        {
            environmentData.MinAnimalAmount = evt.newValue;
        }

        private void OnMaxLakeCountChanged(ChangeEvent<int> evt)
        {
            environmentData.MaxLakeCount = evt.newValue;
        }

        private void OnPClassifyChanged(ChangeEvent<bool> evt)
        {
            environmentData.Classify = evt.newValue;
        }

        private void OnRapidChangeChanged(ChangeEvent<bool> evt)
        {
            if (evt.newValue) environmentData.RateOfChange = Change.rapid;
        }

        private void OnSlowChangeChanged(ChangeEvent<bool> evt)
        {
            if (evt.newValue) environmentData.RateOfChange = Change.slow;
        }

        private void OnNoneChangeChanged(ChangeEvent<bool> evt)
        {
            if (evt.newValue) environmentData.RateOfChange = Change.none;
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        private void OnEndlessWorldChanged(ChangeEvent<bool> evt)
        {
            environmentData.EndlessWorld = evt.newValue;
            if (environmentData.EndlessWorld)
            {
                environmentData.LakeCount = 0;
                lakeCount.value = environmentData.LakeCount;
            }
        }

        private void OnRandomSpawnPointChanged(ChangeEvent<bool> evt)
        {
            environmentData.RandomSpawnPoint = evt.newValue;
        }

        private void OnConstantTreeAmountChanged(ChangeEvent<bool> evt)
        {
            environmentData.ConstantTreeAmount = evt.newValue;
        }

        private void OnMapSizeChanged(ChangeEvent<int> evt)
        {
            environmentData.MapSize = evt.newValue;
        }

        private void OnLakeCountChanged(ChangeEvent<int> evt)
        {
            if (environmentData.EndlessWorld)
            {
                lakeCount.value = 0;
            }
            else
            {
                environmentData.LakeCount = evt.newValue;
            }
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