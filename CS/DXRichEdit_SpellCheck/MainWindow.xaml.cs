using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.SpellChecker;
using DevExpress.XtraSpellChecker;
using System.Globalization;

namespace DXRichEdit_SpellCheckMenu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string dictPath = "Dic";
        public SpellChecker spellChecker;

        public MainWindow()
        {
            InitializeComponent();

            richEdit.Loaded+=new RoutedEventHandler(richEdit_Loaded);

        }


        void richEdit_Loaded(object sender, RoutedEventArgs e)
        {
            richEdit.LoadDocument("Bunin.docx");
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForDictionary(this.spellChecker, "ru")) return;

            Mouse.OverrideCursor = Cursors.Wait;
            try {
                this.spellChecker = InitializeSpellChecker();
                richEdit.SpellChecker = spellChecker;
                spellChecker.SpellCheckMode = SpellCheckMode.AsYouType;
                spellChecker.Culture = new CultureInfo("ru-RU");
            }
            finally {
                Mouse.OverrideCursor = null;
            } 
        }

        private bool CheckForDictionary(SpellChecker spellChecker, string langname){
            if (spellChecker != null) {
                if (spellChecker.Dictionaries.Count > 0) {
                    foreach (SpellCheckerDictionaryBase dict in spellChecker.Dictionaries) {
                        if (dict.Culture.TwoLetterISOLanguageName == langname) return true;
                    }
                }
            }
            return false;
        }
        #region SpellChecker initialization
        public static SpellChecker InitializeSpellChecker()
        {
            SpellChecker spellChecker = new SpellChecker();
            spellChecker.Culture = new CultureInfo("en-US");
            RegisterDictionary(spellChecker, GetDefaultDictionary(dictPath));
            RegisterDictionary(spellChecker, GetCustomDictionary(dictPath,"en-US"));
            RegisterDictionary(spellChecker, GetLanguageDictionary(dictPath, "ru-RU"));
            RegisterDictionary(spellChecker, GetCustomDictionary(dictPath, "ru-RU"));

            //SpellCheckTextControllersManager.Default.RegisterClass(typeof(RichEditControl), typeof(RichEditSpellCheckController));
            return spellChecker;
        }
        static SpellCheckerDictionaryBase GetDefaultDictionary(string path)
        {
             SpellCheckerISpellDictionary dic = new SpellCheckerISpellDictionary();
            dic.DictionaryLoaded += new EventHandler(dic_DictionaryLoaded);

            dic.DictionaryPath = path + "\\american.xlg";
            dic.GrammarPath = path + "\\english.aff";
            dic.AlphabetPath = path+ "\\EnglishAlphabet.txt";
            dic.Culture = new CultureInfo("en-US");
            dic.Load();
            dic.DictionaryLoaded -= dic_DictionaryLoaded;
            return dic;
        }
        static SpellCheckerDictionaryBase GetLanguageDictionary(string path, string _culture)
        {
            SpellCheckerOpenOfficeDictionary dict = new SpellCheckerOpenOfficeDictionary();
            dict.DictionaryLoaded += new EventHandler(dic_DictionaryLoaded);

            dict.DictionaryPath = String.Format("{0}\\{1}.dic", path, _culture);
            dict.GrammarPath = String.Format("{0}\\{1}.aff", path, _culture);
            dict.Culture = new CultureInfo(_culture);
            dict.Load();
            dict.DictionaryLoaded -= dic_DictionaryLoaded;
            return dict;
        }
        static void dic_DictionaryLoaded(object sender, EventArgs e)
        {
            SpellCheckerDictionaryBase dictBase = (SpellCheckerDictionaryBase) sender;
            if (dictBase == null) MessageBox.Show(String.Format("Dictionary for {0} culture is not loaded", dictBase.Culture.DisplayName));
        }
        static void RegisterDictionary(SpellChecker spellChecker, SpellCheckerDictionaryBase dict)
        {
            spellChecker.Dictionaries.Add(dict);
        }
        static SpellCheckerDictionaryBase GetCustomDictionary(string path, string _culture)
        {
            CultureInfo cInfo = new CultureInfo(_culture);
            SpellCheckerCustomDictionary custom_dict = new SpellCheckerCustomDictionary(String.Format("{0}\\{1}-custom.dic", path, _culture), cInfo);
            custom_dict.Encoding = System.Text.Encoding.GetEncoding(cInfo.TextInfo.ANSICodePage);
            custom_dict.Load();
            return custom_dict;
        }        
        #endregion

    }
}
