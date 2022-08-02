using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace HoudiniWebClient
{
    public interface IHouParm
    {
        ParmTemplate template { get; }
        void GUILayout();

        void RegisterFormData(WWWForm form);
    }
    
    public class FloatParm : IHouParm
    {
        ParmTemplate IHouParm.template => template;
        public FloatParmTemplate template;
        public float[] value { get; private set; }

        public FloatParm(FloatParmTemplate template)
        {
            this.template = template;
            value = new float[template.numComponents];
            Array.Copy(template.defaultValue, value, template.numComponents);
        }

        public void GUILayout()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(template.label);
            for (int i = 0; i < template.numComponents; ++i)
            {
                value[i] = EditorGUILayout.FloatField(value[i]);
            }
            EditorGUILayout.EndHorizontal();
        }

        public void RegisterFormData(WWWForm form)
        {
            form.AddField(template.name, JsonConvert.SerializeObject(value));
        }
    }
    public class IntParm : IHouParm
    {
        ParmTemplate IHouParm.template => template;
        public IntParmTemplate template;
        public int[] value { get; private set; }
        public IntParm(IntParmTemplate template)
        {
            this.template = template;
            value = new int[template.numComponents];
            Array.Copy(template.defaultValue, value, template.numComponents);
        }
        public void RegisterFormData(WWWForm form)
        {
            form.AddField(template.name, JsonConvert.SerializeObject(value));
        }

        public void GUILayout()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(template.label);
            for (int i = 0; i < template.numComponents; ++i)
            {
                value[i] = EditorGUILayout.IntField(value[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    public class StringParm : IHouParm
    {
        ParmTemplate IHouParm.template => template;
        public StringParmTemplate template;

        private Texture2D texture;
        public string value { get; private set; }

        public StringParm(StringParmTemplate template)
        {
            this.template = template;
            value = template.defaultValue[0];
        }

        public void GUILayout()
        {
            switch (template.stringType)
            {
                case StringParmType.FileReference:
                {
                    switch (template.fileType)
                    {
                        case FileType.Geometry:
                        case FileType.Fbx:
                        case FileType.Usd:
                        {
                            EditorGUILayout.LabelField("Geometry parameters are not supported");
                            break;
                        }
                        case FileType.Image:
                        {
                            texture = EditorGUILayout.ObjectField(template.label, texture, typeof(Texture2D), true) as Texture2D;
                            value = texture == null ? "" : GUID.Generate().ToString();
                            break;
                        }
                    }
                    break;
                }
                default:
                {
                    value = EditorGUILayout.TextField(template.label, value);
                    break;
                }
            }
        }

        public void RegisterFormData(WWWForm form)
        {
            if (template.stringType == StringParmType.FileReference 
                && template.fileType == FileType.Image 
                && texture != null)
            {
                form.AddBinaryData(template.name, texture.EncodeToPNG(), value);
            }
            else
            {
                form.AddField(template.name, value);
            }
        }
    }
}