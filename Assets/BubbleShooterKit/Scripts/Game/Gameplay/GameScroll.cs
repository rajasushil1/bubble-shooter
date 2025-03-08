﻿// Copyright (C) 2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// This class is responsible for scrolling the level as appropriate based on
	/// the evolution of its contained bubbles.
	/// </summary>
	public class GameScroll : MonoBehaviour
	{
		[SerializeField]
		private GameScreen gameScreen = null;

		private int prevLevelGround;
		private bool isScrollDisabled;
		private Coroutine updateScrollCoroutine;

		private Level level;
		private float tileHeight;
		private List<List<Vector2>> tilePositions;
		private GameObject topLine;
		private List<GameObject> leaves;

		public void SetGameInfo(Level lvl, float tileH, List<List<Vector2>> positions, GameObject line, List<GameObject> levelLeaves)
		{
			level = lvl;
			tileHeight = tileH;
			tilePositions = positions;
			topLine = line;
			leaves = levelLeaves;

			prevLevelGround = level.GetGround();
			PlayerPrefs.SetFloat("scrolled_height", 0f);
		}

		public void Reset()
		{
			isScrollDisabled = false;
		}

		public void PerformScroll()
		{
			if (updateScrollCoroutine != null)
				StopCoroutine(updateScrollCoroutine);

			updateScrollCoroutine = StartCoroutine(UpdateScrollAsync());
		}

		private void UpdateScroll()
		{
			if (!isScrollDisabled)
			{
				var newGround = level.GetGround();
				var groundDiff = newGround - prevLevelGround;
				ScrollLevel(groundDiff);
				prevLevelGround = newGround;
			}
			else
			{
				gameScreen.UnlockInput();
			}
		}

		private void ScrollLevel(int rows)
		{
			var verticalIncrement = PlayerPrefs.GetFloat("scrolled_height");
			verticalIncrement += rows * tileHeight * GameplayConstants.TileHeightMultiplier;
			PlayerPrefs.SetFloat("scrolled_height", verticalIncrement);

			for (var i = 0; i < tilePositions.Count; i++)
			{
				var row = tilePositions[i];
				for (var j = 0; j < row.Count; j++)
				{
					var newPos = row[j];
					newPos.y += rows * tileHeight * GameplayConstants.TileHeightMultiplier;
					row[j] = newPos;
				}
			}

			var temptativePosY = tilePositions[0][0].y;

			var topPivot = new Vector2(0, Camera.main.pixelHeight * GameplayConstants.TopPivotHeight);
			var topPivotPos = Camera.main.ScreenToWorldPoint(topPivot);
			if (temptativePosY <= topPivotPos.y)
			{
				if (!isScrollDisabled)
				{
					FixBubblePositions(temptativePosY);
					isScrollDisabled = true;
				}
			}
			else
			{
				if (isScrollDisabled)
				{
					isScrollDisabled = false;
				}

				foreach (var row in level.Tiles)
				{
					foreach (var tile in row)
					{
						if (tile != null)
						{
							var newPos = tile.transform.position;
							newPos.y += rows * tileHeight * GameplayConstants.TileHeightMultiplier;
							tile.transform.DOMove(newPos, GameplayConstants.LevelScrollSpeed);
						}
					}
				}

				var pos = topLine.transform.position;
				pos.y += rows * tileHeight * GameplayConstants.TileHeightMultiplier;
				var seq = DOTween.Sequence();
				seq.Append(topLine.transform.DOMove(pos, GameplayConstants.LevelScrollSpeed));
				seq.AppendCallback(gameScreen.UnlockInput);

				foreach (var leaf in leaves)
				{
					if (leaf != null)
					{
						pos = leaf.transform.position;
						pos.y += rows * tileHeight * GameplayConstants.TileHeightMultiplier;
						leaf.transform.DOMove(pos, GameplayConstants.LevelScrollSpeed);
					}
				}
			}
		}

		private void FixBubblePositions(float temptativePosY)
		{
			var topPivot = new Vector2(0, Camera.main.pixelHeight * GameplayConstants.TopPivotHeight);
			var topPivotPos = Camera.main.ScreenToWorldPoint(topPivot);

			var temptativePosY2 = topPivotPos.y;
			var idx = 0;
			foreach (var row in level.Tiles)
			{
				foreach (var tile in row)
				{
					if (tile != null)
					{
						var newPos = tile.transform.position;
						newPos.y = topPivotPos.y - idx * tileHeight * GameplayConstants.TileHeightMultiplier;
						tile.transform.DOMove(newPos, GameplayConstants.LevelScrollSpeed);
					}
				}

				++idx;
			}

			var pos = topLine.transform.position;
			pos.y = temptativePosY2 + (tileHeight * 0.6f);
			var seq = DOTween.Sequence();
			seq.Append(topLine.transform.DOMove(pos, GameplayConstants.LevelScrollSpeed));
			seq.AppendCallback(gameScreen.UnlockInput);

			foreach (var leaf in leaves)
			{
				if (leaf != null)
				{
					pos = leaf.transform.position;
					pos.y = temptativePosY2 + tileHeight;
					leaf.transform.DOMove(pos, GameplayConstants.LevelScrollSpeed);
				}
			}

			var verticalIncrement = PlayerPrefs.GetFloat("scrolled_height");
			verticalIncrement += topPivotPos.y - temptativePosY;
			PlayerPrefs.SetFloat("scrolled_height", verticalIncrement);

			for (var i = 0; i < tilePositions.Count; i++)
			{
				var row = tilePositions[i];
				for (var j = 0; j < row.Count; j++)
				{
					var newPos = row[j];
					newPos.y += topPivotPos.y - temptativePosY;
					row[j] = newPos;
				}
			}
		}

		private IEnumerator UpdateScrollAsync()
		{
			gameScreen.LockInput();
			yield return new WaitForSeconds(0.5f);
			UpdateScroll();
		}
	}
}
