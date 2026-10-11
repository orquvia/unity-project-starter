using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Orquvia.Starter.Tests.Presentation
{
    // View の組み立て用。シーンを使わずに Button や Text を生成し、private な [SerializeField] に設定する。
    internal static class TestUi
    {
        public static T CreateChild<T>(GameObject parent, string name)
            where T : Component
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            return child.AddComponent<T>();
        }

        public static void Assign(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(propertyName)
                ?? throw new ArgumentException($"{target.GetType().Name} に {propertyName} がありません");

            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
