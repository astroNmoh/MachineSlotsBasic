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

	public IEnumerator SpinColumn(float duration, float columnInterval = 0.3f)
	{
		for (int i = 0; i < reelBGs.Length; i++)
		{
			StartCoroutine(MoveReelsBottomToTop(reelBGs[i], duration));
			yield return new WaitForSeconds(columnInterval);
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
	private IEnumerator MoveReelsBottomToTop(GameObject column, float duration)
	{
		float topPositionY = column.transform.GetChild(0).GetComponent<RectTransform>().rect.position.y;
		Debug.Log("Column nro " + column.name + " with top reel at y: " + topPositionY);
		Transform lastChild = column.transform.GetChild(column.transform.childCount - 1);

		// Move all elements down smoothly
		foreach (Transform child in column.transform)
		{
			float moveDistance = column.GetComponent<RectTransform>().rect.position.y;
			StartCoroutine(MoveUIElement(child, moveDistance, duration));
		}

		// Wait for the movement to complete
		yield return new WaitForSeconds(duration);

		// Reset last child to the top instantly
		lastChild.SetSiblingIndex(0);
		lastChild.position = new Vector3(lastChild.position.x, topPositionY, lastChild.position.z);
	}
	private IEnumerator MoveUIElement(Transform element, float targetY, float duration)
	{
		Vector3 target = element.localPosition - new Vector3(0, targetY, 0);
		float speed = 5f;
		Vector3 startPos = element.position;

		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
			float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
			element.position = Vector3.Lerp(startPos, target, t);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		element.position = target;
	}
}


/*
Transform trans = reelBGs[0].transform;
if (trans.childCount > 0)
{
	Transform lastChild = trans.GetChild(trans.childCount - 1);
	lastChild.SetSiblingIndex(0);
}*/