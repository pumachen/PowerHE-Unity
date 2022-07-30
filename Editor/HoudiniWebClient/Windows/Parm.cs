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
        string value { get; }
    }
    
    public class FloatParm : IHouParm
    {
        ParmTemplate IHouParm.template => template;
        string IHouParm.value => JsonConvert.SerializeObject(value);
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
    }
    public class IntParm : IHouParm
    {
        ParmTemplate IHouParm.template => template;
        string IHouParm.value => JsonConvert.SerializeObject(value);
        public IntParmTemplate template;
        public int[] value { get; private set; }
        public IntParm(IntParmTemplate template)
        {
            this.template = template;
            value = new int[template.numComponents];
            Array.Copy(template.defaultValue, value, template.numComponents);
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
        string IHouParm.value => JsonConvert.SerializeObject(value);
        public StringParmTemplate template;
        public string value { get; private set; }

        public StringParm(StringParmTemplate template)
        {
            this.template = template;
            value = template.defaultValue[0];
        }

        public void GUILayout()
        {
            value = EditorGUILayout.TextField(template.label, value);
        }
    }
}