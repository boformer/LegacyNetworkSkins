using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkSkins
{
    class Test
    {
        void test() 
        {
            var outerPadding = 5;
            var layoutPadding = 10;

            var panel = UIView.GetAView().AddUIComponent(typeof(UIPanel)) as UIPanel;
            panel.backgroundSprite = "SubcategoriesPanel";
            panel.name = "TestPanel";
            panel.padding = new RectOffset(outerPadding, 0, outerPadding, 0);
            panel.autoLayoutDirection = LayoutDirection.Vertical;
            panel.autoLayoutPadding = new RectOffset(0, 0, 0, layoutPadding);
            panel.autoLayout = true;

            var child1 = panel.AddUIComponent<UIPanel>();
            child1.name = "TestPanelChild1";
            child1.backgroundSprite = "SubcategoriesPanel";
            child1.color = new Color32(0, 0, 255, 255);
            child1.size = new Vector2(100,100);
            var child2 = panel.AddUIComponent<UIPanel>();
            child2.name = "TestPanelChild2";
            child2.backgroundSprite = "SubcategoriesPanel";
            child2.color = new Color32(0, 255, 0, 255);
            child2.size = new Vector2(100,100);
            var child3 = panel.AddUIComponent<UIPanel>();
            child3.name = "TestPanelChild3";
            child3.backgroundSprite = "SubcategoriesPanel";
            child3.color = new Color32(255, 0, 0, 255);
            child3.size = new Vector2(100,100);

            panel.FitChildren(Vector2.one * outerPadding);
        }
        void test2()
        {
            var outerPadding = 5;
            
            var child2 = GameObject.Find("TestPanelChild2").GetComponent<UIPanel>();
            child2.isVisible = true;

            var panel = GameObject.Find("TestPanel").GetComponent<UIPanel>();
            panel.FitChildren(Vector2.one * outerPadding);

            















            panel.PerformLayout();
            panel.FitChildren(new Vector2(5,5));




            panel.padding = new RectOffset(2, 2, 2, 20);
            panel.autoFitChildrenHorizontally = true;
            panel.autoFitChildrenVertically = true;

            panel.PerformLayout();
            panel.autoLayout = true;
            panel.autoLayoutDirection = LayoutDirection.Vertical;
            panel.backgroundSprite = "SubcategoriesPanel";
            panel.color = new Color32(0, 0, 255, 255);

            panel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Right;

            panel.transformPosition = new Vector3(0, 0);

            var child = GameObject.Find("TestPanelChild1").GetComponent<UIPanel>();
            child.height = 20;
            child.pivot = UIPivotPoint.BottomRight;
            child.transformPosition = new Vector3(.5f, .5f, 0);
            child.transformPosition = new Vector3(1, -0.5f,0);
            //child.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            child.ResetLayout(true, true);

            var child2 = GameObject.Find("TestPanelChild2").GetComponent<UIPanel>();
            child2.size = new Vector2(30, 30);
            child2.verticalSpacing = 10;
            child2.isVisible = false;
            child2.height = 100;
            child2.backgroundSprite = "SubcategoriesPanel";
            child2.relativePosition = new Vector3(10,10);


            child.relativePosition = Vector3.zero;

            child.backgroundSprite = "SubcategoriesPanel";
            child.color = new Color32(0, 255, 255, 255);
            child.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
            child.size = panel.size / 3;
        }
    }
}
