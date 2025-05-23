using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;

public class SlotMachineController : MonoBehaviour
{
	[SerializeField] private GameObject[] reelBGs = new GameObject[5]; //Create this dinamically if needed, just keeping it simple here
	private RollersData data;
	private Dictionary<Reels, Sprite> reelSprites = new Dictionary<Reels, Sprite>();
	private List<List<Reels>> currentRollers = new List<List<Reels>>();
	
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
		int totalRotations = PrecomputeIndexes(randomTime); //We could do parallel processing of animation and results
		StartCoroutine(AnimateSpinColumn(randomTime, totalRotations));
		UpdateCurrentRollers(totalRotations);
		CheckRewards();
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
		
		AnnounceResults();
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
				Debug.Log(newOrder[j]);
			}
			currentRollers[i] = newOrder;
		}
	}
	private void CheckRewards()
	{
		
	}

	private void AnnounceResults()
	{

	}
}