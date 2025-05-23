using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineController : MonoBehaviour
{
	[SerializeField] private GameObject[] reelBGs = new GameObject[5]; //Create this dinamically if needed, just keeping it simple here
	private RollersData data;
	private Dictionary<Reels, Sprite> reelSprites = new Dictionary<Reels, Sprite>();
	private List<List<Reels>> currentRollers = new List<List<Reels>>();
	private int winningRow, winningStreak;
	
	private void Start()
	{
		data = new RollersData();
		currentRollers = data.rollers;
		PopulateReelSpritesDictionary();
		PopulateColumns();
	}
	public void Spin()
	{
		float randomTime = UnityEngine.Random.Range(2f, 4f);
		int totalRotations = PrecomputeIndexes(randomTime);
		StartCoroutine(AnimateSpinColumn(randomTime, totalRotations));
		UpdateCurrentRollers(totalRotations);
		(bool, bool)hasWon = CheckWin();

		if (hasWon.Item1)
		{
			StartCoroutine(HighlightReels(winningRow, winningStreak));
		}
	}
	private void PopulateReelSpritesDictionary()
	{
		foreach (Reels reel in Enum.GetValues(typeof(Reels)))
		{
			int reelIndex = (int)reel + 1;
			Texture2D texture = Resources.Load<Texture2D>("Figures/" + reelIndex);
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			reelSprites.Add(reel, sprite);
		}
	}
	private void PopulateColumns()
	{
		float imageScale = 2.5f;
		for (int i = 0; i < data.rollers.Count; i++)
		{
			var list = data.rollers[i];
			foreach (var reel in list)
			{
				GameObject imageHolder = new GameObject(reel.ToString());
				imageHolder.transform.SetParent(reelBGs[i].transform);
				imageHolder.transform.localScale *= imageScale;
				Image img = imageHolder.AddComponent<Image>();
				AssignSpriteToUI(img, reel);
			}
		}
	}
	private void AssignSpriteToUI(Image imageComponent, Reels reelType)
	{
		if (reelSprites.TryGetValue(reelType, out Sprite sprite))
		{
			imageComponent.sprite = sprite;
		}
		else
		{
			Debug.LogWarning($"No sprite found for {reelType}");
		}
	}
	private int PrecomputeIndexes(float spinDuration)
	{
		float speedRotation = 10f;
		float easeFactor = 1 - Mathf.Exp(-spinDuration);
		int totalRotations = (int)Mathf.Floor(spinDuration * speedRotation * easeFactor);
		return totalRotations;
	}
	private IEnumerator AnimateSpinColumn(float duration, int totalRotations, float columnInterval = 0.5f)
	{
		for (int i = 0; i < reelBGs.Length; i++)
		{
			StartCoroutine(MoveReelsUpToDown(reelBGs[i], duration, totalRotations));
			yield return new WaitForSeconds(columnInterval);
		}
	}
	private IEnumerator MoveReelsUpToDown(GameObject column, float duration, int totalRotations)
	{
		int childCount = column.transform.childCount;
		for (int j = 0; j < totalRotations; j++)
		{
			for (int i = 0; i < childCount; i++)
			{
				column.transform.GetChild(i).SetSiblingIndex(i + 1);
			}
			yield return new WaitForSeconds(duration / totalRotations);//add ease factor
		}
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

				if (matchCount >= 2) {
					Debug.Log(data.winRewards[first][matchCount]);//Print UI function with the data
					winningRow = row;
					winningStreak = matchCount;
					return true;
				}
			}
		}
		return false;
	}

	//We could adapt this function to highliht custom pattern images as well
	private IEnumerator HighlightReels(int row, int streak, float duration = 1.5f)
	{
		Image[] images = new Image[streak];
		for (int i = 0; i < streak; i++) 
		{
			images[i] = reelBGs[i].transform.GetChild(row).GetComponent<Image>();
			images[i].color = Color.red;
		}
		yield return new WaitForSeconds(duration);

		for (int i = 0; i < streak; i++)
		{
			images[i].color = Color.white;
		}
	}
}