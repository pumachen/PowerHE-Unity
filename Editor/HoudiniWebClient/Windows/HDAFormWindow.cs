using System;
using System.Collections;
using System.Collections.Generic;
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
				string dataType = parmTemplates[i].dataType;
				if (dataType.Contains("Int"))
					parms[i] = new IntParm(parmTemplates[i].ToObject<IntParmTemplate>());
				if (dataType.Contains("Float"))
					parms[i] = new FloatParm(parmTemplates[i].ToObject<FloatParmTemplate>());
				if (dataType.Contains("String"))
					parms[i] = new StringParm(parmTemplates[i].ToObject<StringParmTemplate>());
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
				form.AddField(parm.template.name, parm.value);
			}

			using (UnityWebRequest post = UnityWebRequest.Post(uri, form))
			{
				yield return post.SendWebRequest();
				Debug.Log(post.downloadHandler.text);
			}
		}
	}
}