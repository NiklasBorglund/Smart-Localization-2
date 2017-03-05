# How to load the device language in Smart Localization
This is a code snippet on how to set Smart Localization to load the system language in the start method of a script.

```csharp
void Start(){
    languageManager = LanguageManager.Instance;
    var deviceCulture = languageManager.GetDeviceCultureIfSupported();
    if(deviceCulture != null){
        languageManager.ChangeLanguage(deviceCulture);
    }
    else{
        //The device language is not available in the current application or it had a region that 
        //the Application.systemLanguage cannot handle
    }
}
```

The built in functions to detect the device language in Smart Localization is entirely based on Application.systemLanguage. That is an enum that only returns the language in plain english and no regional data. So, if you need better control over the device language - writing a native plugin for that is probably the best way to go.