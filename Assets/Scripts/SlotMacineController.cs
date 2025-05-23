using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineController : MonoBehaviour
{
	private RollersData data;
	private SlotMachineAnimator animator;
	private List<List<Reels>> currentRollers = new List<List<Reels>>();
	public event Action<int, int> OnWinningDataUpdated;
	
	private void Start()
	{
		data = new RollersData();
		animator = gameObject.AddComponent<SlotMachineAnimator>();
		animator.Init(data.rollers, this);
		currentRollers = data.rollers;
	}
	public void Spin()
	{
		float randomTime = UnityEngine.Random.Range(2f, 4f);
		int totalRotations = PrecomputeIndexes(randomTime);
		animator.AnimateSpin(randomTime, totalRotations); //We could set up an inteface if more decoupling needed
		UpdateCurrentRollers(totalRotations);
		(bool, bool)hasWon = CheckWin();
	}
	#region SpinLogicController
	private int PrecomputeIndexes(float spinDuration)
	{
		float speedRotation = 10f;
		float easeFactor = 1 - Mathf.Exp(-spinDuration);
		int totalRotations = (int)Mathf.Floor(spinDuration * speedRotation * easeFactor);
		return totalRotations;
	}
	private void UpdateCurrentRollers(int rotations)
	{
		for (int i = 0; i < currentRollers.Count; i++)
		{
			int columnLength = currentRollers[i].Count;
			List<Reels> newOrder = new List<Reels>(columnLength);
			for (int j = 0; j < columnLength; j++)
			{
				newOrder.Add(currentRollers[i][(j + rotations) % columnLength]);
			}
			currentRollers[i] = newOrder;
		}
	}
	public (bool, bool) CheckWin()
	{
		bool customPatternWin = false;
		foreach ((int, int)[] pattern in data.winningPatterns)
		{
			if (CheckPattern(pattern))
				customPatternWin = true;
		}
		bool rowWin = CheckRows();

		return (customPatternWin, rowWin);
	}
	private bool CheckPattern((int col, int row)[] pattern)
	{
		Reels first = currentRollers[pattern[0].col][pattern[0].row];

		foreach (var (col, row) in pattern)
		{
			if (currentRollers[col][row] != first)
				return false;
		}
		return true;
	}
	private bool CheckRows()
	{
		int rowCount = 3;
		int colCount = currentRollers.Count;

		for (int row = 0; row < rowCount; row++)
		{
			Reels first = currentRollers[0][row];
			int matchCount = 1;

			for (int col = 1; col < colCount; col++)
			{
				if (currentRollers[col][row] == first)
					matchCount++;
				else
					break;

				if (matchCount >= 2)
				{
					Debug.Log("Won " + data.winRewards[first][matchCount] + " Credits"); //Replace with Animator credits win show function
					OnWinningDataUpdated?.Invoke(row, matchCount);
					return true;
				}
			}
		}
		return false;
	}
	#endregion
}
