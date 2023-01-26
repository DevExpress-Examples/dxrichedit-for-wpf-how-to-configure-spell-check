Imports System
Imports System.Windows
Imports System.Windows.Input
Imports DevExpress.Xpf.SpellChecker
Imports DevExpress.XtraSpellChecker
Imports System.Globalization

Namespace DXRichEdit_SpellCheckMenu

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window

        Public Shared dictPath As String = "Dic"

        Public spellChecker As SpellChecker

        Public Sub New()
            Me.InitializeComponent()
            AddHandler Me.richEdit.Loaded, New RoutedEventHandler(AddressOf richEdit_Loaded)
        End Sub

        Private Sub richEdit_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.richEdit.LoadDocument("Bunin.docx")
        End Sub

        Private Sub button1_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            If CheckForDictionary(spellChecker, "ru") Then Return
            Mouse.OverrideCursor = Cursors.Wait
            Try
                spellChecker = InitializeSpellChecker()
                Me.richEdit.SpellChecker = spellChecker
                spellChecker.SpellCheckMode = SpellCheckMode.AsYouType
                spellChecker.Culture = New CultureInfo("ru-RU")
            Finally
                Mouse.OverrideCursor = Nothing
            End Try
        End Sub

        Private Function CheckForDictionary(ByVal spellChecker As SpellChecker, ByVal langname As String) As Boolean
            If spellChecker IsNot Nothing Then
                If spellChecker.Dictionaries.Count > 0 Then
                    For Each dict As SpellCheckerDictionaryBase In spellChecker.Dictionaries
                        If Equals(dict.Culture.TwoLetterISOLanguageName, langname) Then Return True
                    Next
                End If
            End If

            Return False
        End Function

'#Region "SpellChecker initialization"
        Public Shared Function InitializeSpellChecker() As SpellChecker
            Dim spellChecker As SpellChecker = New SpellChecker()
            spellChecker.Culture = New CultureInfo("en-US")
            Call RegisterDictionary(spellChecker, GetDefaultDictionary(dictPath))
            Call RegisterDictionary(spellChecker, GetCustomDictionary(dictPath, "en-US"))
            Call RegisterDictionary(spellChecker, GetLanguageDictionary(dictPath, "ru-RU"))
            Call RegisterDictionary(spellChecker, GetCustomDictionary(dictPath, "ru-RU"))
            'SpellCheckTextControllersManager.Default.RegisterClass(typeof(RichEditControl), typeof(RichEditSpellCheckController));
            Return spellChecker
        End Function

        Private Shared Function GetDefaultDictionary(ByVal path As String) As SpellCheckerDictionaryBase
            Dim dic As SpellCheckerISpellDictionary = New SpellCheckerISpellDictionary()
            AddHandler dic.DictionaryLoaded, New EventHandler(AddressOf DXRichEdit_SpellCheckMenu.MainWindow.dic_DictionaryLoaded)
            dic.DictionaryPath = path & "\american.xlg"
            dic.GrammarPath = path & "\english.aff"
            dic.AlphabetPath = path & "\EnglishAlphabet.txt"
            dic.Culture = New CultureInfo("en-US")
            dic.Load()
            RemoveHandler dic.DictionaryLoaded, AddressOf DXRichEdit_SpellCheckMenu.MainWindow.dic_DictionaryLoaded
            Return dic
        End Function

        Private Shared Function GetLanguageDictionary(ByVal path As String, ByVal _culture As String) As SpellCheckerDictionaryBase
            Dim dict As SpellCheckerOpenOfficeDictionary = New SpellCheckerOpenOfficeDictionary()
            AddHandler dict.DictionaryLoaded, New EventHandler(AddressOf DXRichEdit_SpellCheckMenu.MainWindow.dic_DictionaryLoaded)
            dict.DictionaryPath = String.Format("{0}\{1}.dic", path, _culture)
            dict.GrammarPath = String.Format("{0}\{1}.aff", path, _culture)
            dict.Culture = New CultureInfo(_culture)
            dict.Load()
            RemoveHandler dict.DictionaryLoaded, AddressOf DXRichEdit_SpellCheckMenu.MainWindow.dic_DictionaryLoaded
            Return dict
        End Function

        Private Shared Sub dic_DictionaryLoaded(ByVal sender As Object, ByVal e As EventArgs)
            Dim dictBase As SpellCheckerDictionaryBase = CType(sender, SpellCheckerDictionaryBase)
            If dictBase Is Nothing Then MessageBox.Show(String.Format("Dictionary for {0} culture is not loaded", dictBase.Culture.DisplayName))
        End Sub

        Private Shared Sub RegisterDictionary(ByVal spellChecker As SpellChecker, ByVal dict As SpellCheckerDictionaryBase)
            spellChecker.Dictionaries.Add(dict)
        End Sub

        Private Shared Function GetCustomDictionary(ByVal path As String, ByVal _culture As String) As SpellCheckerDictionaryBase
            Dim cInfo As CultureInfo = New CultureInfo(_culture)
            Dim custom_dict As SpellCheckerCustomDictionary = New SpellCheckerCustomDictionary(String.Format("{0}\{1}-custom.dic", path, _culture), cInfo)
            custom_dict.Encoding = Text.Encoding.GetEncoding(cInfo.TextInfo.ANSICodePage)
            custom_dict.Load()
            Return custom_dict
        End Function
'#End Region
    End Class
End Namespace
