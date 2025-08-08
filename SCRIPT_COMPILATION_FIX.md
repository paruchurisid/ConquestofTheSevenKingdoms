# Script Compilation Fix - Unity Troubleshooting

## Quick Fix Steps

### 1. Force Unity to Recompile
1. **Save all scripts** (Ctrl+S or Cmd+S)
2. **Go to Unity menu**: Assets → Reimport All
3. **Wait** for Unity to finish reimporting (check bottom-right progress bar)
4. **Check Console** for any error messages

### 2. Check Console for Errors
1. **Open Console window**: Window → General → Console
2. **Look for red error messages**
3. **Click on errors** to see which script and line has the problem

### 3. Common Issues and Solutions

#### Issue: "Scripts not found in Add Component menu"
**Solution:**
1. Make sure all .cs files are in `Assets/Scripts/` folder
2. Check that file names match exactly (case-sensitive)
3. Wait for Unity to finish compiling (no spinning icon in bottom-right)

#### Issue: "Missing MonoBehaviour" errors
**Solution:**
1. Select the script in Project window
2. In Inspector, check that "Script" field shows the correct script name
3. If it shows "Missing (MonoBehaviour)", click the circle selector and search for the script

#### Issue: "Namespace" or "using" errors
**Solution:**
1. Make sure all scripts have `using UnityEngine;` at the top
2. Check that `using UnityEngine.UI;` is in scripts that use UI components

### 4. Step-by-Step Import Process

#### Clean Import Method:
1. **Delete existing Scripts folder** in Unity (if it exists)
2. **Create new Scripts folder**: Right-click Project → Create → Folder → "Scripts"
3. **Copy scripts one by one**:
   - Copy `Territory.cs` → paste into Scripts folder
   - Copy `ArmyMovement.cs` → paste into Scripts folder
   - Copy `GameManager.cs` → paste into Scripts folder
   - Copy `AIController.cs` → paste into Scripts folder
   - Copy `TerritoryData.cs` → paste into Scripts folder
   - Copy `MapLoader.cs` → paste into Scripts folder
   - Copy `CameraController.cs` → paste into Scripts folder
   - Copy `UIController.cs` → paste into Scripts folder
4. **Wait** for Unity to compile each script
5. **Check Console** after each script import

### 5. Verify Script Contents

Make sure each script has the correct basic structure:

```csharp
using UnityEngine;
// other using statements as needed

public class ScriptName : MonoBehaviour
{
    // script content
}
```

### 6. Unity Version Compatibility

**Check your Unity version:**
- Go to Help → About Unity
- Make sure you're using Unity 2022.3 LTS or newer
- If using older version, update to latest LTS

### 7. Project Settings Check

1. **Go to**: Edit → Project Settings → Player
2. **Check**: Scripting Backend should be "Mono" or "IL2CPP"
3. **Check**: API Compatibility Level should be ".NET Standard 2.1"

### 8. If Still Not Working

#### Try This Nuclear Option:
1. **Close Unity completely**
2. **Delete the Library folder** from your project (this forces Unity to rebuild everything)
3. **Reopen Unity** and wait for it to reimport everything
4. **Reimport scripts** using the clean import method above

### 9. Test with Simple Script

Create a test script to verify Unity is working:

1. **Right-click Scripts folder** → Create → C# Script
2. **Name it "TestScript"**
3. **Double-click to open** and replace content with:

```csharp
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Test script is working!");
    }
}
```

4. **Save and check Console** for the message

### 10. Common Error Messages

| Error | Solution |
|-------|----------|
| "The type or namespace name 'X' could not be found" | Check using statements, make sure script exists |
| "Assets/Scripts/X.cs(1,1): error CS1513: } expected" | Missing closing brace, check script syntax |
| "Script 'X' has no class definition" | Make sure class name matches file name |
| "Multiple definitions of 'X'" | Duplicate script files, delete duplicates |

## Still Having Issues?

If none of these steps work:
1. **Check Unity Console** for specific error messages
2. **Share the exact error messages** you're seeing
3. **Try creating a new Unity project** and importing scripts there first

The scripts are designed to work with Unity 2022.3+ and Universal 2D Core template. If you're using a different setup, let me know!
