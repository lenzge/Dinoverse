using System;
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
        [Header("Organs n stuff")]
        public CharacterController CharacterController;
        public Legs Legs;
        public Stomach Stomach;
        public Uterus Uterus;
        public Brain Brain;
        public Eyes Eyes;
        public DNA DNA;
        
        [Space]
        [Header("Info")]
        [SerializeField] private Gender gender;
        [SerializeField] private Layer species;
        [SerializeField] private Layer enemySpecies;
        [SerializeField] private Layer food;
        
        [Space]
        [Header("Plot Infos")]
        public Plot Plot;
        public int Age;
        public int Key;
        public int Population;
        public int Generation;
        public int EatenTrees;
        
        private bool isDrown;
        private Action currentAction;

        private Transform characterTransform;
        
        [HideInInspector] public UnityEvent<AnimalController> Died;

        
        protected override void TimedStart()
        {
            // TODO make Plot Static
            characterTransform = transform;
            Plot = GameObject.Find("Plot").GetComponent<Plot>();
            
            // Reset Variables
            Age = 0;
            EatenTrees = 0;
            isDrown = false;

            // TODO choose action from neural network
            currentAction = Action.Walk;

        }

        public void UpdateName()
        {
            gameObject.name = $"{Key}.{Population}.{Generation}";
        }

        public void Update()
        {
            Legs.Move(this, characterTransform);
        }

        protected override void TimedUpdate()
        {
            Age += 1;
            
            float[] output = Brain.Survive(Eyes.LookAround(characterTransform, food));

            Legs.SetMoveDirection(new Vector2(output[0], output[1]));

            bool isEating = Stomach.TryToEat(characterTransform, CharacterController.radius, food);
            if (isEating)
            {
                EatenTrees += 1;
                Uterus.TryToReproduce(this);
            }
            
            Stomach.BurnCalories(currentAction);
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
                    Uterus.GetChildCount(), timeOfDeath, causeOfDeath);
                
                Died.Invoke(this);
                Destroy(gameObject);
            }
            
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
                return (int) Enums.CauseOfDeath.drown;
            }
            
            return (int) Enums.CauseOfDeath.other;
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int) Layer.Water)
            {
                isDrown = true;
            }
            
        }

    }
}