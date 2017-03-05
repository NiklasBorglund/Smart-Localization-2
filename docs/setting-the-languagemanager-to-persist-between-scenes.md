# Setting the LanguageManager to persist between scenes

The LanguageManager singleton instance does not by default persist between scenes. So you need to set this by yourself.

To achieve this, you can use the static method in the LanguageManager class named SetDontDestroyOnLoad()
```csharp
LanguageManager.SetDontDestroyOnLoad();
```
The reason for this is mainly of legacy reasons, but that might change in the future.