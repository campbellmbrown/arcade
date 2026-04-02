Getting Started
===============

Creating the Project
--------------------

Create and clone your repo. Add Arcade as a submodule to your repo:

.. code-block:: bash

   git submodule add git@github.com:campbellmbrown/arcade.git Arcade/

Create your project, create a solution, add your project and Arcade to the solution, and add a project reference to Arcade:

.. code-block:: bash

   dotnet new sln -n MyGame
   dotnet new mgdesktopgl -o MyGame
   dotnet sln add MyGame/MyGame.csproj
   dotnet sln add Arcade/Arcade/Arcade.csproj
   dotnet add MyGame/MyGame.csproj reference Arcade/Arcade/Arcade.csproj

Add the following packages to your game project:

.. code-block:: bash

   dotnet add MyGame/MyGame.csproj package MonoGame.Extended --version 3.8
   dotnet add MyGame/MyGame.csproj package MonoGame.Extended.Content.Pipeline --version 3.8

Template Files
--------------

Copy the following template files from the Arcade repository to your project:

.. code-block:: bash

   cp Arcade/.editorconfig .

Required Changes
----------------

Update ``MyGame.csproj`` with the following changes:

.. code-block:: xml

   <Project Sdk="Microsoft.NET.Sdk">
     <PropertyGroup>
       <OutputType>WinExe</OutputType>
       <TargetFramework>net10.0</TargetFramework>
       <RollForward>Major</RollForward>
       <PublishReadyToRun>false</PublishReadyToRun>
       <TieredCompilation>false</TieredCompilation>
       <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
       <Nullable>enable</Nullable>
     </PropertyGroup>
     <PropertyGroup>
       <ApplicationManifest>app.manifest</ApplicationManifest>
       <ApplicationIcon>Icon.ico</ApplicationIcon>
     </PropertyGroup>
     <!-- Allow reference to the MonoGame.Extended pipeline regardless of build location
     See https://docs.monogame.net/articles/getting_started/tools/mgcb.html -->
     <PropertyGroup>
       <MonoGameMGCBAdditionalArguments>/quiet /reference:&quot;$(PkgMonoGame_Extended_Content_Pipeline)/tools/MonoGame.Extended.Content.Pipeline.dll&quot;</MonoGameMGCBAdditionalArguments>
     </PropertyGroup>
     <ItemGroup>
       <None Remove="Icon.ico" />
       <None Remove="Icon.bmp" />
     </ItemGroup>
     <ItemGroup>
       <EmbeddedResource Include="Icon.ico">
         <LogicalName>Icon.ico</LogicalName>
       </EmbeddedResource>
       <EmbeddedResource Include="Icon.bmp">
         <LogicalName>Icon.bmp</LogicalName>
       </EmbeddedResource>
     </ItemGroup>
     <ItemGroup>
       <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.*" />
       <PackageReference Include="MonoGame.Extended.Content.Pipeline" Version="3.8.*" GeneratePathProperty="true" />
       <PackageReference Include="MonoGame.Extended" Version="3.8.*" />
       <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.*" />
     </ItemGroup>
     <ItemGroup>
       <ProjectReference Include="../Arcade/Arcade/Arcade.csproj" />
     </ItemGroup>
     <ItemGroup>
       <MonoGameContentReference Include="../Arcade/Arcade/Content/Content.mgcb" />
     </ItemGroup>
   </Project>


Run the Project
---------------

Create the following ``.vscode/launch.json`` file to enable debugging:

.. code-block:: json

   {
      // Use IntelliSense to learn about possible attributes.
      // Hover to view descriptions of existing attributes.
      // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
      "version": "0.2.0",
      "configurations": [
          {
              "name": "MyGame (Debug)",
              "type": "coreclr",
              "request": "launch",
              "preLaunchTask": "build-MyGame",
              "program": "${workspaceFolder}/MyGame/bin/Debug/net10.0/MyGame.dll",
              "cwd": "${workspaceFolder}/MyGame",
              "stopAtEntry": false,
              "console": "internalConsole"
          }
      ]
   }

Create the following ``.vscode/tasks.json`` file to enable building:

.. code-block:: json

   {
       "version": "2.0.0",
       "tasks": [
           {
               "label": "build-MyGame",
               "type": "process",
               "command": "dotnet",
               "args": [
                   "build",
                   "${workspaceFolder}/MyGame/MyGame.csproj"
                ],
                "problemMatcher": "$msCompile"
            }
        ]
    }

In VS Code, open the debug panel and select the "MyGame (Debug)" configuration, or press F5, or run the following command in the terminal:

.. code-block:: bash

   dotnet run --project MyGame/MyGame.csproj

You should see a window with a white background. Congratulations, you have successfully set up your game project!
