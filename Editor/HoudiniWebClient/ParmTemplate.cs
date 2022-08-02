using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace HoudiniWebClient
{
	[Serializable]
	public class ParmTemplate
	{
		public string name;
		public string label;
		[JsonConverter(typeof(StringEnumConverter))]
		public ParmTemplateType type;
		[JsonConverter(typeof(StringEnumConverter))]
		public ParmData dataType;
		public int numComponents;
		// public string namingScheme;
		[JsonConverter(typeof(StringEnumConverter))]
		public ParmLook look;
		public string help;
		public bool isHidden;
		public bool isLabelHidden;
		public bool joinsWithNext;
		// public string disableWhen;
		// public string conditionals;
		public Dictionary<string, string> tags;
		// public string scriptCallback;
		// public string scriptCallbackLanguage;
	}

	[Serializable]
	public class FloatParmTemplate : ParmTemplate
	{
		public float[] defaultValue;
		// public string[] defaultExpression;
		// public string[] defaultExpressionLanguage;
		public float minValue;
		public float maxValue;
		public bool minIsStrict;
		public bool maxIsStrict;
	}
	
	[Serializable]
	public class IntParmTemplate : ParmTemplate
	{
		public int[] defaultValue;
		// public string[] defaultExpression;
		// public string[] defaultExpressionLanguage;
		public int minValue;
		public int maxValue;
		public bool minIsStrict;
		public bool maxIsStrict;
		//public string itemGeneratorScript;
		//public string itemGeneratorScriptLanguage;
		[JsonConverter(typeof(StringEnumConverter))]
		public MenuType menuType;
		public bool menuUseToken;
	}
	
	[Serializable]
	public class StringParmTemplate : ParmTemplate
	{
		public string[] defaultValue;
		// public string[] defaultExpression;
		// public string[] defaultExpressionLanguage;
		[JsonConverter(typeof(StringEnumConverter))]
		public StringParmType stringType;
		[JsonConverter(typeof(StringEnumConverter))]
		public FileType fileType;
		public string[] menuItems;
		public string[] menuLabels;
		//public string[] iconNames;
		//public string itemGeneratorScript;
		//public string itemGeneratorScriptLanguage;
		[JsonConverter(typeof(StringEnumConverter))]
		public MenuType menuType;
		public bool menuUseToken;
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/parmData.html
	public enum ParmData
	{
		Int, 
		Float, 
		String, 
		Ramp
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/stringParmType.html
	public enum StringParmType
	{
		Regular,
		FileReference, 
		NodeReference,
		NodeReferenceList
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/parmLook.html
	public enum ParmLook
	{
		Regular, 
		Logarithmic, 
		Angle, 
		Vector, 
		ColorSquare, 
		HueCircle, 
		CRGBAPlaneChooser
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/fileType.html
	public enum FileType
	{
		Any,
		Image,
		Geometry,
		Ramp,
		Capture,
		Clip,
		Lut,
		Cmd,
		Midi,
		I3d,
		Chan,
		Sim,
		SimData,
		Hip,
		Otl,
		Dae,
		Gallery,
		Directory,
		Icon,
		Ds,
		Alembic,
		Psd,
		LightRig,
		Gltf,
		Movie,
		Fbx,
		Usd,
		Sqlite,
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/folderType.html
	public enum FolderType
	{
		Collapsible, 
		Simple, 
		Tabs, 
		RadioButtons, 
		MultiparmBlock, 
		ScrollingMultiparmBlock, 
		TabbedMultiparmBlock, 
		ImportBlock
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/parmTemplateType.html
	public enum ParmTemplateType
	{
		Int, 
		Float, 
		String, 
		Toggle, 
		Menu, 
		Button, 
		FolderSet, 
		Separator, 
		Label, 
		Remap, 
		Data
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/dataParmType.html
	public enum DataParmType
	{
		Geometry, 
		KeyValueDictionary
	}

	// https://www.sidefx.com/docs/houdini/hom/hou/menuType.html
	public enum MenuType
	{
		Normal, 
		Mini, 
		ControlNextParameter, 
		StringReplace, 
		StringToggle
	}
}