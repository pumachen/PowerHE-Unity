using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HoudiniWebClient
{

	public class HWCUtils
	{
		private const string host = "localhost";
		private const string protocol = "http";
		private const int port = 8008;

		[MenuItem("HWebServer/Inspect")]
		public static void CreateTerrainLOD()
		{
			EditorCoroutineUtility.StartCoroutineOwnerless(GetHDAHeader("terrain_lod_gen.hdalc"));
		}

		public static IEnumerator GetHDAHeader(string hdaName)
		{
			string path = Path.Combine("HDALibrary", hdaName);
			UriBuilder uriBuilder = new UriBuilder(protocol, host, port, path);

			using (UnityWebRequest request = UnityWebRequest.Get(uriBuilder.Uri))
			{
				request.SetRequestHeader("User-Agent", "HWebClient");
				yield return request.SendWebRequest();
				HDAFormWindow.Open(uriBuilder.Uri, request.downloadHandler.text);
			}
		}
	}
}
