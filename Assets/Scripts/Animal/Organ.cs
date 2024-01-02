using UnityEngine;
using Util;

namespace Animal
{
    public class Organ : TimeBasedBehaviour
    {
        [SerializeField] protected AnimalController animalController;
        
        public virtual void Init(bool isChild = false){}
    }
}