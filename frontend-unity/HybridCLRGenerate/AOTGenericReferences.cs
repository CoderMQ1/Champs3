using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Cinemachine.dll",
		"DOTween.dll",
		"LitJson.dll",
		"Mirror.dll",
		"ProtoBuf-Net.dll",
		"QFramework.CoreKit.dll",
		"QFramework.dll",
		"System.Core.dll",
		"System.dll",
		"UIKit.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.UI.dll",
		"YooAsset.dll",
		"com.unity-common.core.dll",
		"mscorlib.dll",
	};

	public void RefMethods()
	{
	}
}