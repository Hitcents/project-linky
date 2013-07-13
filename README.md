project linky
=============

A project linking tool for cross platform development. Still a prototype, but should still be pretty useful for keeping `*.csproj` files in sync. 

`linky` takes in an xml file for input, and adds/removes linked files 

Example file:

```
<?xml version="1.0"?>
<linky xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <project path="Code\Android\YourApp.Droid\YourApp.Droid.csproj">
    <rule inputPattern="Assets\Binary\*.bin" outputPattern="Assets\Binary\" buildAction="AndroidAsset" />
    <rule inputPattern="Assets\Images\Tiled\*.png" outputPattern="Assets\Content\Images\" buildAction="AndroidAsset" />
    <rule inputPattern="Assets\Images\Spritesheets\*.png" outputPattern="Assets\Content\Images\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Fonts\*.xnb" outputPattern="Assets\Content\Fonts\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Sounds\*.mp3" outputPattern="Assets\Content\Sounds\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Music\*.mp3" outputPattern="Assets\Content\Music\" buildAction="AndroidAsset" />
  	<rule inputPattern="Code\YourApp.Shared\*.cs" outputPattern="Shared\" excludePattern="AssemblyInfo|Local.+Loader" buildAction="Compile" />
  </project>
  <project path="Code\Android\YourApp.Samsung\YourApp.Samsung.csproj">
    <rule inputPattern="Assets\Binary\*.bin" outputPattern="Assets\Binary\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Images\Tiled\*.png" outputPattern="Assets\Content\Images\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Images\Spritesheets\*.png" outputPattern="Assets\Content\Images\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Fonts\*.xnb" outputPattern="Assets\Content\Fonts\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Sounds\*.mp3" outputPattern="Assets\Content\Sounds\" buildAction="AndroidAsset" />
  	<rule inputPattern="Assets\Music\*.mp3" outputPattern="Assets\Content\Music\" buildAction="AndroidAsset" />
  	<rule inputPattern="Code\YourApp.Shared\*.cs" outputPattern="Shared\" excludePattern="AssemblyInfo|Local.+Loader" buildAction="Compile" />
  </project>
  <project path="Code\iOS\YourApp.iOS\YourApp.iOS.csproj">
    <rule inputPattern="Assets\Binary\*.bin" outputPattern="Content\Binary\" buildAction="Content" />
  	<rule inputPattern="Assets\XNB\Images\*.xnb" outputPattern="Content\Images\" buildAction="Content" />
  	<rule inputPattern="Assets\Images\Spritesheets\*.png" outputPattern="Content\Images\" buildAction="Content" />
  	<rule inputPattern="Assets\Fonts\*.xnb" outputPattern="Content\Fonts\" buildAction="Content" />
  	<rule inputPattern="Assets\Sounds\*.mp3" outputPattern="Content\Sounds\" buildAction="Content" />
  	<rule inputPattern="Assets\Music\*.mp3" outputPattern="Content\Music\" buildAction="Content" />
  	<rule inputPattern="Code\YourApp.Shared\*.cs" outputPattern="Shared\" excludePattern="AssemblyInfo|Local.+Loader" buildAction="Compile" />
  </project>
</linky>
```

Drop this in the root of your source control repository, name the file `linky.xml`.

Values:

* `path` - relative path from `linky.xml` to a `*.csproj` file
* `inputPattern` - passed to `Directory.EnumerateFiles` for matching input files
* `outputPattern` - folder in the project to add the linked file to
* `excludePattern` - if desired, you can add a regular expression to exclude files
* `buildAction` - the build action for the linked file

To update files, merely run `linky` in the root of your source control directory, or `linky --dry-run` to see what would be changed first.

Note:

* Still a prototype, make sure your project files are backed up (or in source control)
* Works on Windows only right now, has some path issues with `\` versus `/`
* Needs conventions - right now writing rules for a project can take some time
* I'll be improving the code and adding features over time
