using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace HoudiniWebClient
{

	public class HDAFormWindow : EditorWindow
	{
		private Uri uri;
		private dynamic hdaHeader;
		private IHouParm[] parms;
		
		public static void Open(Uri uri, string hdaHeader)
		{
			HDAFormWindow window = CreateWindow<HDAFormWindow>();
			window.Init(uri, JsonConvert.DeserializeObject(hdaHeader));
			window.Show();
		}

		void Init(Uri uri, dynamic hdaHeader)
		{
			this.uri = uri;
			this.hdaHeader = hdaHeader;
			string title = hdaHeader.nodeType.description;
			titleContent = new GUIContent(title);
			var parmTemplates = hdaHeader.parmTemplateGroup.parmTemplates;
			parms = new IHouParm[parmTemplates.Count];
			for (int i = 0; i < parmTemplates.Count; ++i)
			{
				var parmTemplate = parmTemplates[i];
				switch (parmTemplate.ToObject<ParmTemplate>().dataType)
				{
					case (ParmData.Int):
					{
						parms[i] = new IntParm(parmTemplate.ToObject<IntParmTemplate>());
						break;
					}
					case (ParmData.Float):
					{
						parms[i] = new FloatParm(parmTemplate.ToObject<FloatParmTemplate>());
						break;
					}
					case (ParmData.String):
					{
						parms[i] = new StringParm(parmTemplate.ToObject<StringParmTemplate>());
						break;
					}
				}
			}
		}

		private void OnGUI()
		{
			foreach (var parm in parms)
			{
				parm.GUILayout();
			}

			if (GUILayout.Button("Submit"))
			{
				EditorCoroutineUtility.StartCoroutineOwnerless(Post());
			}
			
		}

		IEnumerator Post()
		{
			WWWForm form = new WWWForm();
			foreach (var parm in parms)
			{
				parm.RegisterFormData(form);
			}

			string downloadedFile = Path.Combine(Application.dataPath, "../Temp", "Tiles.zip");
			using (UnityWebRequest post = UnityWebRequest.Post(uri, form))
			{
				post.timeout = 300;
				post.downloadHandler = new DownloadHandlerFile(downloadedFile);
				yield return post.SendWebRequest();
			}

			string outputDir = EditorUtility.SaveFolderPanel("Output Dir", Application.dataPath, "Output");
			ZipFile.ExtractToDirectory(downloadedFile, outputDir);
			File.Delete(downloadedFile);
		}
	}
}