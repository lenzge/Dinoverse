using System;
using System.Collections;
using System.Linq;
using DefaultNamespace;
using Enums;
using UnityEngine;
using UnityEngine.Events;
using Util;
using Action = Enums.Action;
using Vector2 = UnityEngine.Vector2;

namespace Animal
{
    public class AnimalController: TimeBasedBehaviour
    {
        public AnimalCreator AnimalCreator;
        public Material Material;

        [Header("Organs n stuff")]
        public CharacterController CharacterController;
        public Legs Legs;
        public Stomach Stomach;
        public Uterus Uterus;
        public Brain Brain;
        public Eyes Eyes;
        public Weapon Weapon;
        public DNA DNA;
        public Genome Genome;
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
        public int Age;
        public int Key;
        public int Population;
        public int Generation;
        public int EatenTrees;
        public int EatenAnimals;
        public int Fitness;
        public int NewLevel;
        public bool IsDrown;
        public bool IsKilled;
        public float Color;

        [Space]
        public Action CurrentAction;

        private bool isInAnimationFreeze;
        private bool isInDrownAni;
        private Vector2 lastDirection;
        private int actionSpace;
        private Collider[] colliderBuffer;
        private Material[] materials;

        private Transform characterTransform;
        
        [HideInInspector] public UnityEvent<AnimalController> Died;
        
        
        protected override void TimedStart()
        {
            characterTransform = transform;
            Plot = GameObject.Find("Plot").GetComponent<Plot>();
            materials = gameObject.GetComponentInChildren<Renderer>().materials;
            UpdateColor(0.266f);
            UpdateScale();

            // Reset Variables
            Age = 0;
            EatenTrees = 0;
            EatenAnimals = 0;
            Fitness = 0;
            IsDrown = false;
            IsKilled = false;
            isInDrownAni = false;
            NewLevel = 0;
            colliderBuffer = new Collider[1];
            isInAnimationFreeze = false;
            CurrentAction = Action.Chill;
            
            actionSpace = 3;
            if (EnvironmentData.AllowPredation)
            {
                actionSpace = 4;
            }
            
            //Debug.LogError(Genome.Weights.Length);
            //Debug.LogError(Genome.Biases.Length);
            TimedUpdate();

        }

        public void UpdateInfo(int key, int population, int generation, int hue = 266)
        {
            Key = key;
            Population = population;
            Generation = generation;
            gameObject.name = $"animal {Key}.{Population}.{Generation} | {hue}";
        }
        
        // TODO implement subspecies
        public void UpdateInfo(string subspecies)
        {
            gameObject.name = $"animal ({subspecies}) {Key}.{Population}.{Generation}";
        }

        public void Update()
        {
            if (isInDrownAni) return;
            Legs.Move(characterTransform);
        }
        
        public int EvaluateFitness()
        {
            int fitness = 0;
            fitness += Age / 50; //25
            fitness += EatenTrees * 1; //2
            fitness += Uterus.GetChildCountSolo() * 3;
            fitness += Uterus.GetChildCountMutual() * 3; // 6
            //if (IsDrown) fitness -= 10;
            //fitness += (int) Eyes.NavigationFitness;
            Fitness = fitness;
            return fitness;
        }

        public int GetStrength()
        {
            return DNA.Weight[0] * 10 + (int) Stomach.GetCurrentCalories();
        }

        private Vector2 SetMovementDirection(float x, float y)
        {
            if (Mathf.Abs(x) < 0.01 && Mathf.Abs(y) < 0.01)
            {
                return Vector2.zero;
            }
            else return new Vector2(x, y);
        }

        private int SetMovementSpeed(float speedValue, Vector2 direction)
        {
            if (speedValue < 0.01 || direction == Vector2.zero)
            {
                return 0;
            }

            return DNA.MovementSpeed[1] + (int) (speedValue * (DNA.MovementSpeed[0] - DNA.MovementSpeed[1]));
        }

        protected override void TimedUpdate()
        {
            if (isInDrownAni) return;

            Age += 1;
            if (isInAnimationFreeze)
            {
                Stomach.BurnCalories(CurrentAction, 0, isInAnimationFreeze);
                return;
            }
            
            KillIfDead();

            //Debug.Log("last direction " + lastDirection);
            float[] inputs = PerceiveInputs();
            float[] output = Brain.Survive(inputs);

            CurrentAction = EvaluateAction(output);
            //Debug.Log($"[{name}] inputs: {ArrayToString(inputs)}, outputs: {ArrayToString(output)}, Action: {CurrentAction}");

            Vector2 movementDirection = SetMovementDirection(output[0], output[1]);
            int movementSpeed = SetMovementSpeed(output[2], movementDirection);
            Legs.SetMoveDirection(movementDirection, movementSpeed);
            lastDirection = movementDirection;

            //Debug.Log($"[{name}] Direction {new Vector2(output[0], output[1])}, speed: {output[2]} \n" +
                      //$"corrected: {movementDirection}, {movementSpeed}, {Legs.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")}");

            switch (CurrentAction)
            {
                case Action.Chill:
                    Hearts.SetActive(false);
                    break;
                case Action.Eat:
                    Hearts.SetActive(false);
                    bool isEating = Stomach.TryToEatPlants(characterTransform, CharacterController.radius, food);
                    if (isEating) EatenTrees += 1;
                    if (isEating) Uterus.ReproductionEnergy += 1;
                    if (isEating) StartCoroutine(AnimationFreeze((int) Action.Eat));
                    break;
                case Action.Reproduce:
                    Hearts.SetActive(true);
                    bool reproduced = Uterus.TryToReproduce(species);
                    if (reproduced) StartCoroutine(AnimationFreeze((int) Action.Reproduce));
                    break;
                case Action.Fight:
                    Hearts.SetActive(false);
                    bool fight = Weapon.TryToFight(species);
                    if (fight && !IsKilled) EatenAnimals += 1;
                    if (fight && !IsKilled) Debug.LogError($"{name} killed another animal. Eaten animals: {EatenAnimals}. Animation Freeze? {isInAnimationFreeze}");
                    if (fight && !IsKilled) Uterus.ReproductionEnergy += 1;
                    if (fight) StartCoroutine(AnimationFreeze((int) Action.Fight));
                    break;
                default:
                    Hearts.SetActive(false);
                    break;
            }
            
            Stomach.BurnCalories(CurrentAction, movementSpeed, isInAnimationFreeze);
        }

        public void InitOrgans(bool isChild)
        {
            Brain.Init(isChild);
            Eyes.Init();
            Legs.Init();
            Stomach.Init();
            Uterus.Init();
            Weapon.Init();
            
            Genome = new Genome(Brain, DNA);
        }

        public float AgeLevel()
        {
            return (float) Age / DNA.LifeExpectation[0];
        }

        private float[] PerceiveInputs()
        {
            float[] inputs = new float[DNA.NumRaycasts[0]*7 + 7];
            
            float[] raycasts = Eyes.LookAround(characterTransform, food, species);
            for (int i = 0; i < raycasts.Length; i++)
            {
                inputs[i] = raycasts[i];
            }

            inputs[DNA.NumRaycasts[0]*7] = Eyes.FoodDensity(food);
            inputs[DNA.NumRaycasts[0]*7 + 1] = Eyes.AnimalDensity(species);

            // Current calories relative to max calories
            inputs[DNA.NumRaycasts[0]*7 + 2] = Stomach.HungerInput();
            
            // Current age relative to SexualMaturity
            inputs[DNA.NumRaycasts[0]*7 + 3] = Uterus.SexualMaturityLevel();
            
            // Current Reproduction Energy relative to the needed one
            inputs[DNA.NumRaycasts[0]*7 + 4] = Uterus.ReproductionEnergyLevel();
            
            // last movement direction
            inputs[DNA.NumRaycasts[0] * 7 + 5] = lastDirection.x;
            inputs[DNA.NumRaycasts[0] * 7 + 6] = lastDirection.y;
            
            return inputs;
        }

        private Action EvaluateAction(float[] outputs)
        {
            float[] actionInputs = outputs.Skip(3).Take(actionSpace).ToArray();
            int index = Array.IndexOf(actionInputs, actionInputs.Max());
            switch (index)
            {
                case 0:
                    return Action.Eat;
                case 1:
                    return Action.Chill;
                case 2:
                    if (Uterus.CanReproduce())
                        return Action.Reproduce;
                    else
                        return Action.Chill;
                case 3:
                    return Action.Fight;
                default:
                    return Action.Chill;
            }
        }
        

        private void KillIfDead()
        {
            EvaluateFitness();
            int causeOfDeath = CauseOfDeath();
            if (causeOfDeath != (int) Enums.CauseOfDeath.other)
            {
                int timeOfDeath = Mathf.FloorToInt(Time.time * EnvironmentData.TimeSpeed / 60f);
                Debug.LogWarning($"[{gameObject.name}] {timeOfDeath} R.I.P: Died the age of {Age}, " +
                                 $"Fitness {Fitness}, " +
                                 $"Reproduced {Uterus.GetChildCountSolo()} times solo, " +
                                 $"Reproduced {Uterus.GetChildCountMutual()} times mutual, " +
                                 $"Found {EatenTrees} Trees. Killed {EatenAnimals} Animals." +
                                 $"Cause of Death: {(CauseOfDeath)Enum.Parse(typeof(CauseOfDeath), causeOfDeath.ToString())}");

                Died.Invoke(this);
                Plot.SaveData(Key, Population, Generation,Color,Age, EatenTrees, EatenAnimals,
                    Uterus.GetChildCountSolo(), Uterus.GetChildCountMutual(),timeOfDeath, causeOfDeath, Fitness, NewLevel, DNA);
                //StartCoroutine(DestroyAfterAni());
                Destroy(gameObject);
            }
            
        }
        
        IEnumerator DestroyAfterAni()
        {
            if (IsDrown)
            {
                isInDrownAni = true;
                CharacterController.enabled = false;
                GetComponentInChildren<CapsuleCollider>().enabled = false;
                float timeInterval = 4f / EnvironmentData.TimeSpeed;
                yield return new WaitForSeconds(timeInterval);
            }
            Destroy(gameObject);
            
        }
        
        public IEnumerator AnimationFreeze(int action)
        {
            isInAnimationFreeze = true;
            Debug.LogError($"{name} animation freeze {action} set to true {isInAnimationFreeze}. is killed? {IsKilled}");
            Legs.SetMoveDirection(Vector2.zero, 0);
            Legs.Animator?.SetInteger("Action", action);
            if (action != (int) Action.Reproduce) Hearts.SetActive(false);
            else Hearts.SetActive(true);
            float timeInterval = 10f / EnvironmentData.TimeSpeed;
            yield return new WaitForSeconds(timeInterval);
            KillIfDead();
            isInAnimationFreeze = false;
            Debug.LogError($"{name} animation freeze {action} set to false {isInAnimationFreeze}");
            Legs.Animator?.SetInteger("Action", (int) Action.Chill);
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

            if (IsDrown)
            {
                return (int) Enums.CauseOfDeath.drown;
            }

            if (IsKilled)
            {
                return (int) Enums.CauseOfDeath.killed;
            }
            
            return (int) Enums.CauseOfDeath.other;
            
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int) Layer.Water)
            {
                if (!EnvironmentData.EndlessWorld)
                {
                    IsDrown = true;
                }

                else
                {
                    characterTransform.SetPositionAndRotation(characterTransform.position -
                                                                              characterTransform.forward * EnvironmentData.MapSize * 10, characterTransform.rotation);
                    for (int i = 0; i < 10; i++)
                    {
                        if (Physics.OverlapSphereNonAlloc(characterTransform.position, 40, colliderBuffer,
                            1 << (int) Layer.Water) >= 1)
                        {
                            characterTransform.SetPositionAndRotation(characterTransform.position +
                                                                      characterTransform.forward *
                                                                      (EnvironmentData.MapSize / 2f),
                                characterTransform.rotation);
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    Physics.SyncTransforms();
                }
                
            }
            
        }

        public static string ArrayToString(float[] array)
        {
            // Convert the array elements to strings and join them with commas
            return "[" + string.Join(" , ", array) + "]";
        }


        public void UpdateColor(float hue)
        {
            if (materials == null) materials = gameObject.GetComponentInChildren<Renderer>().materials;
            materials[2].color = UnityEngine.Color.HSVToRGB(hue, 0.45f, 0.3f);
            materials[3].color = UnityEngine.Color.HSVToRGB(hue, 0.35f, 0.45f);
            int intHue = (int) (hue * 1000);
            UpdateInfo(Key, Population, Generation, intHue);
            Color = hue;
        }

        private void UpdateScale()
        {
            float scale = 1f;
            float variance = DNA.Weight[2] - DNA.Weight[1];
            scale += (DNA.Weight[0] - DNA.Weight[1]) / variance;
            characterTransform.localScale = new UnityEngine.Vector3(scale, scale, scale);
        }

        public bool CanBeAttacked()
        {
            if (IsKilled || CurrentAction == Action.Fight && isInAnimationFreeze) return false;
            else return true;
        }
    }
}