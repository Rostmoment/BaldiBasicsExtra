using BaldiLevelEditor;
using BBE.CustomClasses;
using System;
using System.Collections.Generic;
using System.Text;
using static BBE.Compats.EditorCompat.EditorPatches;

namespace BBE.Compats.EditorCompat
{
    class EditorCompat : BaseCompat
    {
        public override string GUID => "mtm101.rulerp.baldiplus.leveleditor";
        public override void Postfix()
        {
            base.Postfix();
            foreach (FunSetting fun in FunSetting.GetAll())
            {
                if (fun.EditorIcon == null) continue;
                FunSettingTool.CreateVisual(fun);
            }
        }
    }
}
