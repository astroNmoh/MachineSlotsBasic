using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
public class SlotMachineAnimator : MonoBehaviour
{
	[SerializeField] private SlotMachineController slotController;
	private Dictionary<Reels, Sprite> reelSprites = new Dictionary<Reels, Sprite>();
	private List<Canvas> canvases = new List<Canvas>();

	public void Init()
	{
		foreach (Canvas can in gameObject.GetComponentsInChildren<Canvas>())
		{
			canvases.Add(can);
		}
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
		for (int i = 0; i < slotController.slotMachine.Count; i++)
		{
			var list = slotController.slotMachine[i];
			foreach (var reel in list)
			{
				GameObject imageHolder = new GameObject("ReelImage");
				imageHolder.transform.SetParent(canvases[i].transform);

				Image img = imageHolder.AddComponent<Image>();
				AssignSpriteToUI(img, reel);
			}
		}
	}

	public void AnimateColumn(float duration)
	{

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
