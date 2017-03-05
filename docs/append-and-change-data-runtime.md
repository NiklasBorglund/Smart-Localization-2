# Appending and changing language data at runtime
Waiting 6-10 working days for an approval to one of the app stores can be a tedious process - especially if you only want to do something like a minor update in a translation.

One way to circumvent this and push translations directly into your published game would be to create a system to store updates on a server and then checking if there’s an update at a timed interval in your game.

If you are a developer with a solution similar to the one described - we have the following methods that you can use to add/update your translations into the Smart Localization system.

Both of these methods are available in the LanguageManager class and they only take in data in the .resx format.
```csharp
void ChangeLanguageWithData(string languageDataInResX, string languageCode);
```
This method clears the currently loaded language from the system and loads a new one with the specified .resx data and the accompanied language code.
```csharp
bool AppendLanguageWithTextData(string languageDataInResX);
```
Append Language with Text Data takes the data in the resx data and appends it to your currently loaded language. If there’s duplicate keys, the new one will be chosen.