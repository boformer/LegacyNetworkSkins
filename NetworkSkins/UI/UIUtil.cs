using ColossalFramework.UI;
using System.Text;
using UnityEngine;

namespace NetworkSkins.UI
{
    public class UIUtil
    {
        private const float LABEL_RELATIVE_WIDTH = .30f;
        private const float COLUMN_PADDING = 5f;

        public static UIDropDown CreateDropDownWithLabel(out UILabel label, UIComponent parent, string labelText, float width) 
        {
            var labelWidth = Mathf.Round(width * LABEL_RELATIVE_WIDTH);
            
            UIDropDown dropDown = UIUtil.CreateDropDown(parent);
            dropDown.relativePosition = new Vector3(labelWidth + COLUMN_PADDING, 0);
            dropDown.width = width - labelWidth - COLUMN_PADDING;

            label = AddLabel(parent, labelText, labelWidth, dropDown.height);

            return dropDown;
        }
        private static UILabel AddLabel(UIComponent parent, string text, float width, float dropDownHeight)
        {
            var label = parent.AddUIComponent<UILabel>();
            label.text = text;
            label.textScale = .85f;
            label.textColor = new Color32(200, 200, 200, 255);
            label.autoSize = false;
            label.autoHeight = true;
            label.width = width;
            label.textAlignment = UIHorizontalAlignment.Right;
            label.relativePosition = new Vector3(0, Mathf.Round((dropDownHeight - label.height) / 2));

            return label;
        }

        public static UIDropDown CreateDropDown(UIComponent parent)
        {
            UIDropDown dropDown = parent.AddUIComponent<UIDropDown>();
            dropDown.size = new Vector2(90f, 30f);
            dropDown.listBackground = "GenericPanelLight";
            dropDown.itemHeight = 30;
            dropDown.itemHover = "ListItemHover";
            dropDown.itemHighlight = "ListItemHighlight";
            dropDown.normalBgSprite = "ButtonMenu";
            dropDown.disabledBgSprite = "ButtonMenuDisabled";
            dropDown.hoveredBgSprite = "ButtonMenuHovered";
            dropDown.focusedBgSprite = "ButtonMenu";
            dropDown.listWidth = 90;
            dropDown.listHeight = 500;
            dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropDown.popupColor = new Color32(45, 52, 61, 255);
            dropDown.popupTextColor = new Color32(170, 170, 170, 255);
            dropDown.zOrder = 1;
            dropDown.textScale = 0.8f;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            dropDown.selectedIndex = 0;
            dropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropDown.itemPadding = new RectOffset(14, 0, 8, 0);

            UIButton button = dropDown.AddUIComponent<UIButton>();
            dropDown.triggerButton = button;
            button.text = "";
            button.size = dropDown.size;
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            dropDown.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t; dropDown.listWidth = (int)t.x;
            });

            return dropDown;
        }

        // TODO better names from localisation!
        public static string GenerateBeautifiedPrefabName(PrefabInfo prefab, PrefabInfo defaultPrefab) 
        {
            string itemName = (prefab == null ? "None" : prefab.name);

            var index1 = itemName.IndexOf('.');
            if (index1 > -1) itemName = itemName.Substring(index1 + 1);

            var index2 = itemName.IndexOf("_Data");
            if (index2 > -1) itemName = itemName.Substring(0, index2);

            itemName = AddSpacesToSentence(itemName);

            if (prefab == defaultPrefab) itemName += " (Default)";

            return itemName;
        }

        private static string AddSpacesToSentence(string text)
        {
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
