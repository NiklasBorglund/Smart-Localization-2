# Editor Scripting in Smart Localization: Getting Language Values and Keys

Getting your language values not just at runtime, but in the editor can be a valuable tool. Especially if you are creating an editor extension or another system that is based on Smart Localization and the values it produces.

This can be done with a method found in the static class LanguageHandlerEditor named LoadParsedLanguageFile.

If you want only the keys or the comments in the root language file - you can pass in null as the language code and true as the second parameter that indicates that this is the root file.
```csharp
var rootValues = LanguageHandlerEditor.LoadParsedLanguageFile(null, true);
```
If you'd like to have the language values for a specific language - you pass in the language code of the language you want to load (i.e. "en" "sv" etc.") and false as the second parameter.
```csharp
var languageValues = LanguageHandlerEditor.LoadParsedLanguageFile("en", false);
```