namespace NetworkSkins.Pillars
{
    public class PillarCustomizerPanelVanilla : PillarCustomizerPanel
    {
        public override void Update()
        {
            base.Update();

            // Vanilla Net Tool
            var netTool = ToolsModifierControl.GetCurrentTool<NetTool>();
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
