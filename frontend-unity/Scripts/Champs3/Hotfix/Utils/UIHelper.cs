using UnityEngine.Events;
using UnityEngine.UI;

namespace champs3.Hotfix
{
    public static class UIHelper
    {
        public static void AddListener(this ToggleGroup toggleGroup, UnityAction<int> selectEventHandler)
        {
            var togglesList = toggleGroup.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < togglesList.Length; i++)
            {
                int index = i;
                togglesList[i].AddListener((isOn) => 
                {
                    if (isOn)
                    {
                        selectEventHandler(index);
                    }
                });
            }
        }
        
        public static void AddListener(this Toggle toggle, UnityAction<bool> selectEventHandler)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(selectEventHandler);
        }
    }
}