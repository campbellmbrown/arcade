Contribution Guide
==================

Unit Testing
------------

To run unit tests:

.. code-block:: console

    dotnet test

Formatting
----------

Run code formatting with the `dotnet format <https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-format>`_ tool
which is included with the .NET SDK.

To format the entire solution, run:

.. code-block:: console

    dotnet format --verify-no-changes

Omit the ``--verify-no-changes`` flag to allow the tool to automatically apply the formatting changes for you.

Line Endings
~~~~~~~~~~~~

If you see lots of these errors when checking formatting, your line endings are incorrect:

.. code-block::

    error WHITESPACE: Fix whitespace formatting.
    error ENDOFLINE: Fix end of line marker.

Formatting requires line endings to be set to LF (see ``end_of_line`` in ``.editorconfig``).
This setting *cannot* be set to CRLF, as that would break builds on Linux.
Therefore, all source files should be in the work tree with LF line endings (even on Windows).
This is controlled by the ``.gitattributes`` file.

Creating a fresh clone of the repository will automatically set up the correct line endings.
However, if you already have a clone with incorrect line endings, you can fix it by running:

.. code-block:: console

    git rm --cached -r .
    git reset --hard

Check that the line endings are now correct by running:

.. code-block:: console

    git ls-files --eol | grep -E '\.cs'

Or on PowerShell:

.. code-block:: powershell

    git ls-files --eol | Select-String '\.cs$'

The expected output is ``i/lf    w/lf    attr/text eol=lf`` for all source files.

Install the `EditorConfig extension <https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig>`_
in VS Code to automatically save files with the correct line endings.
