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

Make the following changes to ``EchoLocation.csproj``:

#. Change ``net9.0`` to ``net10.0``
#. Add the following to the first property group:

   .. code-block:: xml

      <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
      <Nullable>enable</Nullable>

#. Add a content reference to the Arcade content pipeline:

   .. code-block:: xml

      <ItemGroup>
        <MonoGameContentReference Include="../Arcade/Arcade/Content/Content.mgcb" />
      </ItemGroup>

Make the following changes to ``Game1.cs``:

#. Add a nullable annotation to the ``_spriteBatch`` field declaration:

   .. code-block:: csharp

      private SpriteBatch? _spriteBatch;

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
