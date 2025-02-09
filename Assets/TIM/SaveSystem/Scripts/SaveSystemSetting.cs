using Sirenix.OdinInspector;
using UnityEngine;

namespace TIM
{
    public class SaveSystemSetting : ScriptableObject
    {
        public SaveSystem.SavePath Path = SaveSystem.SavePath.Device;
    }
}