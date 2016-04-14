18/1/2016: 0.1.0  
Initial release  
  
24/1/2016: 0.2.0  
Steam install auto-detection  
Open and Save As dialogs default to KSP/Ships/Scripts(Per the steam install detection)  
Dark mode  
Upgrade detection(Tools>Upgrade)  
  
10/2/2016: 0.3.0  
Tabs!  
Linux version!  
Fixed issue #1 through use of try-catch.  
Changed undo/redo behaviour: Buffer now 10char long with (hopefully) unlimited actions  
Added report bug and kOS docu to help menu  
Added graphical toolbar  
Export code in markup  
General code cleanup  
  
20/02/2016: 0.4.0  
Reworked steam install detection  
Reworked session restore when settings changed(Issue #4)  
Redesigned the settings menu(It finally looks good!)  
Added support for multiple installs(Steam or not)(When you add the install, Make sure it is too your root KSP directory)  
Added support for lowercase syntax highlighting(Issue #5)  
Added the code minimiser at long last, It will strip comments, empty lines and double spaces.  
Tweaked save all and save as behaviour.  
  
08/04/2016: 0.5.0  
Project mode added  
Support for libraries  
Upgraded Linux Installer  
Windows Installer  
Add support for opening files via explorer  
Added auto indenting  
Added tab space setting for auto indenting  
Added keyboard shortcuts  
Added a code minimiser  
Added column count  
Removed now defunct "Export to KSP" button  
Kode now checks for unsaved changes on exit  
Kode now supports launching by double clicking files in explorer(See known issues below)  
Fixes bug where Kode crashes when you open with no tabs  
Fixes bug where Kode crashes if you don't select a folder on first start  
Fixes bug where Kode will treat recovered tabs as new ones on setting change restart  
Fixes beta bug where Export to KSP crashes when not in project mode  
Worked around a bug where Kode auto restart doesn't change UI correctly. Manual restart is required after changing editor/project modes.  
All changes to file will now trigger an *, not just additions  
Tab close X behaviour tweaked   
Syntax highlighting and autocomplete now support UPPER, lower and Mixed Case without changing settings  
Update auto-detect(Added in 0.4 but this is the first release to use it)  
