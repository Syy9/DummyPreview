using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Syy.Logics
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine.UI;
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class DummyPreview : MonoBehaviour
    {
        [SerializeField]
        public string _guid;

        Image _image = null;

        Image _dummyImage = null;
        Sprite _dummySprite = null;
        string _preGuid = null;

        void OnEnable()
        {
            if (!Application.isPlaying)
            {
                _image = GetComponent<Image>();

                if (_image == null)
                {
                    return;
                }

                var go = new GameObject();
                go.name = "DummyPreview(DontSave)";
                go.hideFlags = HideFlags.HideAndDontSave;
                _dummyImage = go.AddComponent<Image>();
            }
        }

        void OnDisable()
        {
            if (!Application.isPlaying)
            {
                if (_dummyImage != null && _dummyImage.gameObject != null)
                {
                    GameObject.DestroyImmediate(_dummyImage.gameObject);
                }
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                if (_dummyImage != null && _image != null)
                {
                    var rect = _dummyImage.transform as RectTransform;
                    rect.SetParent(transform);
                    rect.localPosition = Vector3.zero;
                    rect.localScale = Vector3.one;
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                    rect.SetAsFirstSibling();
                    if (_preGuid != _guid)
                    {
                        _preGuid = _guid;
                        _dummySprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(_guid));
                    }

                    if (_dummyImage.sprite != _dummySprite)
                    {
                        _dummyImage.sprite = _dummySprite;
                    }

                    if (_dummyImage.enabled != _image.enabled)
                    {
                        _dummyImage.enabled = _image.enabled;
                    }
                }
            }
        }
    }

    [CustomEditor(typeof(DummyPreview))]
    public class DummyPreviewEditor : Editor
    {
        DummyPreview _component = null;
        Sprite _prevSprite = null;
        GUIContent _guiContent = new GUIContent("Preview Source", "The sprite is not included in the build because it is a GUID reference.");

        void OnEnable()
        {
            _component = (DummyPreview)target;
            _prevSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(_component._guid));
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Works only during non-playing");
            }
            else
            {
                var sprite = (Sprite)EditorGUILayout.ObjectField(_guiContent, _prevSprite, typeof(Sprite), false);
                if (_prevSprite != sprite)
                {
                    _prevSprite = sprite;
                    _component._guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_prevSprite));
                    EditorUtility.SetDirty(_component);
                }
            }
        }
    }
#else
    public class DummyPreview : MonoBehaviour
    {
    }
#endif
}
