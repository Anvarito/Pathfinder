using System.Collections.Generic;
using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace TIM
{
    public static class SaveSystem
    {
        /// <summary>
        /// Путь по которому осуществляется сохранение
        /// </summary>
        public static SavePath Path
        {
            get => Setting.Path;
            set => Setting.Path = value;
        }

        private static List<DataFile> _files = new List<DataFile>();
        private static DataFormat _dataFormat = DataFormat.JSON;

        public static SaveSystemSetting Setting
        {
            get
            {
                if (!_setting)
                    _setting = Resources.Load<SaveSystemSetting>("Save System Setting");
                return _setting;
            }
        }

        private static SaveSystemSetting _setting;

        #region Public Methods

        /// <summary>
        /// Загрузить всё из диска в оперативную память.
        /// </summary>
        public static void LoadAll()
        {
            _files.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(GetRootPath());
            if (!directoryInfo.Exists)
                return;

            LoadDirectory(directoryInfo);
        }

        /// <summary>
        /// Записать данные с оперативки на диск
        /// (Метод работает так, что сначала удаляет всю информацию с диска, а потом записывает новую, что означает: если ты не загрузил условно файл 'пупок', то при вызове SaveAll(), если его не будет в оперативке, то и с диска он пропадет!)
        /// </summary>
        /// <param name="deleteExistingDataFromDisk">если false, то данные будут перенесены поверх имеющихся на диске (т.е. файлы не имеющиеся в оперативке, но имеющиеся на диске не будут удалены)</param>
        public static void SaveAll(bool deleteExistingDataFromDisk = true)
        {
            if (deleteExistingDataFromDisk)
                DeleteAllFromDisk(false);

            foreach (DataFile saveFile in _files)
            {
                SaveFile(saveFile.Name);
            }
        }

        /// <summary>
        /// загрузить директорию из диска в оперативную память
        /// </summary>
        /// <param name="directoryName">путь к директории. Например: "Player/Inventory"</param>
        public static void LoadDirectory(string directoryName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetDirectoryPath(directoryName));

            LoadDirectory(directoryInfo);
        }

        /// <summary>
        /// загрузить директорию из диска в оперативную память (этот метод может быть полезен, если уже есть DirectoryInfo, или путь к папке на компьютере, вне папки системы сохранениый)
        /// </summary>
        /// <param name="directoryInfo"></param>
        public static void LoadDirectory(DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
                return;

            var files = directoryInfo.GetFiles();
            foreach (var fileInfo in files)
            {
                if (fileInfo.Extension == ".json")
                {
                    int cellPathLength = GetRootPath().Length + 1;
                    string fileName = fileInfo.FullName.Substring(cellPathLength,
                        fileInfo.FullName.Length - cellPathLength - fileInfo.Extension.Length);
                    fileName = fileName.Replace("\\", "/");
                    LoadFile(fileName);
                }
            }

            var directories = directoryInfo.GetDirectories();
            foreach (DirectoryInfo directory in directories)
            {
                LoadDirectory(directory);
            }
        }

        /// <summary>
        /// записать всю директорию из оперативки в память устройства
        /// </summary>
        /// <param name="directoryPath">путь к директории. Например: "Player/Inventory"</param>
        /// <param name="deleteExistingDataFromDisk">если false, то данные будут записаны поверх имеющихся, без удаления тех файлов, что отсутствуют в оперативке, но существуют на диске</param>
        public static void SaveDirectory(string directoryPath, bool deleteExistingDataFromDisk = true)
        {
            if (deleteExistingDataFromDisk)
                DeleteDirectoryFromDisk(directoryPath);

            foreach (var file in _files)
            {
                if (file.Name.Contains(directoryPath))
                    SaveFile(file.Name);
            }
        }

        /// <summary>
        /// Load file from disk to RAM. If the file doesn't exist on the disk, an exception will occur
        /// *Use CheckFileExistence(string название файла) to check file existence
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static void LoadFile(string fileName)
        {
            if (!CheckFile_Disk(fileName))
                throw new UnityException(
                    "File not found! Save data before load. You can use CheckFileExistence() method for make file existing condition. File: " +
                    fileName);


            string json = System.IO.File.ReadAllText(GetFilePath(fileName));

            foreach (DataFile file in _files)
            {
                if (file.Name == fileName)
                    file.Data = json;
            }

            _files.Add(new DataFile() {Name = fileName, Data = json});
        }

        /// <summary>
        /// Load file from disk to RAM. If the file doesn't exist on the disk, an exception will occur
        /// *Use CheckFileExistence(string название файла) to check file existence
        /// </summary>
        /// <param name="fileName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="UnityException"></exception>
        public static T LoadFile<T>(string fileName)
        {
            LoadFile(fileName);
            return Get<T>(fileName);
        }

        /// <summary>
        /// Save file from RAM to disk
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static void SaveFile(string fileName)
        {
            foreach (DataFile file in _files)
            {
                if (file.Name == fileName)
                {
                    string path = GetFilePath(fileName);
                    var f = new FileInfo(path);
                    Directory.CreateDirectory(f.Directory.FullName);
                    System.IO.File.WriteAllText(path, file.Data);
                    return;
                }
            }

            throw new UnityException("You must set data before save. Use Set() method for it");
        }

        /// <summary>
        /// Save the file directly to disk
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public static void SaveFile(object data, string fileName)
        {
            Set(data, fileName);
            SaveFile(fileName);
        }


        /// <summary>
        /// удалить указанный файл из диска
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static void DeleteFileFromDisk(string fileName)
        {
            if (CheckFile_Disk(fileName))
            {
                System.IO.File.Delete(GetFilePath(fileName));
            }
        }


        /// <summary>
        /// удалить всё из оперативной памяти
        /// </summary>
        public static void ClearCache()
        {
            _files.Clear();
        }


        /// <summary>
        /// прочесть файл из оперативки
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        /// <typeparam name="T">тип объекта</typeparam>
        /// <returns>возвращает объект данного типа T</returns>
        public static T Get<T>(string fileName)
        {
            foreach (DataFile file in _files)
            {
                if (file.Name == fileName)
                {
                    System.Type type;
                    return DeserializeData<T>(file.Data);
                }
            }

            throw new UnityException("File not found! Load data before get! fileName: " + fileName);
        }


        /// <summary>
        /// сохранить файл в оперативку
        /// (объект сериализуется в текст и сохраняется в .json файле)
        /// </summary>
        /// <param name="data">экземпляр любого класса или структуры
        /// (кроме тех, что унаследованны от UnityEngine.Object, например: Transform, MonoBehaviour, ScriptableObject, Texture и т.д.)</param>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static void Set(object data, string fileName)
        {
            string json = SerializeData(data);
            foreach (DataFile file in _files)
            {
                if (file.Name == fileName)
                {
                    file.Data = json;
                    return;
                }
            }

            _files.Add(new DataFile() {Name = fileName, Data = json});
        }


        /// <summary>
        /// удалить файл из оперативной памяти
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static void DeleteFile(string fileName)
        {
            for (int i = 0; i < _files.Count; i++)
            {
                if (_files[i].Name == fileName)
                {
                    _files.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// удалить всю папку из оперативной памяти
        /// </summary>
        /// <param name="directory">название директории, например: "Settings" или "Player/Inventory"</param>
        public static void DeleteDirectory(string directory)
        {
            if (directory == "")
            {
                ClearCache();
            }
            else
            {
                for (int i = _files.Count - 1; i >= 0; i--)
                {
                    if (_files[i].Name.Length > directory.Length &&
                        _files[i].Name.Substring(0, directory.Length) == directory)
                    {
                        _files.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// удалить всю папку с диска
        /// </summary>
        /// <param name="directoryName">название директории, например: "Settings" или "Player/Inventory"</param>
        public static void DeleteDirectoryFromDisk(string directoryName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(GetDirectoryPath(directoryName));
            if (directoryInfo.Exists)
                directoryInfo.Delete(true);
        }

        /// <summary>
        /// открыть папку с сохранениями. Работает только на компах
        /// </summary>
        public static void OpenRootFolder()
        {
#if UNITY_STANDALONE
            if (!System.IO.Directory.Exists(GetRootPath()))
                Directory.CreateDirectory(GetRootPath());

            Application.OpenURL("file:///" + GetRootPath());
#endif
        }


        /// <summary>
        /// удалить все сохраненные данные c диска по выбранному пути сохранения Path
        /// </summary>
        /// <param name="clearCache">если true, то данные удалятся из оперативной памяти тоже. Полезно, если нужно сбросить все сохранения игрока.</param>
        public static void DeleteAllFromDisk(bool clearCache = true)
        {
            if (clearCache)
                ClearCache();

            DirectoryInfo directoryInfo = new DirectoryInfo(GetRootPath());
            if (directoryInfo.Exists)
                directoryInfo.Delete(true);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Sirenix Odin Serialization way to serialize any type of data
        /// </summary>
        /// <returns>.json file</returns>
        public static string SerializeData(object data)
        {
            var bytes = SerializationUtility.SerializeValue(data, _dataFormat);
            string json = Encoding.UTF8.GetString(bytes);
            return json;
        }

        /// <summary>
        /// Sirenix Odin Serialization way to serialize any type of data
        /// </summary>
        public static byte[] SerializeDataToBytes(object data)
        {
            var bytes = SerializationUtility.SerializeValue(data, _dataFormat);
            return bytes;
        }

        /// <summary>
        /// Sirenix Odin Serialization way to deserialize .json file
        /// </summary>
        /// <param name="data">.json file</param>
        public static T DeserializeData<T>(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return SerializationUtility.DeserializeValue<T>(bytes, _dataFormat);
        }

        /// <summary>
        /// Sirenix Odin Serialization way to deserialize bytes array
        /// </summary>
        public static T DeserializeDataFromBytes<T>(byte[] bytes)
        {
            return SerializationUtility.DeserializeValue<T>(bytes, _dataFormat);
        }

        #endregion

        #region Getters

        /// <summary>
        /// возвращает список загруженных файлов. Не знаю, зачем это может быть вам полезно, но такой метод применяется в коде SaveSystem.
        /// </summary>
        public static List<DataFile> GetLoadedSaveFiles => _files;

        /// <summary>
        /// Возвращает полный путь к директории в памяти устройства
        /// </summary>
        /// <param name="name">название директории, например: "Settings" или "Player/Inventory"</param>
        public static string GetDirectoryPath(string name)
        {
            return GetRootPath() + "/" + name;
        }

        /// <summary>
        /// Возвращает путь к папке, где хранятся сохранения
        /// </summary>
        public static string GetRootPath()
        {
            string path = Path == SavePath.ApplicationFolder
                ? Application.dataPath
                : Application.persistentDataPath;
            path += "/SaveSystem Data";
            return path;
        }

        /// <summary>
        /// проверить наличие файла в оперативной памяти
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static bool CheckFile_RAM(string fileName)
        {
            foreach (DataFile file in _files)
            {
                if (file.Name == fileName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// проверить наличие файла на диске. Вернет true, если файл существует
        /// </summary>
        /// <param name="fileName">название файла без использования знаков * | ? : \ / , ` и т.д. Например: "Player_stats", или "Controls/Mouse sensitivity"</param>
        public static bool CheckFile_Disk(string fileName)
        {
            return File.Exists(GetFilePath(fileName));
        }

        /// <summary>
        /// Возвращает полный путь к файлу в памяти устройства
        /// </summary>
        /// <param name="name"></param>
        public static string GetFilePath(string name)
        {
            return GetRootPath() + "/" + name + ".json";
        }

        #endregion

        #region Private methods

        private static bool CheckDirectoryExisting(string directoryName)
        {
            return System.IO.Directory.Exists(GetDirectoryPath(directoryName));
        }

        #endregion

        [System.Serializable]
        public class DataFile
        {
            [HideInInspector] public string Name;

            [TextArea(1, 40), HideLabel, ShowIf("@!_editMode"), Header("@Name"),
             OnValueChanged("@_valueChanged = true")]
            public string Data;

            [SerializeField, HideInInspector] private bool _valueChanged;

            [OdinSerialize, BoxGroup, ShowInInspector, ShowIf("@_editMode"), EnableGUI]
            private object _dataSerialized;

            [HorizontalGroup("Buttons", Width = 80), GUIColor("GetDeleteColor"), Button(ButtonSizes.Medium),
             PropertyOrder(-1)]
            private void Delete()
            {
                OnWantToBeDeleted?.Invoke(Name);
            }

            [HorizontalGroup("Buttons", Width = 80), Button(ButtonSizes.Medium), PropertyOrder(-1),
             ShowIf("@_valueChanged || _editMode")]
            private void Save()
            {
                if (_editMode)
                {
                    var bytes = SerializationUtility.SerializeValue(_dataSerialized, _dataFormat);
                    Data = System.Text.Encoding.UTF8.GetString(bytes);
                    _editMode = false;
                }

                OnWantToBeSaved?.Invoke(Name);
                _valueChanged = false;
            }

            [PropertyOrder(-0.5f), SerializeField, ToggleLeft, OnValueChanged("OnEditModeChanged")]
            private bool _editMode;

            private void OnEditModeChanged()
            {
                if (_editMode)
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(Data);
                    DeserializationContext deserializationContext = new DeserializationContext();
                    _dataSerialized =
                        SerializationUtility.DeserializeValueWeak(bytes, _dataFormat, deserializationContext);
                }

                EditModeChangedAction?.Invoke(_editMode);
            }

            [OnInspectorInit]
            private void OnInspectorInit()
            {
                if (_editMode)
                {
                    _editMode = false;
                    EditModeChangedAction?.Invoke(_editMode);
                }
            }

            private Color GetDeleteColor => new Color(1, 0.5f, 0.5f);

            public UnityAction<string> OnWantToBeSaved { get; set; }
            public UnityAction<string> OnWantToBeDeleted { get; set; }
            public UnityAction<bool> EditModeChangedAction { get; set; }
        }

        public enum SavePath
        {
            ApplicationFolder,
            Device
        }
    }
}