# Elena
Elena is a .net standard utility library for reading various FF7 files. Currently it can extract data from both Kernel files, list and extract files from LGP archives, and read and convert FF7's text file format to images.

## Reading the Kernels

KERNEL.BIN contains full data for all aspects of the game. This include initialisation data, item, materia and equipment statistics, and various game texts.

kernel2.bin contains just game text in a different format - and the game uses this instead of the equivalent text within the game.

### Reading KERNEL.BIN
```cs
// Initialise the reader with the kernel file
KernelReader reader = new KernelReader("KERNEL.BIN", KernelType.KernelBin);

// Example of getting weapon statistics
Weapon[] weapons = reader.WeaponData.Weapons;
```

### Reading kernel2.bin
```cs
KernelReader reader = new KernelReader("KERNEL.BIN", KernelType.KernelBin);

// Example of getting text used within battles
string[] texts = reader.BattleText.Strings;
```

### Getting data as the game sees it.
When merging both kernels data, Elena will use the base stats from KERNEL.BIN and the strings from kernel2.bin. Text fields on items, equipment, etc, is updated to use the values found in kernel.bin.
```cs
// Initialise the reader with both kernels
KernelReader reader = new KernelReader("KERNEL.BIN", KernelType.KernelBin)
                            .MergeKernel2Data("kernel2.bin");

// Example of getting weapon statistics
Weapon[] weapons = reader.WeaponData.Weapons;
```

## Reading LGP Files
```cs
// Initialise the reader with an LGP file
LgpReader lgp = new LgpReader(@"chocobo.lgp");

// Get a list of file names found in the LGP contents
string[] containedFiles = lgp.ListFiles();

// Extract a file to a stream
Stream outputStream = lgp.ExtractFile("bv.tex");
```

## Reading texture and image files
```cs
// Pull a .tex file out. 
using (var dataStream = lgp.ExtractFile("bv.tex"))
{
    // This can be from a FileWriter, or memory stream, or any other destination.
    Stream outputStream; 

    // You can use a stream or byte[] buffer to convert from.
    TexConverter.ToPng(dataStream, outputStream);
    
    // OR if you want to process the bitmap before saving
               
    Bitmap bmp = TexConverter.ToBitmap(dataStream);

    // ...change the image here...

    bmp.Save($"choco-faces.png", ImageFormat.Png);           
}
```