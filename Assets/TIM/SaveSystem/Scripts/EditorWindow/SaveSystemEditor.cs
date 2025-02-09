using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TIM
{
    public class SaveSystemEditor : OdinMenuEditorWindow
    {
        private bool _focused = true;
        [SerializeField, HideInInspector] private WindowParameters _windowParameters;
        [SerializeField, HideInInspector] private Guide _guide;

        public bool ParametersChanged;
        public bool EditMode;
        
        [MenuItem("TIM/Save system")]
        public static void OpenWindow()
        {
            var window = GetWindow<SaveSystemEditor>();
            window.titleContent = new GUIContent("Save System", Resources.Load<Texture>("SaveSystem window icon"));
            window.Show();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            
            if (_focused)
            {
                ParametersChanged = false;
                // show parameters:
                if(_windowParameters == null)
                    _windowParameters = new WindowParameters();
                
                _windowParameters.SaveSystemEditor = this;
                if (!Application.isPlaying)
                {
                    SaveSystem.Path = _windowParameters.SavePath;
                    SaveSystem.LoadAll();
                }
                var loadedSaveFiles = SaveSystem.GetLoadedSaveFiles;
                tree.Add("Save system", _windowParameters, EditorIcons.House);
                
                // guide:
                if(_guide == null)
                    _guide = new Guide();
                
                tree.Add("Guide", _guide, EditorIcons.Info);
                
                // show files
                foreach (SaveSystem.DataFile saveFile in loadedSaveFiles)
                {
                    tree.Add(saveFile.Name.Replace("\\", "/"), saveFile, EditorIcons.File);
                    saveFile.OnWantToBeSaved = SaveFile;
                    saveFile.OnWantToBeDeleted = DeleteFile;
                    saveFile.EditModeChangedAction = OnEditModeChanged;
                }
            }

            return tree;
        }

        private void SaveFile(string fileName)
        {
            if(ParametersChanged)
                return;
            
            SaveSystem.SaveFile(fileName);
        }
        
        private void DeleteFile(string fileName)
        {
            if(ParametersChanged)
                return;
            
            SaveSystem.DeleteFileFromDisk(fileName);
            ForceMenuTreeRebuild();
        }

        private void OnEditModeChanged(bool editMode)
        {
            EditMode = editMode;
        }

        private void OnFocus()
        {
            if(EditMode)
                return;
            
            _focused = true;
            ForceMenuTreeRebuild();
        }

        private void OnLostFocus()
        {
            if(EditMode)
                return;

            _focused = false;
            ForceMenuTreeRebuild();
        }
        
        [System.Serializable]
        private class WindowParameters
        {
            [HorizontalGroup("Group", Width = 350)]
            [VerticalGroup("Group/V")]
            [BoxGroup("Group/V/Box", false), LabelWidth(80), EnumToggleButtons, ShowInInspector, OnValueChanged(nameof(OnParametersChanged))]
            public SaveSystem.SavePath SavePath
            {
                get => SaveSystem.Path;
                set => SaveSystem.Path = value;
            }

            [HideInInspector] public SaveSystemEditor SaveSystemEditor;
            
            [HorizontalGroup("Group/V/Buttons"), Button(ButtonSizes.Medium)]
            private void Refresh()
            {
                SaveSystemEditor.ForceMenuTreeRebuild();
            }

            private void OnParametersChanged()
            {
                SaveSystemEditor.ParametersChanged = true;
            }

            [HorizontalGroup("Group/V/Buttons"), Button(ButtonSizes.Medium)]
            private void OpenFolder()
            {
                SaveSystem.OpenRootFolder();
            }

            [HorizontalGroup("Group/V/Buttons"), GUIColor(1,0.3f,0.3f), Button(ButtonSizes.Medium)]
            private void DeleteAll()
            {
                SaveSystem.DeleteAllFromDisk();
                SaveSystemEditor.ForceMenuTreeRebuild();
            }
            
            [BoxGroup("Group/V/Runtime operations"), HorizontalGroup("Group/V/Runtime operations/H", 80), Button(ButtonSizes.Medium), ShowIf("@SaveButtonEnabled")]
            private void SaveAll()
            {
                SaveSystem.SaveAll();
            }

            private bool SaveButtonEnabled => Application.isPlaying && SaveSystem.GetLoadedSaveFiles.Count > 0;

            private bool PlayModeEnabled => Application.isPlaying;
        }

        [System.Serializable]
        private class Guide
        {
            [FoldoutGroup("Как вообще это работает?"), HideLabel, DisplayAsString(false), ShowInInspector, EnableGUI]
            public string CommonHeader =>
                "Для удобства работы с системой сохранений и функциональности данные необходимо загрузить из диска в оперативную память. " +
                "Прочесть содержимое файла можно только после того, как он загрузился в оперативную память. " +
                "Так же для сохранения данных на диске нужно сначала их кинуть в оперативную память, а потом произвести запись на диск. " +
                "Может звучать сложно, но на самом деле это очень просто! Примерно так выглядит процесс\n" +
                "1. Ты вызываешь метод Set(объект, название файла) чтобы поместить экземпляр класса в оперативную память. \n" +
                "2. После того, как всё, что нужно было помещено в оперативку, необходимо совершить запись на диск через метод SaveAll() \n" +
                "3. Для загрузки данных в оперативку используй метод LoadAll() \n" +
                "4. Для чтения (чтобы получить экземпляр класса из оперативки) используй метод Get<T>(название файла)." +
                "\n\n" +
                "Если объяснять более подробно, то:" +
                "Сначала тебе необходимо назначить данные в оперативную память, для этого используй функцию <b>Set(объект, название файла)</b>. " +
                "Название файла можешь придумать любое, главное чтобы в нём не было точек, вопросительных знаков и т.д. \n" +
                "<i>Например: 'file' или если ты хочешь сохранить файл внутри какой-то папки, то можешь написать так: " +
                "'directory/file'</i>\n" +
                "потом когда данные назначены, ты можешь их перенести на диск, используя метод <b>SaveAll()</b>, который " +
                "переводит все файлы из оперативной памяти на диск, но тогда те файлы, которые отсутствуют в оперативной памяти, " +
                "но присутствуют на диске, будут удалены с диска. \n" +
                "Или можешь использовать метод <b>SaveFile(название файла)</b>, который переместит " +
                "указанный файл из оперативной памяти на диск. " +
                "Если ты не сохранишь файлы на диске, то тогда при выходе из игры, или повторном вызове метода LoadAll(), данные находящиеся в оперативке просто пропадут." +
                "\n\n" +
                "Для получения данных, тебе сначала нужно загрузить их из диска в оперативную память через метод <b>LoadAll()</b>, " +
                "а потом доставать считывать необходимый файл через функцию <b>Get<Тип объекта>(название файла)</b>. Но важно учитывать, что если такой файл " +
                "не был загружен в оперативку, то метод Get кинет ошибочку. Чтобы проверить наличие файла в оперативке используй " +
                "<b>IsFileLoaded(название файла)</b>, который возвращает bool." +
                "\n\n" +
                "SaveSystem умеет сохранять любые объекты (классы, унаследованные от object), кроме тех, которые наследуются от UnityEngine.Object. " +
                "Например, чтобы сохранить поле типа float, тебе надо создать класс и поместить в него это поле, а потом " +
                "поместить экзепляр класса в оперативную память через метод Set(объект, название файла) и сохранить всю ячейку на диске с помощью " +
                "метода Save(). Если ты попытаешься засунуть в функцию Set() что-то наподобие float, bool, int, или string, а потом вытащить " +
                "это с помощью функции Get<>(), то у тебя нихера не получится.";

            [FoldoutGroup("Как создать ячеичное сохранение?"), HideLabel, DisplayAsString(false), ShowInInspector, EnableGUI]
            public string CellsHeader =>
                "Ячейки нужны если вы хотите создать систему слотового сохранения. Помните, " +
                "есть в играх такая механика: 'Выберите слот для сохранения' чтоб можно было делать несколько параллельных сохранений. " +
                "Конечно система так устроена, что вы сами можете просто сохранять файлы в папках с разным названием, тем самым сгруппируя файлы по слотам. " +
                "Но вместе с вызовом метода LoadAll() будут загружаться вообще все папки (т.е. в нашем случае все ячейки), что очень тупо. \n" +
                "Именно для этого придуманы методы <b>LoadDirectory(директория)</b> и <b>SaveDirectory(директория)</b>. \n" +
                "Например: LoadDirectory(\"Cell 0\"), SaveDirectory(\"Cell 0\")"
            ;
            
            [FoldoutGroup("Какого хера я должен сначала сохранить в оперативку?"), HideLabel, DisplayAsString(false), ShowInInspector, EnableGUI]
            public string SaveMechanicHeader =>
                "Вы можете задаться вопросом: 'А почему я не могу работать сразу с постоянной памятью устройства? Зачем мне сначала загружать " +
                "данные в оперативную память и только потом получать доступ к ним?!'." +
                "\n\n" +
                "Представь ситуацию, когда тебе необходимо сохранить пройденный уровень в игре на компьютере, находящимся внутри твоей игры на Unity, " +
                "над которой ты работаешь. Проблема в том, что при выключении компьютера игроком или переходе игрока в другую локацию, объект хранящий данные (например это " +
                "само приложение-игра, запущенная на компьютере внутри твоей игры) просто будет удалён. А запись данных на диск должна производиться " +
                "строго по нажатию кнопки 'сохранить' в меню игры. Это приведет к потере данных о текущем уровне внутри той игры, внутри компа. Да, ты правильно " +
                "понял, что это очень ситуативная хуйня. Но тебе так может показаться лишь в том случае, если ты работаешь над гиперказуальными, простейшими " +
                "играми. Я пример не придумывал. У меня реально стоит сейчас такая задача, пока я создаю эту систему сохранений. Главная фича этой системы - " +
                "функциональность, ведь её можно использовать для работы над проектами любого уровня. Как и любой хороший инструмент, эта система может " +
                "показаться сложной в начале, но если её освоить, то она откроет тебе большой простор для возможностей."
            ;
            
            [FoldoutGroup("Список методов"), HideLabel, DisplayAsString(false), ShowInInspector, EnableGUI]
            public string MethodsHeader =>
                "- Set(Object объект, string название файла) - сохранить файл в оперативку (объект сериализуется в текст и сохраняется в .json файле) \n\n" +
                "- Get<Тип>(string название файла) - прочесть файл из оперативки (вернет объект данного типа) \n\n" +
                "- SaveAll() - записать данные с оперативки на диск. (Метод работает так, что сначала удаляет всю информацию с диска, а потом записывает новую, что " +
                "означает: если ты не загрузил условно файл 'пупок', то при вызове SaveAll(), если его не будет в оперативке, то и с диска он пропадет!)\n\n" +
                "- SaveDirectory(string название директории) - записать всю директорию из оперативки на диск.\n\n" +
                "- SaveFile(string название файла) - записать файл из оперативки на диск.\n\n" +
                "- LoadAll() - загрузить всё из диска в оперативную память. \n\n" +
                "- LoadDirectory(string название директории) - загрузить директорию из диска в оперативную память. \n\n" +
                "- LoadFile(string название файла) - загрузить указанный файл из диска в оперативную память. Если файла не существует на диске, то вылезет ошибка.\n" +
                "  *Для проверки наличия файла на диске используй CheckFileExistence(string название файла) \n\n" +
                "- DeleteFile(string название файла) - удалить файл из оперативной памяти. Если не вызвать метод SaveAll() или SaveDirectory() для директории, где находится файл, то файл останется на диске и при повторном вызове метода Load() снова будет загружен в оперативную память \n\n" +
                "- DeleteFileFromDisk(string название файла) - удалить указанный файл с диска. Но если файл был загружен в оперативную память при вызове метода Load(), " +
                " то он так и останется в оперативной памяти. Для удаления файла из оперативной памяти тоже, используй метод DeleteFile()\n\n" +
                "- DeleteDirectory(string папка) - удалить всю папку из оперативной памяти. Работает по принципу метода DeleteFile()\n\n" +
                "- DeleteDirectoryFromDisk(string папка) - удалить всю папку с диска. Работает по принципу метода DeleteFileFromDisk()\n\n" +
                "- DeleteAllFromDisk(bool clearCache) - удалить все сохраненные данные c диска по выбранному пути сохранения Path. Если данные были загружены в оперативную " +
                "память через метод Load(), то они оттуда не удалятся, только при условии что параметр clearCache будет равен false. \n\n" +
                "- IsFileLoaded(string название файла) - проверить наличие файла в оперативной памяти \n\n" +
                "- bool CheckFileExistence(string название файла) - проверить наличие файла на диске. " +
                    "(Если данные не загружены в оперативную память, но файл существует, то функция всё равно вернет true) \n\n" +
                "- OpenRootFolder() - открыть папку с сохранениями. Работает только на компах. \n\n" +
                "- ClearCache() - удалить всё из оперативной памяти"
            ;
        }
    }
}