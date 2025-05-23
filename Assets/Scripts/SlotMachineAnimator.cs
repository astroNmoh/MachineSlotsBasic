using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineAnimator : MonoBehaviour
{
	private GameObject[] reelBGs = new GameObject[5]; //Create this dinamically if needed, just keeping it simple here
	private Button spinButton;
	private Sprite[] spinButtonSprites = new Sprite[2];
	private SlotMachineController machineController;
	private Dictionary<Reels, Sprite> reelSprites = new Dictionary<Reels, Sprite>();
	private int winningRow, winningStreak;
	public void Init(List<List<Reels>> initRolleresData, SlotMachineController sMachineController)
  {
		this.machineController = sMachineController;
		this.machineController.OnWinningDataUpdated += HandleWin;

		for (int i = 0; i < reelBGs.Length; i++)
		{
			reelBGs[i] = transform.GetChild(0).transform.GetChild(i).gameObject;
		}
		spinButton = GetComponentInChildren<Button>();

		PopulateReelSpritesDictionary();
		PopulateColumns(initRolleresData);
		CreateButtonSprites();
	}
	public void AnimateSpin(float randomTime, int totalRotations)
	{
		winningStreak = 0;
		float columnIntervalWait = 0.5f;
		StartCoroutine(AnimateSpinColumn(randomTime, totalRotations, columnIntervalWait));
		StartCoroutine(UpdateButtonUI(randomTime + columnIntervalWait * reelBGs.Length));
	}
	#region SetUPUI
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
	private void PopulateColumns(List<List<Reels>> initialRollersData)
	{
		float imageScale = 2.5f;
		for (int i = 0; i < initialRollersData.Count; i++)
		{
			var list = initialRollersData[i];
			foreach (var reel in list)
			{
				GameObject imageHolder = new GameObject(reel.ToString());
				imageHolder.transform.SetParent(reelBGs[i].transform);
				imageHolder.transform.localScale *= imageScale;
				Image img = imageHolder.AddComponent<Image>();
				AssignReelSpriteToImage(img, reel);
			}
		}
	}
	private void AssignReelSpriteToImage(Image imageComponent, Reels reelType)
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
	private void CreateButtonSprites()
	{
		Texture2D[] textures = { Resources.Load<Texture2D>("spin_disable"), 
			Resources.Load<Texture2D>("spin_normal") };

		// Debug if any texture is missing
		for (int i = 0; i < textures.Length; i++)
		{
			if (textures[i] == null)
			{
				Debug.LogError($"Texture at index {i} is null! Check file name and location.");
			}
		}

		for (int i = 0; i < textures.Length; i++)
		{
			spinButtonSprites[i] = Sprite.Create(
			textures[i],
			new Rect(0, 0, textures[i].width, textures[i].height),
			new Vector2(0.5f, 0.5f)
			);
		}
	}
	#endregion

	#region Animate
	private IEnumerator UpdateButtonUI(float duration)
	{
		float clickAnim = 0.3f;
		spinButton.interactable = false;
		yield return new WaitForSeconds(clickAnim);
		spinButton.image.sprite = spinButtonSprites[0];

		yield return new WaitForSeconds(duration - clickAnim);
		spinButton.image.sprite = spinButtonSprites[1];
		spinButton.interactable = true;
	}
	private IEnumerator AnimateSpinColumn(float duration, int totalRotations, float columnInterval)
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
		if (winningStreak > 0)
		{
			StartCoroutine(HighlightReels(winningRow, winningStreak));
		}
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
	private void HandleWin(int row, int streak)
	{
		winningRow = row;
		winningStreak = streak;
	}
	#endregion

	void OnDisable()
	{
		this.machineController.OnWinningDataUpdated -= HandleWin;
	}
}
