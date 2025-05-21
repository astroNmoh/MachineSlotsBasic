using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
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
			//Debug.Log("address: " + ("Figures/" + reelIndex).ToString());
			Texture2D texture = Resources.Load<Texture2D>("Figures/" + reelIndex);
			//Debug.Log(texture);
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
				GameObject imageHolder = new GameObject("ReelImage");
				imageHolder.transform.SetParent(reelBGs[i].transform);
				imageHolder.transform.localScale *= imageScale;

				Image img = imageHolder.AddComponent<Image>();
				AssignSpriteToUI(img, reel);
			}
		}
	}

	public void SpinColumn(float duration)
	{
		MoveReelsBottomToTop();
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
	void MoveReelsBottomToTop()
	{
		Transform trans = reelBGs[0].transform;
		if (trans.childCount > 0)
		{
			Transform lastChild = trans.GetChild(trans.childCount - 1);
			lastChild.SetSiblingIndex(0);
		}
	}


}
