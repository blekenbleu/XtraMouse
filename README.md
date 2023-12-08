# XtraMouse
 SimHub plugin to intercept selected extra mouse events

- Repo created by GitHub Desktop:  
	![](Docs/create.png)  
- Created a new `XtraMouse` Visual Studio solution elsewhere  
	deleted its `.vs/` `obj/` and `Properties/` folders
- Copy the rest of that new solution to the new repo
- To apply new solution and project name to SimHub example,  
	Use VIM split diff in User.PluginSdkDemo folder:
	- push SimHub `VisualStudioVersion` to `XtraMouse.sln`;  
		and remove its `GlobalSection`  
	- push SimHub `<Project ToolsVersion` to `XtraMouse.csproj`  
		copy `<AppDesignerFolder>`,  
		push `<TargetFrameworkProfile>` and `<OutputPath>`  
		push `<OutputPath>` and `<StartProgram>` `<PropertyGroup`  
		push `<Reference Include` `<ItemGroup>`  
	- push `<Compile Include` `<ItemGroup>`, with  
		`"DataPluginDemo.cs"` replacing `"UserControl1.cs"` and  
		`"SettingsControlDemo.xaml.cs"` replacing `"UserControl1.Designer.cs"`  
	- push `<Page Include="Properties\DesignTimeResources.xaml"` `<ItemGroup>`
- Replace corresponding source files
	- remove `Demo` from file names

<details><summary>click for details</summary>

<pre>
bleke@Antec MSYS /d/my/SimHub/PluginSdk/XtraMouse
$ ls | grep Demo
DataPluginDemo.cs
DataPluginDemoSettings.cs
SettingsControlDemo.xaml
SettingsControlDemo.xaml.cs

bleke@Antec MSYS /d/my/SimHub/PluginSdk/XtraMouse
$ mv DataPluginDemo.cs DataPlugin.cs

bleke@Antec MSYS /d/my/SimHub/PluginSdk/XtraMouse
$ mv DataPluginDemoSettings.cs DataPluginSettings.cs

bleke@Antec MSYS /d/my/SimHub/PluginSdk/XtraMouse
$ mv SettingsControlDemo.xaml SettingsControl.xaml

bleke@Antec MSYS /d/my/SimHub/PluginSdk/XtraMouse
$ mv SettingsControlDemo.xaml.cs SettingsControl.xaml.cs

</pre>

</details>

- use VIM to
	- remove 'Demo' from references  
	- replace `My.PluginSdk` with `XtraMouse`
- copy all but `AssemblyInfo.cs` from `PluginSdkDemo/Properties/' to 'XtraMouse/Properties/'
- 2 sections in `XtraMouse.csproj` that help debug:
	- install dll in SimHub folder:
		`<OutputPath>..\..\</OutputPath>`
	- launch SimHub:
```
    <PropertyGroup Condition="'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'">
      <StartAction>Program</StartAction>
      <StartProgram>..\..\SimHubWPF.exe</StartProgram>
    </PropertyGroup>
```


```
