namespace VirtualMachine
{
    public class VmFile(string fileName, IEnumerable<string> contents)
    {
        public string FileName { get; } = fileName;
        public IEnumerable<string> Contents { get; } = contents;
    }
}