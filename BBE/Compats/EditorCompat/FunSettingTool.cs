using BaldiLevelEditor;
using BBE.CustomClasses;
using BBE.Extensions;
using MTM101BaldAPI;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.Compats.EditorCompat
{
    public class FunSettingTool : ObjectTool
    {
        public static List<KeyValuePair<IntVector2, FunSettingTool>> all = new List<KeyValuePair<IntVector2, FunSettingTool>>();
        class FunSettingEditor : MonoBehaviour
        {
            public FunSetting funSetting;
            void Start()
            {
                funSetting.Set(true);
            }
        }
        public static void CreateVisual(FunSetting fun)
        {
            // I copied it from expanded level editor, I was too lazy, sorry
            GameObject funSettingObjectThing = new GameObject();
            funSettingObjectThing.transform.localScale = new Vector3(15f, 15f, 15f);
            funSettingObjectThing.AddComponent<BoxCollider>().size = new Vector3(0.2f, 0.2f, 0.2f);
            GameObject funSettingActualObject = new GameObject();
            funSettingActualObject.transform.SetParent(funSettingObjectThing.transform);
            SpriteRenderer spriteRenderer = funSettingActualObject.AddSpriteRender(fun.EditorIcon.ResizeSprite(20));
            spriteRenderer.material = new Material(ObjectCreators.SpriteMaterial);
            funSettingObjectThing.ConvertToPrefab(true);
            BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>(fun.UniqueName, funSettingObjectThing, Vector3.up * 5f));
            GameObject funSettingObject = new GameObject();
            funSettingObject.ConvertToPrefab(true);
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add(fun.UniqueName, funSettingObject);
            funSettingObject.name = fun.UniqueName;
            funSettingObject.AddComponent<FunSettingEditor>().funSetting = fun;
        }

        public FunSetting funSetting;
        public override Sprite editorSprite => funSetting.EditorIcon;

        public FunSettingTool(FunSetting fun) : base(fun.Name)
        {
            funSetting = fun;
        }
        public void PlacementFail()
        {
            PlusLevelEditor.Instance.audMan.PlaySingle(BaldiLevelEditorPlugin.Instance.assetMan.Get<SoundObject>("Elv_Buzz"));
        }
        public override void OnDrop(IntVector2 vector)
        {
            if (all.Exists(x => x.Key == vector))
            {
                PlacementFail();
                return;
            }
            if (all.Exists(x => x.Value.funSetting == funSetting))
            {
                PlacementFail();
                return;
            }
            all.Add(vector, this);
            base.OnDrop(vector);
        }
        public void Delete()
        {

        }
    }
}
