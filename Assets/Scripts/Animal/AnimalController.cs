﻿using System;
using System.Collections;
using System.Linq;
using DefaultNamespace;
using Enums;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Action = Enums.Action;

namespace Animal
{
    public class AnimalController: TimeBasedBehaviour
    {
        public bool isBot;
        
        [Header("Organs n stuff")]
        public CharacterController CharacterController;
        public Legs Legs;
        public Stomach Stomach;
        public Uterus Uterus;
        public Brain Brain;
        public Eyes Eyes;
        public DNA DNA;
        public GameObject Hearts;

        [Space]
        [Header("Info")]
        [SerializeField] private Gender gender;
        [SerializeField] private Layer species;
        [SerializeField] private Layer enemySpecies;
        [SerializeField] private Layer food;
        
        [Space]
        [Header("Plot Infos")]
        public Plot Plot;
        public AnimalCreator AnimalCreator;
        public int Age;
        public int Key;
        public int Population;
        public int Generation;
        public int EatenTrees;
        public int Fitness;

        [Space]
        public Action CurrentAction;
        
        private bool isDrown;
        private bool isReproducing;

        private Transform characterTransform;
        
        [HideInInspector] public UnityEvent<AnimalController> Died;

        [HideInInspector] public int drowningPunishment;
        
        protected override void TimedStart()
        {
            // TODO make Plot Static
            characterTransform = transform;
            Plot = GameObject.Find("Plot").GetComponent<Plot>();
            AnimalCreator = GameObject.Find("AnimalCreator").GetComponent<AnimalCreator>();
            
            // Reset Variables
            Age = 0;
            EatenTrees = 0;
            Fitness = 0;
            isDrown = false;
            drowningPunishment = 1;
            
            TimedUpdate();

        }

        public void UpdateInfo(int key, int population, int generation)
        {
            Key = key;
            Population = population;
            Generation = generation;
            gameObject.name = $"animal {Key}.{Population}.{Generation}";
        }
        
        // TODO implement subspecies
        public void UpdateInfo(string subspecies)
        {
            gameObject.name = $"animal ({subspecies}) {Key}.{Population}.{Generation}";
        }

        public void Update()
        {
            Legs.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            if (isBot)
            {
                Age = 50;
                EatenTrees = 2;
                Uterus.ReproductionEnergy = 2;
                CurrentAction = Action.Reproduce;
                Legs.SetMoveDirection(Vector2.zero, 0);
                Legs.Animator.SetInteger("Action", 4);
                Hearts.SetActive(true);
                return;
            }
            
            
            Age += 1;
            if (isReproducing) return;

            float[] inputs = PerceiveInputs();
            float[] output = Brain.Survive(inputs);

            CurrentAction = EvaluateAction(output);
            //Debug.Log($"[{name}] inputs: {ArrayToString(inputs)}, outputs: {ArrayToString(output)}, Action: {CurrentAction}");

            switch (CurrentAction)
            {
                case Action.Rest:
                    Legs.SetMoveDirection(Vector2.zero, 0);
                    Legs.Animator.SetInteger("Action", 3);
                    Hearts.SetActive(false);
                    break;
                case Action.Eat:
                    Legs.SetMoveDirection(new Vector2(output[0], output[1]), output[2]);
                    Legs.Animator.SetInteger("Action", 2);
                    Hearts.SetActive(false);
                    bool isEating = Stomach.TryToEat(characterTransform, CharacterController.radius, food);
                    if (isEating) EatenTrees += 1;
                    if (isEating) Fitness += 2;
                    if (isEating) Uterus.ReproductionEnergy += 1;
                    break;
                case Action.Reproduce:
                    Legs.SetMoveDirection(new Vector2(output[0], output[1]), output[2]);
                    Legs.Animator.SetInteger("Action", 2);
                    Hearts.SetActive(true);
                    bool reproduced = Uterus.TryToReproduce(this, species);
                    if (reproduced) Fitness += 20;
                    if (reproduced) StartCoroutine(ReproductionFreeze());
                    //if (reproduced) Age += 5000;
                    break;
                default:
                    Legs.SetMoveDirection(Vector2.zero, 0);
                    Legs.Animator.SetInteger("Action", 3);
                    Hearts.SetActive(false);
                    break;
            }
            
            Stomach.BurnCalories(CurrentAction, output[2]);
            KillIfDead();
        }

        public void InitOrgans(bool isChild)
        {
            Brain.Init(isChild);
            Eyes.Init();
            Legs.Init();
            Stomach.Init();
            Uterus.Init();
        }

        public float AgeLevel()
        {
            return (float) Age / DNA.LifeExpectation[0];
        }

        private float[] PerceiveInputs()
        {
            float[] inputs = new float[DNA.NumRaycasts[0]*2 + 3 + 12];
            
            // x Food Raycasts and 1 Water Raycast (already normed)
            float[] raycasts = Eyes.LookAround(characterTransform, food);
            for (int i = 0; i < raycasts.Length; i++)
            {
                inputs[i] = raycasts[i];
            }

            // Current calories relative to max calories
            inputs[DNA.NumRaycasts[0]*2 + 0] = Stomach.HungerInput();
            
            // Current age relative to SexualMaturity
            inputs[DNA.NumRaycasts[0]*2 + 1] = Uterus.SexualMaturityLevel(this);
            
            // Current Reproduction Energy relative to the needed one
            inputs[DNA.NumRaycasts[0]*2 + 2] = Uterus.ReproductionEnergyLevel(this);
            
            // Nearest mates relative position, relative age and relative eaten Trees (to the best seen)
            float[] mates = Eyes.LookForMate(characterTransform, species);
            for (int i = 0; i < mates.Length; i++)
            {
                inputs[DNA.NumRaycasts[0]*2 + 3 + i] = mates[i];
            }
            
            return inputs;
        }

        private Action EvaluateAction(float[] inputs)
        {
            float[] actionInputs = inputs.Skip(3).Take(3).ToArray();
            int index = Array.IndexOf(actionInputs, actionInputs.Max());
            switch (index)
            {
                case 0:
                    return Action.Eat;
                case 1:
                    return Action.Rest;
                case 2:
                    return Action.Reproduce;
                default:
                    return Action.Rest;
            }
        }
        

        private void KillIfDead()
        {
            int causeOfDeath = CauseOfDeath();
            if (causeOfDeath != (int) Enums.CauseOfDeath.other)
            {
                int timeOfDeath = Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f);
                Debug.Log($"[{gameObject.name}] {timeOfDeath} R.I.P: Died the age of {Age}, " +
                                 $"Reproduced {Uterus.GetChildCount()} times, " +
                                 $"Found {EatenTrees} Trees. " +
                                 $"Cause of Death: {(CauseOfDeath)Enum.Parse(typeof(CauseOfDeath), causeOfDeath.ToString())}");
                Plot.SaveData(Key, Population, Generation,Age, EatenTrees, 
                    Uterus.GetChildCount(), timeOfDeath, causeOfDeath, Fitness);
                
                Died.Invoke(this);
                StartCoroutine(DestroyAfterAni());
            }
            
        }
        
        IEnumerator DestroyAfterAni()
        {
            if (isDrown)
            {
                float timeInterval = 4f / EnvironmentData.TimeSpeed;
                yield return new WaitForSeconds(timeInterval);
            }
            Destroy(gameObject);
            
        }
        
        IEnumerator ReproductionFreeze()
        {
            isReproducing = true;
            Legs.SetMoveDirection(Vector2.zero, 0);
            Legs.Animator.SetInteger("Action", 4);
            float timeInterval = 4f / EnvironmentData.TimeSpeed;
            yield return new WaitForSeconds(timeInterval);
            isReproducing = false;

        }

        private int CauseOfDeath()
        {
            if (Stomach.IsStarving())
            {
                return (int) Enums.CauseOfDeath.starved;
            }

            if (Age >= DNA.LifeExpectation[0])
            {
                return (int) Enums.CauseOfDeath.decrepitude;
            }

            if (isDrown)
            {
                Fitness -= AnimalCreator.DrowningPunishment;
                return (int) Enums.CauseOfDeath.drown;
            }
            
            return (int) Enums.CauseOfDeath.other;
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int) Layer.Water)
            {
                isDrown = true;
                Legs.Animator.SetTrigger("isDrown");
            }
            
        }
        
        private string ArrayToString(float[] array)
        {
            // Convert the array elements to strings and join them with commas
            return "[" + string.Join(" , ", array) + "]";
        }
        

    }
}