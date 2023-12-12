# XtraMouse
 SimHub plugin to intercept selected extra mouse events,  
 using [Model-view-viewmodel pattern](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm) for `PropertyChangedEventHandler`;  
 plugin UI buttons turn red corresponding to pressed buttons of intercepted mice.

- Built using Visual Studio 2022 Community with C# `<langVersion>9.0`  
- Depends on [InputIntercept](https://github.com/blekenbleu/InputIntercept);&nbsp; see [`<HintPath>`](https://github.com/blekenbleu/XtraMouse/blob/main/XtraMouse.csproj#L55)  
	- ... which requires [this installer package](https://github.com/oblitum/Interception/releases/latest) for [the Interception driver](https://www.oblita.com/interception.html).  
- Repo created by GitHub Desktop:  
	![](Docs/create.png)  
- Created a new `XtraMouse` Visual Studio 2022 solution elsewhere  
	deleted its `.vs/` `obj/` and `Properties/` folders
- Copied the rest of that new solution to the new repo
- To insert SimHub example in new solution and project name:    
	Use [VIM](https://www.vim.org/download.php) split diff with [`SimHubPluginSdk`](../SimHubPluginSdk) folder:
	- push SimHub `VisualStudioVersion` to `XtraMouse.sln`;  
		and remove its `GlobalSection`  
	- push SimHub `<Project ToolsVersion` to `XtraMouse.csproj`  
		copy `<AppDesignerFolder>`,  
		push `<TargetFrameworkProfile>` and `<OutputPath>`  
		push `<OutputPath>` and `<StartProgram>` `<PropertyGroup`  
		push `<Reference Include` `<ItemGroup>`  
	- push `<Compile Include` `<ItemGroup>`, with  
		`"DataPlugin.cs"` replacing `"UserControl1.cs"` and  
		`"SettingsControl.xaml.cs"` replacing `"UserControl1.Designer.cs"`  
	- push `<Page Include="Properties\DesignTimeResources.xaml"` `<ItemGroup>`
- Replace corresponding source files
- use VIM to
	- replace `SimHubPluginSdk` with `XtraMouse`
- copy all but `AssemblyInfo.cs` from [`SimHubPluginSdk/Properties/`](../SimHubPluginSdk/Properties/) to `XtraMouse/Properties/`
- [PropertyChanged code](https://github.com/Fody/PropertyChanged) for "live" XAML

### porting from [XPF_XAML](https://github.com/blekenbleu/WPF_XAML)

1. start with view model class `DataViewModel`, limit to 5 mouse buttons  
	builds OK
2. Add [reference for `using InputInterceptorNS;`](https://github.com/blekenbleu/InputIntercept) to XtraMouse in VS Solution Explorer:  
	`<HintPath>..\..\..\GitHub\InputIntercept\bin\Release\netstandard2.0\InputIntercept.dll</HintPath>`  

3. Add `Intercept.cs` and class with trivial contents  
	**Note**: &nbsp; adding `Intercept.cs` file in VS *does NOT* add the class;  
	that is a separate step under Solution Exploder, what a PoS...  

<details><summary>trivial <code>Intercept.cs</code>, just to build and debug the project</summary>

<pre>
using InputInterceptorNS;
using System;

namespace XtraMouse
{
    internal class Intercept
    {
        public static short Captured = 0;
        public delegate void WriteStatus(string s);
        static WriteStatus Writestring = Console.WriteLine;
        public delegate void ButtonDel(ushort index, bool down);
        static ButtonDel ButtonEvent = Dummy;
        public static short[] Stroke = { 0,0,0,0,0 };


        static void Dummy(ushort index, bool down) { }

        public Intercept()
        {
        }
    }
}

</pre>

</details>

4. replaced SimHUb demo UI XAML content with that from WPF_XAML  

5.  Added to `XtraMouse.csproj` for `MouseHook` callback syntax:  
```
  <PropertyGroup>
    <LangVersion>9.0</LangVersion>
  </PropertyGroup>
```
6. pushed `XPF_XAML/Intercept.cs` contents to `Intercept.cs`  
	except for namespace, they are nearly identical...  

7. Declare `Intermouse` in `DataPlugin.cs` `public class DataPlugin`;  
	- `Intermouse?.End();` in `DataPlugin.End()`

8. pushed methods from `XPF_XAML/MainWindow.xaml.cs` to `SettingsControl.xaml.cs`
	- `this.DataContext = _mainViewModel;`
	- Instance `Intermouse` in `SettingsControl.xaml.cs` *after* `InputInterceptor.Initialize()` success.  
	![](Docs/debug.png)  
#### status / To Do
- add properties, button events, 
- identify Settings to save and restore, e.g. selected mouse
- proper icon

#### mouse interception plugin issues
- when to continue using designated mouse?
- [elapsed time from plugin end to restart](https://stackoverflow.com/questions/2821040/how-do-i-get-the-time-difference-between-two-datetime-objects-using-c)
- continue using a mouse if the same device number and hardware ID? 
