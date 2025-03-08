﻿// Copyright (C) 2018 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterKit
{
	[Serializable]
	public class LevelRow : ScriptableObject
	{
		public List<TileInfo> Tiles;
	}
}
