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
		float topPositionY = column.transform.GetChild(0).GetComponent<RectTransform>().localPosition.y;
		Debug.Log("Column nro " + column.name + " with top reel at y: " + topPositionY);
		RectTransform lastChildRTrans = column.transform.GetChild(column.transform.childCount - 1).GetComponent<RectTransform>();
		Transform firstChild = column.transform.GetChild(0);

		// Move all elements down smoothly
		foreach (Transform reel in column.transform)
		{
			//float moveDistance = column.transform.GetChild(column.transform.childCount -1).GetComponent<RectTransform>().localPosition.y;
			float speedRotation = -1000;
			StartCoroutine(MoveUIReel(reel, speedRotation, duration, lastChildRTrans.localPosition));
		}

		// Wait for the movement to complete
		yield return new WaitForSeconds(duration);

    List<Transform> reelsToMove = new List<Transform>();
    foreach (Transform reel in column.transform)
		{

    }

		foreach (Transform reel in reelsToMove)
		{
      reel.SetSiblingIndex(column.transform.childCount - 1);
    }

  }
  private IEnumerator MoveUIReel(Transform reel, float targetY, float duration, Vector3 lastColumnPos)
	{
		Vector3 target = new Vector3(reel.GetComponent<RectTransform>().localPosition.x,
			reel.GetComponent<RectTransform>().localPosition.y - targetY,
			reel.GetComponent<RectTransform>().localPosition.z);

		//Debug.Log("Original TargetY to move reel: " + target.y);
		Vector3 startPos = reel.GetComponent<RectTransform>().localPosition;

		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
      if (reel.GetComponent<RectTransform>().localPosition.y >= 500)
      {
				// Move reel instantly if exits mask
				float lastReelPos = reel.GetComponent<RectTransform>().localPosition.y;
        reel.GetComponent<RectTransform>().localPosition = lastColumnPos;

        //Debug.Log("moving reel to last post" + reel.GetComponent<RectTransform>().localPosition);

        // Update start position so Lerp continues from the new location
        float remainingDistance = target.y - lastReelPos - startPos.y;
        target = new Vector3(startPos.x, startPos.y - remainingDistance, startPos.z);
				//Debug.Log("New targetY: " + target.y);
        startPos = lastColumnPos;
      }
      float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
			reel.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPos, target, t);
			elapsedTime += Time.deltaTime;
      yield return null;
		}
		//reel.GetComponent<RectTransform>().position = target;
	}
}