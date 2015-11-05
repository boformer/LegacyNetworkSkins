using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkSkins.Pillars
{
    public class PillarCustomizerPanelFineRoadHeights : PillarCustomizerPanel
    {
        public override void Update()
        {
            base.Update();

            // Fine Road Heights Net Tool
            var netTool = ToolsModifierControl.GetCurrentTool<NetToolFine>();

            if (netTool != null && netTool.m_prefab != null)
            {
                if (netTool.m_prefab == selectedPrefab) return;
                selectedPrefab = netTool.m_prefab;

                var elevatedPrefab = PillarCustomizer.instance.GetElevatedPrefab(selectedPrefab);
                if (elevatedPrefab == selectedPrefabElevated) return;
                selectedPrefabElevated = elevatedPrefab;

                if (PopulateDropDown(PillarType.BRIDGE_PILLAR) || PopulateDropDown(PillarType.MIDDLE_PILLAR))
                {
                    isVisible = true;
                    return;
                }
            }

            if (isVisible) 
            { 
                isVisible = false;
                selectedPrefab = null;
                selectedPrefabElevated = null;
            }
        }
    }
}
