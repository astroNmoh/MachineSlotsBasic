using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Collections;

public class SlotMachineAnimator : MonoBehaviour
{
	[SerializeField] private SlotMachineController slotController; //We could setup an interface to decouple more
	[SerializeField] private GameObject[] reelBGs = new GameObject[5]; //Create this dinamically if needed, just keeping it simple here
	private Dictionary<Reels, Sprite> reelSprites = new Dictionary<Reels, Sprite>();
	
	public void Init()
	{
		foreach (Reels reel in Enum.GetValues(typeof(Reels)))
		{
			int reelIndex = (int)reel + 1;
			Texture2D texture = Resources.Load<Texture2D>("Figures/" + reelIndex);
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			reelSprites.Add(reel, sprite);
		}

		PopulateColumns();
	}
	public void PopulateColumns()
	{
		float imageScale = 2.5f;
		for (int i = 0; i < slotController.slotMachine.Count; i++)
		{
			var list = slotController.slotMachine[i];
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

	public IEnumerator SpinColumn(float duration, int totalRotations, float columnInterval = 0.5f)
	{
		for (int i = 0; i < reelBGs.Length; i++)
		{
			StartCoroutine(MoveReelsBottomToTop(reelBGs[i], duration, totalRotations));
			yield return new WaitForSeconds(columnInterval);
		}
	}
	private IEnumerator MoveReelsBottomToTop(GameObject column, float duration, int totalRotations)
	{
		int childCount = column.transform.childCount;
		Transform[] children = new Transform[childCount];
		
		for (int i = 0; i < childCount; i++)
		{
			children[i] = column.transform.GetChild(i);
		}

		// Move each child forward cyclically
		for (int i = 0; i < childCount; i++)
		{
			int newIndex = (i + 1) % childCount; // Cyclic shift
			children[i].SetSiblingIndex(newIndex);
		}
		/*
		for (int i = 0; i < 1; i++)
		{
			foreach (Transform reel in column.transform)
			{
				yield return new WaitForEndOfFrame();
				reel.SetAsLastSibling();
				Debug.Log(reel.name + " " + reel.GetSiblingIndex());
			}
			*/
		// Wait for the movement to complete
		yield return new WaitForSeconds(duration / totalRotations);
		//}
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
}