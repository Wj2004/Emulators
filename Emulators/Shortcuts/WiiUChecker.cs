using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Emulators.Shortcuts
{
    class WiiUChecker
    {
        public static string CheckForWiiUFolder(string path)
        {
            var filesInFolder = Directory.GetFiles($"{path}", "*.*", SearchOption.AllDirectories);
            foreach (string file in filesInFolder)
            {
                if (Path.GetExtension(file).Equals(".rpx") || Path.GetExtension(file).Equals(".wud") || Path.GetExtension(file).Equals(".wux") || Path.GetExtension(file).Equals(".wad") || Path.GetExtension(file).Equals(".elf"))
                {
                    return file;
                }
            }
            return null;
        }
    }
}
