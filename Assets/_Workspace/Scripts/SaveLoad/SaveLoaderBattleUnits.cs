using System.Collections.Generic;
using _Workspace.Scripts.Data.Scripts;
using _Workspace.Scripts.TacticalBattle;
using TIM;

namespace _Workspace.Scripts.SaveLoad
{
    public interface ISaveLoaderBattleUnits
    {
        public void SaveData(List<Unit> units);
        public List<UnitSaveData> LoadUnitsData();
    }
    public class SaveLoaderBattleUnits : ISaveLoaderBattleUnits
    {
        public void SaveData(List<Unit> units)
        {
            List<UnitSaveData> saveDatas = new List<UnitSaveData>();
            foreach (var unit in units)
            {
                UnitSaveData unitSaveData = new UnitSaveData(unit.name, unit.GetSaveData(), unit.Stats.GetSaveData());
                saveDatas.Add(unitSaveData);
            }

            SaveSystem.Set(saveDatas, "Units");
            SaveSystem.SaveAll();
        }
        
        public List<UnitSaveData> LoadUnitsData()
        {
            SaveSystem.LoadAll();
            var data = SaveSystem.Get<List<UnitSaveData>>("Units");
            return data;
        }
    }
}