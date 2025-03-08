﻿// Copyright (C) 2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace BubbleShooterKit
{
	/// <summary>
	/// An abstraction over Unity's audio source component. It is used
	/// in a sound pool that avoids having to dynamically create and
	/// destroy sounds dynamically.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class SoundFx : MonoBehaviour
	{
		private AudioSource audioSource;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		public void Play(AudioClip clip)
		{
			if (clip != null)
			{
				audioSource.clip = clip;
				audioSource.Play();
				Invoke(nameof(KillSoundFx), clip.length + 0.1f);
			}
		}

		private void KillSoundFx()
		{
			GetComponent<PooledObject>().Pool.ReturnObject(gameObject);
		}
	}
}
