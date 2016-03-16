using System.Collections.Generic;
using NetworkSkins.Net;
using NetworkSkins.UI;

namespace NetworkSkins.Props
{
    public class UITreeDistanceOption : UISliderOption
    {
        private LanePosition _lanePosition;
        public LanePosition LanePosition { 
            get { return _lanePosition; }
            set 
            { 
                _lanePosition = value;
                Description = value.GetDescription() + " Tree Dist.";
            } 
        }

        protected override bool PopulateSlider()
        {
            if (SelectedPrefab != null && PropCustomizer.Instance.HasTrees(SelectedPrefab, _lanePosition))
            {
                var defaultDistance = PropCustomizer.Instance.GetDefaultTreeDistance(SelectedPrefab, _lanePosition);
                var activeDistance = PropCustomizer.Instance.GetActiveTreeDistance(SelectedPrefab, _lanePosition);

                if (defaultDistance < 0f) return false;

                Slider.minValue = defaultDistance * 0.25f;
                Slider.maxValue = defaultDistance * 1.75f;
                Slider.value = activeDistance;
                Slider.stepSize = defaultDistance * .15f;

                return true;
            }
            return false;
        }

        protected override void OnValueChanged(float val)
        {
            PropCustomizer.Instance.SetTreeDistance(SelectedPrefab, _lanePosition, val);
        }
    }
}