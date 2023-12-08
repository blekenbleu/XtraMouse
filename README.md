# XtraMouse
 SimHub plugin to intercept selected extra mouse events

- Repo created by GitHub Desktop:  
	![](Docs/create.png)  
- Created a new `XtraMouse` Visual Studio solution elsewhere  
	deleted its `.vs/` `obj/` and `Properties/` folders
- Copied the rest of that new solution to the new repo
- To insert SimHub example in new solution and project name:    
	Use VIM split diff with [`SimHubPluginSdk`](../SimHubPluginSdk) folder:
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
#### mouse interception plugin issues
- [elapsed time from plugin end to restart](https://stackoverflow.com/questions/2821040/how-do-i-get-the-time-difference-between-two-datetime-objects-using-c)
- continue using a mouse if the same device number and hardware ID? 

