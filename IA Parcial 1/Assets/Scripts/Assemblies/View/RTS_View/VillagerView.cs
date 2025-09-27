using System;
using RTS.Model;
using UnityEngine;

namespace RTS.View
{
    public class VillagerView : MonoBehaviour
    {
        public VillagerAgent villagerAgent;

        [SerializeField] private GameObject foodFill;

        private void Start()
        {
            villagerAgent.onFoodUpdate += UpdateFood;
        }

        public void UpdateFood()
        {
            foodFill.transform.localScale = new Vector3(villagerAgent.CurrentFood / villagerAgent.MaxFood, foodFill.transform.localScale.y, 1);
        }
    }
}