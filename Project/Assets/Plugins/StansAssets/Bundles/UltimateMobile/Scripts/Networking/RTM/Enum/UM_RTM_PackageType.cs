﻿using UnityEngine;
using System.Collections;

public enum UM_RTM_PackageType {
	Reliable = 0,
	Unreliable = 1
}

public static class UM_RTM_PackageTypeExtensions {
	public static GP_RTM_PackageType GetGPPackageType (this UM_RTM_PackageType type) {
		return (GP_RTM_PackageType) type;
	}

	public static GK_MatchSendDataMode GetGKPackageType(this UM_RTM_PackageType type) {
		return (GK_MatchSendDataMode) type;
	}
}
