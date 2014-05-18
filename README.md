SHCD-2014-InfoTech
====
A C#-based exam application used for really basic IT exams, decompiled and released by a stupid student since he found that the DLLs included something GPL. Only the mock test system and included libraries are available. 

**Always use at your own risk -- It is said that GNU GPL is against the Chinese Copyright Act.**

What does it include?
----
###General/Qisi.General:
+ FTP-based Client-Server structure.
+ Zip-based Compression with AES encryption to transfer exam data.
+ Xlsx file reading ability.

###Ctrl/Qisi.General.Control:
+ CoreAudioApi
+ Buttons, imebar and so on

###Editor/Qisi.Editor:
+ An RTF Editor with Maths functions availablities.

###ExamCtrl/ExamClientControlsLibrary:
+ RTF Reading for test papers.

###SHCD:
+ The CD mock test client.

###Updater:
+ Network-based updater.

Extras
====
This is the decompiled application of the mock exam system. Released under GNU GPL v2 or (at your option) any later version.

Target modifications include UAC, tempdir, deleting registry dependency (in order to be more mono-friendly) and so on.

A Keygen should also be made, although I am going to create a key-less version first.

This kind of modified app should also work with other similar apps. 

Notes
====
Well, it seems that I should first consider building a KeyGen.

ILSpy didn't work so well...
<hr />
Now everything except Editor compiles, so I can't test the ExamCtrl yet.

Here is a list of required SharpZipLib namespaces:
```
ICSharpCode.SharpZipLib
ICSharpCode.SharpZipLib.Checksums
ICSharpCode.SharpZipLib.Core
ICSharpCode.SharpZipLib.Encryption
ICSharpCode.SharpZipLib.Zip
ICSharpCode.SharpZipLib.Zip.Compression
ICSharpCode.SharpZipLib.Zip.Compression.Streams
```
Use this to minimize space requirements.

The decompiled (compiled, modified and minimized(decompiled)) version of Microsoft CoreAudioApi extracted from `Qisi.General.Controls` is compiled and given now.
