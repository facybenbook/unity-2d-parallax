﻿using UnityEngine;
using System.Collections;

namespace Adnc.Parallax {
	public class ParallaxLayer : MonoBehaviour {
		public bool debug;

		[HideInInspector] public Vector3 originPos; // Captured original position in-case we want to reset it
		static float repeatPadding = 1f; // How many units to spawn a repeated tile in advance

		[SerializeField] bool repeat;
		SpriteRenderer repeatSprite;

		Rect rect = new Rect(); // Used to monitor the boundary of repeating parallax elements

		void Awake () {
			originPos = transform.position;
			Parallax2D.current.parallaxLayers.Add(this);
		}

		public void ParallaxSetup () {
			if (repeat) {
				repeatSprite = GetComponentInChildren<SpriteRenderer>();
				if (repeatSprite.gameObject == gameObject) repeatSprite = null; // Ignore parent element
				
				if (repeatSprite == null) {
					Debug.LogError("ParallaxLayer was marked as repeat, but no child element with SpriteRenderer was found to repeat. Disabling repeat.");
					repeat = false;
				} else {
					rect.width = repeatSprite.bounds.size.x;
					rect.height = repeatSprite.bounds.size.y;
					rect.center = repeatSprite.bounds.center;
					
					// Make sure the repeat element repeats up until it shows on the current viewing window
					while (IsNewRightBuddy()) {
						AddRightBuddy(repeatSprite);
					}

					while (IsNewLeftBuddy()) {
						AddLeftBuddy(repeatSprite);
					}
				}
			}
		}

		bool IsNewRightBuddy () {
			return Parallax2D.current.screen.rect.xMax + repeatPadding > rect.xMax;
		}

		bool IsNewLeftBuddy () {
			return Parallax2D.current.screen.rect.xMin - repeatPadding < rect.xMin;
		}

		Vector2 rectCenter;
		public void ParallaxUpdate (Vector2 change) {
			if (repeat) {
				// Reposition the rectangle to wrap the elements correctly
				rect.position += change;

				if (IsNewRightBuddy()) {
					AddRightBuddy(repeatSprite);
				}
				
				if (IsNewLeftBuddy()) {
					AddLeftBuddy(repeatSprite);
				}

				if (debug) {
					ScreenRect.DrawBoundary(rect, Color.gray);
				}
			}
		}

		public void AddRightBuddy (SpriteRenderer sprite) {
			GameObject go = Instantiate(sprite.gameObject) as GameObject;
			go.transform.SetParent(transform);

			// Set position
			Vector3 pos = rect.center;
			pos.x = rect.xMax + sprite.bounds.extents.x;
			go.transform.position = pos;

			// Update wrapping rectangle
			rect.xMax += sprite.bounds.size.x;
		}

		public void AddLeftBuddy (SpriteRenderer sprite) {
			GameObject go = Instantiate(sprite.gameObject) as GameObject;
			go.transform.SetParent(transform);
			
			// Set position
			Vector3 pos = rect.center;
			pos.x = rect.xMin - sprite.bounds.extents.x;
			go.transform.position = pos;
			
			// Update wrapping rectangle
			rect.xMin -= sprite.bounds.size.x;
		}
	}
}

