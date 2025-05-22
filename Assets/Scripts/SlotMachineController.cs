using UnityEngine;
using System.Collections.Generic;
using static Reels;
public enum Reels : sbyte { Bell, WaterM, Grape, Plum, Orange, Lemon, Berry }
public class SlotMachineController : MonoBehaviour
{
	[SerializeField] private SlotMachineAnimator slotMachineAnimator;
	public List<List<Reels>> slotMachine;
	    
  void Start()
  {
		//This could be handled by Json to separate the data, or a MachineSlotData class, but done here for faster sample building.
		slotMachine = new List<List<Reels>>
		{
			new List <Reels> { Orange, Bell, WaterM, Berry, Plum, Lemon, Grape, Plum, Bell, Bell, Orange, Grape, Lemon, Lemon},
			new List <Reels> { WaterM, Berry, Bell, Plum, Berry, Grape, Orange, Lemon, Lemon, Lemon, Berry, Lemon, Plum, Lemon, Berry},
			new List <Reels> { Grape, WaterM, Plum, Grape, Bell, Lemon, Berry, Bell, Lemon, Lemon, Orange, Orange, Grape},
			new List <Reels> { Lemon, Plum, Plum, Lemon, Grape, Orange, WaterM, WaterM, Bell, Berry, Berry, Lemon, Orange, Plum, Lemon},
			new List <Reels> { Grape, Berry, Bell, WaterM, Orange, Orange, Lemon, Plum, Orange, Lemon, Grape, Bell, WaterM, Berry}
		};
		slotMachineAnimator.Init();
	}

	public void Spin()
	{
		float randomTime = Random.Range(2f, 4f);
		int totalRotations = PrecomputeIndexes(randomTime);
		StartCoroutine(slotMachineAnimator.SpinColumn(randomTime, totalRotations));
	}

	private int PrecomputeIndexes(float spinDuration) 
	{
		float speedRotation = 10f;
		float easeFactor = 1 - Mathf.Exp(-spinDuration);
		int totalRotations = (int)Mathf.Floor(spinDuration * speedRotation * easeFactor);
		return totalRotations;
	}
}

public static class Utils
{
	public static float RandomSeconds()
	{
		return Random.Range(2f, 4f);
	}
}